using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class LocalNetworkConnection : INetworkConnection<int>
    {
        public EventHandler<INetworkMessage> OnRecieve { get; set; }
        public EventHandler OnConnectionDropped { get; set; }

        public int Address { get; set; }
        public LocalNetworkConnection OtherEnd { get; set; }

        private readonly ConcurrentQueue<INetworkMessage> pendingMessages = new ConcurrentQueue<INetworkMessage>();

        private bool disposed = false;

        public LocalNetworkConnection(int addressTo)
        {
            this.Address = addressTo;
        }

        public void StartRecievingTask()
        {
            Task.Run(async () =>
            {
                while (!disposed)
                {
                    if (pendingMessages.TryDequeue(out INetworkMessage message))
                    {
                        OnRecieve?.Invoke(this, message);
                    }
                    else if (OtherEnd.disposed)
                    {
                        OnConnectionDropped?.Invoke(this, null);
                        Dispose(true);
                    }
                    else
                    {
                        await Task.Delay(3);
                    }
                }
            });
        }

        public void Send(INetworkMessage message)
        {
            OtherEnd.pendingMessages.Enqueue(message);
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
                // Other end will call OnConnectionDropped when sees our disposed flag rises
            }

            disposed = true;
        }
    }
}
