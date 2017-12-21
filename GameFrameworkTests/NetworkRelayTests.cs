using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameFrameworkTests
{

    [TestClass]
    public class NetworkRelayTests
    {
        INetworkRelay relayA = new NetworkRelay<TcpNetworkConnection, IPEndPoint>(new TcpNetworkConnectionFactory(4242));
        INetworkRelay relayB = new NetworkRelay<TcpNetworkConnection, IPEndPoint>(new TcpNetworkConnectionFactory(4243));

        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void TestHelloPacketConnects()
        {
            
        }
    }
}
