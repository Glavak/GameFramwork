using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface INetworkRelay : IDisposable
    {
        void Start(IEnumerable<IPEndPoint> startupNodes);

        void Stop();

        NetworkFile GetFile(Guid fileId);
    }
}
