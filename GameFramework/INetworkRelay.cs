using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay<TNetworkConnection, TNetworkAddress> : IDisposable
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        EventHandler<byte[]> OnDirectMessage { get; set; }

        Task<bool> ConnectToNodeAsync(TNetworkAddress address);

        void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved);

        Guid CreateNewFile(Dictionary<string, string> entries);

        void UpdateFile(Guid fileId, Dictionary<string, string> entries);

        void SendDirectMessage(Guid target, byte[] data);
    }
}
