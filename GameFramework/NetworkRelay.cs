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

            HelloNetworkMessage message = new HelloNetworkMessage(ownId);
            clientConnection.Send(message);
        }

        private void OnMessageRecieved(object sender, INetworkMessage e)
        {
            TNetworkConnection senderConnection = (TNetworkConnection) sender;

            // TODO: remake this switch
            INetworkMessage replyMessage = null;

            Dictionary<Guid, TNetworkAddress> closestContacts;
            switch (e)
            {
                case HelloNetworkMessage message:
                    int bucketIndex = DhtUtils.DistanceExp(ownId, message.From);
                    Contact<TNetworkAddress> contact = KBuckets[bucketIndex].FirstOrDefault(c => c.Id == message.From);

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
            }

            if (replyMessage != null) senderConnection.Send(replyMessage);
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
