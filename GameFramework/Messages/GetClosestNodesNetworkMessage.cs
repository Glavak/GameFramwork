using System;

namespace GameFramework
{
    [Serializable]
    public class GetClosestNodesNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public Guid FileId { get; }

        public GetClosestNodesNetworkMessage(Guid from, Guid fileId)
        {
            From = from;
            FileId = fileId;
        }
    }
}
