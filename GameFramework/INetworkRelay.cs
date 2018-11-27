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

        /// <summary>
        /// Tries to get cached version of file
        /// </summary>
        /// <param name="returnEvenExpired">Should local file be return, even if it's too old</param>
        /// <param name="queueIfNotFound">Should attempt to get this file be made in case local file not found, for this function 
        /// to return it next time called</param>
        /// <returns>Found file, or null, if none found</returns>
        NetworkFile GetFileLocalOrNull(Guid fileId, bool returnEvenExpired = false, bool queueIfNotFound = false);

        void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved);

        Guid CreateNewFile(Dictionary<string, string> entries);

        void CreateMatchmakingFile(Guid fileId);

        void CreateLeaderboardsFile(Guid fileId);
        
        NetworkFile UpdateFile(Guid fileId, IDictionary<string, string> entries);

        void SendDirectMessage(Guid target, byte[] data);
    }
}
