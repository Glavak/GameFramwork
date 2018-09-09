using System;
using System.Collections.Generic;

namespace GameFramework
{
    [Serializable]
    public class NodeListNetworkMessage<TNetworkAddress> : INetworkMessage
    {
        public Guid From { get; }

        public Dictionary<Guid, TNetworkAddress> Nodes { get; }

        public NodeListNetworkMessage(Guid @from, Dictionary<Guid, TNetworkAddress> nodes)
        {
            From = @from;
            Nodes = nodes;
        }
    }
}
