using System;

namespace GameFramework
{
    public interface INetworkConnection<out TAddress> : IDisposable
    {
        void Send(INetworkMessage message);

        EventHandler<INetworkMessage> OnRecieve { get; set; }

        EventHandler OnConnectionDropped { get; set; }

        TAddress Address { get; }
    }
}
