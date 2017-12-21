using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class Contact<TNetworkAdress>
    {
        public Guid Id { get; set; }

        public INetworkConnection<TNetworkAdress> NetworkConnection { get; set; }
    }
}
