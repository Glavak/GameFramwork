using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface IRelaySaverLoader<TNetworkAddress>
    {
        void Save(IEnumerable<TNetworkAddress> addresses, IEnumerable<NetworkFile> files);

        bool Load(out IEnumerable<TNetworkAddress> addresses, out IEnumerable<NetworkFile> files);
    }
}
