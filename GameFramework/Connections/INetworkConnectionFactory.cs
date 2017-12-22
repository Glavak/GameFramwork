using System;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkConnectionFactory<TConection, TAddress> : IDisposable
        where TConection : INetworkConnection<TAddress>
    {
        EventHandler<TConection> OnClientConnected { get; set; }

        Task<TConection> ConnectToAsync(TAddress address, EventHandler<INetworkMessage> onMessageRecievedHandler = null);
    }
}
