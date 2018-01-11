using System;

namespace GameFramework
{
    [Serializable]
    public class GotFileNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public NetworkFile File { get; }

        public GotFileNetworkMessage(Guid from, NetworkFile file)
        {
            From = from;
            File = file;
        }
    }
}
