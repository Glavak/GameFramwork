using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class LocalNetworkConnectionFactory :
        INetworkConnectionFactory<LocalNetworkConnection, int>
    {
        public EventHandler<LocalNetworkConnection> OnClientConnected { get; set; }

        private readonly int address;
        private readonly LocalNetworkConnectionHub parent;
        private bool disposed = false;

        public LocalNetworkConnectionFactory(int address, LocalNetworkConnectionHub parent)
        {
            this.address = address;
            this.parent = parent;
        }

        public Task<LocalNetworkConnection> ConnectToAsync(int connectTo,
            EventHandler<INetworkMessage> onMessageRecievedHandler = null)
        {
            var connectionMeToOther = new LocalNetworkConnection(connectTo);
            var connectionOtherToMe = new LocalNetworkConnection(address);

            connectionMeToOther.OtherEnd = connectionOtherToMe;
            connectionOtherToMe.OtherEnd = connectionMeToOther;

            if (onMessageRecievedHandler != null)
                connectionMeToOther.OnRecieve = onMessageRecievedHandler;

            parent.NodeFactories[connectTo].OnClientConnected(parent.NodeFactories[connectTo], connectionOtherToMe);

            return Task.FromResult(connectionMeToOther);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
            }

            disposed = true;
        }
    }
}
