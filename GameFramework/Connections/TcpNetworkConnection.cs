using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GameFramework
{
    public class TcpNetworkConnection : INetworkConnection<IPEndPoint>
    {
        public EventHandler<INetworkMessage> OnRecieve { get; set; }

        public IPEndPoint Address => (IPEndPoint)client.Client.RemoteEndPoint;

        private readonly TcpClient client;
        private BinaryFormatter formatter;
        private bool disposed = false;

        public TcpNetworkConnection(TcpClient client)
        {
            this.client = client;
            
            formatter = new BinaryFormatter();
        }

        public void Send(INetworkMessage message)
        {
            formatter.Serialize(client.GetStream(), message);
        }

        public void StartRecievingTask()
        {
            Task.Run(() =>
            {
                while (!disposed)
                {
                    INetworkMessage m = (INetworkMessage)formatter.Deserialize(client.GetStream());
                    OnRecieve?.Invoke(this, m);
                }
            });
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
                client.Close();
            }

            formatter = null;

            disposed = true;
        }
    }
}
