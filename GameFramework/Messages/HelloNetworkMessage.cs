using System;

namespace GameFramework
{
    [Serializable]
    public class HelloNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public HelloNetworkMessage(Guid from)
        {
            From = from;
        }
    }
}
