using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace GameFrameworkTests
{

    [TestClass]
    public class NetworkRelayTests
    {
        private NetworkRelay<TcpNetworkConnection, IPEndPoint> relayA;
        private NetworkRelay<TcpNetworkConnection, IPEndPoint> relayB;

        [TestInitialize]
        public void SetUp()
        {
            relayA = new NetworkRelay<TcpNetworkConnection, IPEndPoint>(new TcpNetworkConnectionFactory(4242));
            relayB = new NetworkRelay<TcpNetworkConnection, IPEndPoint>(new TcpNetworkConnectionFactory(4243));
        }

        [TestCleanup]
        public void Cleanup()
        {
            relayA?.Dispose();
            relayB?.Dispose();
        }

        [TestMethod]
        public async Task TestHelloPacketConnects()
        {
            await relayA.ConnectToNodeAsync(new IPEndPoint(IPAddress.Loopback, 4243));

            await Task.Delay(100);

            Assert.AreEqual(1, relayA.GetConnectedClientsCount());
            Assert.AreEqual(1, relayB.GetConnectedClientsCount());

            Assert.AreEqual(0, relayA.NotHelloedConnections.Count);
            Assert.AreEqual(0, relayB.NotHelloedConnections.Count);
        }
    }
}
