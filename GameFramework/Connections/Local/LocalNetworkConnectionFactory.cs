using System;
using System.Collections.Generic;
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

        private List<LocalNetworkConnection> generatedConnections = new List<LocalNetworkConnection>();

        public LocalNetworkConnectionFactory(int address, LocalNetworkConnectionHub parent)
        {
            this.address = address;
            this.parent = parent;
        }

        public Task<LocalNetworkConnection> ConnectToAsync(
            int connectTo,
            EventHandler<INetworkMessage> onMessageRecievedHandler = null)
        {
            var connectionMeToOther = new LocalNetworkConnection(connectTo);
            var connectionOtherToMe = new LocalNetworkConnection(address);

            connectionMeToOther.OtherEnd = connectionOtherToMe;
            connectionOtherToMe.OtherEnd = connectionMeToOther;

            if (onMessageRecievedHandler != null)
                connectionMeToOther.OnRecieve = onMessageRecievedHandler;

            generatedConnections.Add(connectionMeToOther);

            var task = InvokeClientConnectedWithDelayAsync(parent.NodeFactories[connectTo], connectionOtherToMe);
            LocalNetworkConnectionHub.TasksToWait.Add(task);

            return Task.FromResult(connectionMeToOther);
        }

        private static async Task InvokeClientConnectedWithDelayAsync(
            LocalNetworkConnectionFactory otherFactory,
            LocalNetworkConnection connection)
        {
            await Task.Delay(5);
            otherFactory.OnClientConnected.Invoke(otherFactory, connection);
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
                foreach (var generatedConnection in generatedConnections)
                {
                    var task = InvokeConnectionDroppedAsync(generatedConnection);
                    LocalNetworkConnectionHub.TasksToWait.Add(task);
                }
            }

            disposed = true;
        }

        private static async Task InvokeConnectionDroppedAsync(LocalNetworkConnection connection)
        {
            await Task.Delay(10);
            connection.OtherEnd.OnConnectionDropped?.Invoke(connection.OtherEnd, null);
        }
    }
}
