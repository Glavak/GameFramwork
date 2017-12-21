using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    /// <summary>
    /// Message that will be delivered directly to the destination, and would not affect the state of DHT network
    /// </summary>
    [Serializable]
    public class DirectNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public Guid Destination { get; }

        public byte[] Data { get; }

        public DirectNetworkMessage(Guid from, Guid destination, byte[] data)
        {
            From = from;
            Destination = destination;
            Data = data;
        }
    }
}
