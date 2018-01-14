using System;

namespace GameFramework
{
    [Serializable]
    public class StoreFileNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public NetworkFile File { get; }

        public StoreFileNetworkMessage(Guid from, NetworkFile file)
        {
            From = from;
            File = file;
        }
    }
}
