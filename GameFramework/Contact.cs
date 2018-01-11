using System;

namespace GameFramework
{
    public class Contact<TNetworkAdress>
    {
        public Guid Id { get; set; }

        public INetworkConnection<TNetworkAdress> NetworkConnection { get; set; }

        public DateTime LastUseful = DateTime.Now;
    }
}
