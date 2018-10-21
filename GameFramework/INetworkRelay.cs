using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay<TNetworkAddress> : IDisposable
    {
        Guid OwnId { get; }

        EventHandler<byte[]> OnDirectMessage { get; set; }

        Task<bool> ConnectToNodeAsync(TNetworkAddress address);

        void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved);

        Guid CreateNewFile(Dictionary<string, string> entries);

        NetworkFile UpdateFile(Guid fileId, IDictionary<string, string> entries);

        void SendDirectMessage(Guid target, byte[] data);
    }
}
