using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkConnectionFactory<TConection, TAddress>
        where TConection : INetworkConnection<TAddress>
    {
        EventHandler<TConection> OnClientConnected { get; set; }

        void StartListening();

        void StopListening();

        Task<TConection> ConnectToAsync(TAddress address);
    }
}
