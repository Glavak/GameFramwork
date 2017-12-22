using System;

namespace GameFramework
{
    public interface INetworkConnection<TAddress> : IDisposable
    {
        void Send(INetworkMessage message);

        EventHandler<INetworkMessage> OnRecieve { get; set; }

        TAddress Address { get; }
    }
}
