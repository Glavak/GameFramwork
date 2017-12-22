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
        public List<TNetworkConnection> NotHelloedConnections { get; set; }

        private INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory;

        private Guid ownId;
        private bool disposed = false;

        public NetworkRelay(INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory)
        {
            KBuckets = new List<Contact<TNetworkAddress>>[128];
            for (int i = 0; i < KBuckets.Length; i++)
            {
                KBuckets[i] = new List<Contact<TNetworkAddress>>();
            }

            NotHelloedConnections = new List<TNetworkConnection>();

            this.connectionFactory = connectionFactory;
            this.connectionFactory.OnClientConnected += OnClientConnected;

            ownId = DhtUtils.GeneratePlayerId();
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
            NotHelloedConnections.Add(connection);

            HelloNetworkMessage message = new HelloNetworkMessage(ownId);
            connection.Send(message);
        }

        public NetworkFile GetFile(Guid fileId)
        {
            throw new NotImplementedException();
        }

        public int GetConnectedClientsCount()
        {
            int count = 0;
            foreach (var kBucket in KBuckets)
            {
                count += kBucket.Count;
            }
            return count;
        }

        private void OnClientConnected(object sender, TNetworkConnection clientConnection)
        {
            NotHelloedConnections.Add(clientConnection);
            clientConnection.OnRecieve += OnMessageRecieved;

            HelloNetworkMessage message = new HelloNetworkMessage(ownId);
            clientConnection.Send(message);
        }

        private void OnMessageRecieved(object sender, INetworkMessage e)
        {
            TNetworkConnection senderConnection = (TNetworkConnection)sender;

            if (e is HelloNetworkMessage)
            {
                int bucketIndex = DhtUtils.DistanceExp(ownId, e.From);
                Contact<TNetworkAddress> contact = KBuckets[bucketIndex].FirstOrDefault(c => c.Id == e.From);

                if (contact == null)
                {
                    NotHelloedConnections.Remove(senderConnection);
                    contact = new Contact<TNetworkAddress>
                    {
                        Id = e.From,
                        NetworkConnection = senderConnection
                    };
                    KBuckets[bucketIndex].Add(contact);
                }
            }
        }
    }
}
