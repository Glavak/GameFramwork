using System;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay<TNetworkConnection, TNetworkAddress> : IDisposable
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        Task ConnectToNodeAsync(TNetworkAddress address);

        NetworkFile GetFile(Guid fileId);
    }
}
