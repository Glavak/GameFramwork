using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkMessage
    {
        Guid From { get; }
    }
}
