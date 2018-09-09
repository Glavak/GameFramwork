using System;
using System.Collections.Generic;

namespace GameFramework
{
    [Serializable]
    public class GetFileNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public Guid FileId { get; }

        /// <summary>
        /// Nodes that have been checked not to have this file
        /// </summary>
        public HashSet<Guid> VisitedNodes { get; }

        public GetFileNetworkMessage(Guid from, Guid fileId, HashSet<Guid> visitedNodes)
        {
            From = from;
            FileId = fileId;
            VisitedNodes = visitedNodes;
        }
    }
}
