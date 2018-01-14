using System;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay<TNetworkConnection, TNetworkAddress> : IDisposable
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        EventHandler<byte[]> OnDirectMessage { get; set; }

        Task<bool> ConnectToNodeAsync(TNetworkAddress address);

        void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved);

        void SendDirectMessage(Guid target, byte[] data);
    }
}
