using System;
using System.Collections.Generic;

namespace GameFramework
{
    [Serializable]
    public class NodeListNetworkMessage<TNetworkAddress> : INetworkMessage
    {
        public Guid From { get; }

        public List<Contact<TNetworkAddress>> Nodes { get; set; }

        public NodeListNetworkMessage(Guid @from, List<Contact<TNetworkAddress>> nodes)
        {
            From = @from;
            Nodes = nodes;
        }
    }
}
