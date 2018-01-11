using System;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay<TNetworkConnection, TNetworkAddress> : IDisposable
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        Task ConnectToNodeAsync(TNetworkAddress address);

        void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved);
    }
}
