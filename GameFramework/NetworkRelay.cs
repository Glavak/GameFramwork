using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class NetworkRelay<TNetworkConnection, TNetworkAddress> : INetworkRelay
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        public List<Contact<TNetworkAddress>>[] KBuckets { get; set; }
        public List<TNetworkConnection> NotHelloedConnections { get; set; }

        private INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory;

        private Guid ownId;

        public NetworkRelay(INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory)
        {
            KBuckets = new List<Contact<TNetworkAddress>>[128];
            for (int i = 0; i < KBuckets.Length; i++)
            {
                KBuckets[i] = new List<Contact<TNetworkAddress>>();
            }

            this.connectionFactory = connectionFactory;
            this.connectionFactory.OnClientConnected += OnClientConnected;
            this.connectionFactory.StartListening();

            ownId = DhtUtils.GeneratePlayerId();
        }

        public void Dispose()
        {
            this.connectionFactory.StopListening();
        }

        public NetworkFile GetFile(Guid fileId)
        {
            throw new NotImplementedException();
        }

        public void Start(IEnumerable<IPEndPoint> startupNodes)
        {

        }

        public void Stop()
        {

        }

        private void OnClientConnected(object sender, TNetworkConnection clientConnection)
        {
            NotHelloedConnections.Add(clientConnection);
            clientConnection.OnRecieve += OnMessageRecieved;
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
