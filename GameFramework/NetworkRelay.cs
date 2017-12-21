using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class NetworkRelay : INetworkRelay
    {
        public List<Contact>[] KBuckets { get; set; }
        public List<TcpNetworkConnection> NotHelloedConnections { get; set; }

        private TcpNetworkConnectionFactory connectionFactory;

        private Guid ownId;

        public NetworkRelay(TcpNetworkConnectionFactory connectionFactory)
        {
            KBuckets = new List<Contact>[128];
            for (int i = 0; i < 128; i++)
            {
                KBuckets[i] = new List<Contact>();
            }

            this.connectionFactory = connectionFactory;
            this.connectionFactory.OnClientConnected += OnClientConnected;

            ownId = DhtUtils.GeneratePlayerId();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public NetworkFile GetFile(Guid fileId)
        {
            throw new NotImplementedException();
        }

        public void Start(IEnumerable<IPEndPoint> startupNodes)
        {
            connectionFactory.StartListening(4242);
        }

        public void Stop()
        {

        }

        private void OnClientConnected(object sender, TcpNetworkConnection clientConnection)
        {
            NotHelloedConnections.Add(clientConnection);
            clientConnection.OnRecieve += OnMessageRecieved;
        }

        private void OnMessageRecieved(object sender, INetworkMessage e)
        {
            TcpNetworkConnection senderConnection = (TcpNetworkConnection)sender;

            if (e is HelloNetworkMessage)
            {
                int bucketIndex = DhtUtils.DistanceExp(ownId, e.From);
                Contact contact = KBuckets[bucketIndex].FirstOrDefault(c => c.Id == e.From);

                if (contact == null)
                {
                    NotHelloedConnections.Remove(senderConnection);
                    contact = new Contact
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
