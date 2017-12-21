using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkConnection<TAddress> : IDisposable
    {
        void Send(INetworkMessage message);

        EventHandler<INetworkMessage> OnRecieve { get; set; }

        TAddress Address { get; }
    }
}
