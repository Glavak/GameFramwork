using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GameFramework
{
    public class LocalNetworkConnection : INetworkConnection<int>
    {
        public EventHandler<INetworkMessage> OnRecieve { get; set; }
        public EventHandler OnConnectionDropped { get; set; }

        public int Address { get; set; }
        public LocalNetworkConnection OtherEnd { get; set; }

        private bool disposed = false;

        public LocalNetworkConnection(int addressTo)
        {
            this.Address = addressTo;
        }

        public void Send(INetworkMessage message)
        {
            OtherEnd.OnRecieve.Invoke(OtherEnd, message);
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
                OtherEnd.OnConnectionDropped.Invoke(OtherEnd, null);
            }

            disposed = true;
        }
    }
}
