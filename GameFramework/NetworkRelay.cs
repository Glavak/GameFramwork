using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameFramework
{
    public class NetworkRelay<TNetworkConnection, TNetworkAddress> :
        INetworkRelay<TNetworkConnection, TNetworkAddress>
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        public List<Contact<TNetworkAddress>>[] KBuckets { get; set; }

        private readonly INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory;

        private readonly Guid ownId;
        private bool disposed = false;

        private Dictionary<Guid, NetworkFile> files = new Dictionary<Guid, NetworkFile>();

        public NetworkRelay(INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory)
        {
            KBuckets = new List<Contact<TNetworkAddress>>[128];
            for (int i = 0; i < KBuckets.Length; i++)
            {
                KBuckets[i] = new List<Contact<TNetworkAddress>>();
            }

            this.connectionFactory = connectionFactory;
            this.connectionFactory.OnClientConnected += OnClientConnected;

            ownId = DhtUtils.GeneratePlayerId();

            var ownFile = new NetworkFile(ownId);
            ownFile.entries.Add("nickname", "client");
            files.Add(ownId, ownFile);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                connectionFactory.Dispose();
            }

            disposed = true;
        }

        public async Task ConnectToNodeAsync(TNetworkAddress address)
        {
            if (IsAddressAlreadyConnected(address)) return;

            TNetworkConnection connection = await connectionFactory.ConnectToAsync(address, OnMessageRecieved);

            HelloNetworkMessage message = new HelloNetworkMessage(ownId);
            connection.Send(message);
        }

        public NetworkFile GetFile(Guid fileId)
        {
            throw new NotImplementedException();
        }

        public int GetConnectedClientsCount()
        {
            return KBuckets.Sum(kBucket => kBucket.Count);
        }

        private void OnClientConnected(object sender, TNetworkConnection clientConnection)
        {
            clientConnection.OnRecieve += OnMessageRecieved;
            clientConnection.OnConnectionDropped += OnConnectionDropped;

            HelloNetworkMessage message = new HelloNetworkMessage(ownId);
            clientConnection.Send(message);
        }

        private void OnConnectionDropped(object sender, EventArgs eventArgs)
        {
            TNetworkConnection senderConnection = (TNetworkConnection)sender;

            foreach (var kBucket in KBuckets)
            {
                if (kBucket.RemoveAll(c => c.NetworkConnection.Address.Equals(senderConnection.Address)) > 0)
                {
                    break;
                }
            }
        }

        private void OnMessageRecieved(object sender, INetworkMessage e)
        {
            TNetworkConnection senderConnection = (TNetworkConnection)sender;

            INetworkMessage replyMessage = null;
            Dictionary<Guid, TNetworkAddress> closestContacts;
            Contact<TNetworkAddress> contact = GetContact(e.From);
            // TODO: remake this switch
            switch (e)
            {
                case HelloNetworkMessage message:
                    int bucketIndex = DhtUtils.DistanceExp(ownId, message.From);

                    if (contact == null)
                    {
                        contact = new Contact<TNetworkAddress>
                        {
                            Id = message.From,
                            NetworkConnection = senderConnection
                        };
                        KBuckets[bucketIndex].Add(contact);
                    }

                    closestContacts = GetClosestContacts(message.From, 10);
                    replyMessage = new NodeListNetworkMessage<TNetworkAddress>(ownId, closestContacts);

                    break;

                case GetFileNetworkMessage message:
                    if (files.TryGetValue(message.FileId, out NetworkFile file))
                    {
                        replyMessage = new GotFileNetworkMessage(ownId, file);
                    }
                    else
                    {
                        closestContacts = GetClosestContacts(message.FileId, 10);
                        replyMessage = new NodeListNetworkMessage<TNetworkAddress>(ownId, closestContacts);
                    }

                    break;

                case GetClosestNodesNetworkMessage message:
                    closestContacts = GetClosestContacts(message.FileId, 10);
                    replyMessage = new NodeListNetworkMessage<TNetworkAddress>(ownId, closestContacts);
                    break;

                case NodeListNetworkMessage<TNetworkAddress> message:
                    if (contact != null)
                    {
                        contact.LastUseful = DateTime.Now;
                    }

                    foreach (var node in message.Nodes)
                    {
                        if (node.Key != ownId)
                        {
                            ConnectToNodeAsync(node.Value).Wait();
                        }
                    }

                    break;
            }

            if (replyMessage != null) senderConnection.Send(replyMessage);
        }

        private bool IsAddressAlreadyConnected(TNetworkAddress address)
        {
            return KBuckets.Any(b => b.Any(c => c.NetworkConnection.Address.Equals(address)));
        }

        private Contact<TNetworkAddress> GetContact(Guid id)
        {
            int bucketIndex = DhtUtils.DistanceExp(ownId, id);

            return KBuckets[bucketIndex].FirstOrDefault(c => c.Id == id);
        }

        private void CheckKBucket(int index)
        {
            var kBucket = KBuckets[index];

            if (kBucket.Count > 10)
            {
                kBucket.Sort((a, b) => a.LastUseful.CompareTo(b.LastUseful));
                kBucket.RemoveAt(kBucket.Count - 1);
            }
        }

        private Dictionary<Guid, TNetworkAddress> GetClosestContacts(Guid nodeId, int maxContacts)
        {
            var allContacts = KBuckets.SelectMany(b => b);
            var closestContacts = allContacts.OrderBy(c => DhtUtils.XorDistance(c.Id, nodeId));
            return closestContacts
                .Take(maxContacts)
                .ToDictionary(c => c.Id, c => c.NetworkConnection.Address);
        }
    }
}
