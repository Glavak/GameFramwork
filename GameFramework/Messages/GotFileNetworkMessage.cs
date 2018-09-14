using System;

namespace GameFramework
{
    [Serializable]
    public class GotFileNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public Guid FileId { get; }

        public NetworkFile File { get; }

        public GotFileNetworkMessage(Guid from, Guid fileId, NetworkFile file)
        {
            From = from;
            FileId = fileId;
            File = file;
        }
    }
}
