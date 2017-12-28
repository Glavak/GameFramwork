using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class LocalNetworkConnectionHub
    {
        public List<LocalNetworkConnectionFactory> NodeFactories { get; }

        public LocalNetworkConnectionHub()
        {
            NodeFactories = new List<LocalNetworkConnectionFactory>();
        }

        public LocalNetworkConnectionFactory CreateNodeFactory()
        {
            var factory = new LocalNetworkConnectionFactory(NodeFactories.Count, this);

            NodeFactories.Add(factory);

            return factory;
        }
    }
}
