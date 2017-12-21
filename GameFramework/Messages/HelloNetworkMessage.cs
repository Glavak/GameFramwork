using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
