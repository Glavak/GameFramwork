using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class LocalNetworkConnectionFactory :
        INetworkConnectionFactory<LocalNetworkConnection, int>
    {
        public EventHandler<LocalNetworkConnection> OnClientConnected { get; set; }

        public bool NatSimulation { get; set; } = false;

        private readonly ConcurrentBag<LocalNetworkConnection> pendingConnections =
            new ConcurrentBag<LocalNetworkConnection>();

        private readonly List<LocalNetworkConnection> ownedConnections = new List<LocalNetworkConnection>();

        private readonly int address;
        private readonly LocalNetworkConnectionHub parent;
        private bool disposed = false;

        public LocalNetworkConnectionFactory(int address, LocalNetworkConnectionHub parent)
        {
            this.address = address;
            this.parent = parent;
            this.StartAccepting();
        }

        private void StartAccepting()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (pendingConnections.TryTake(out LocalNetworkConnection connection))
                    {
                        OnClientConnected.Invoke(this, connection);
                        connection.StartRecievingTask();
                    }
                    else
                    {
                        await Task.Delay(3); // HACK: but maybe not so bad, it's a lag simulation
                    }
                }
            });
        }

        public Task<LocalNetworkConnection> ConnectToAsync(
            int connectTo,
            EventHandler<INetworkMessage> onMessageRecievedHandler = null)
        {
            if (parent.NodeFactories[connectTo].NatSimulation)
            {
                throw new IOException("This node is under NAT and can't be connected to");
            }

            var connectionMeToOther = new LocalNetworkConnection(connectTo);
            var connectionOtherToMe = new LocalNetworkConnection(address);

            connectionMeToOther.OtherEnd = connectionOtherToMe;
            connectionOtherToMe.OtherEnd = connectionMeToOther;

            if (onMessageRecievedHandler != null)
                connectionMeToOther.OnRecieve = onMessageRecievedHandler;

            parent.NodeFactories[connectTo].pendingConnections.Add(connectionOtherToMe);

            ownedConnections.Add(connectionMeToOther);
            parent.NodeFactories[connectTo].ownedConnections.Add(connectionOtherToMe);

            connectionMeToOther.StartRecievingTask();
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
                foreach (var connection in ownedConnections)
                {
                    connection.Dispose();
                }
            }

            disposed = true;
        }
    }
}
