using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    public class Contact
    {
        public Guid Id { get; set; }

        public INetworkConnection NetworkConnection { get; set; }
    }
}
