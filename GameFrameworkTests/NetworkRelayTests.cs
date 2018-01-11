using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace GameFrameworkTests
{

    [TestClass]
    public class NetworkRelayTests
    {
        private NetworkRelay<LocalNetworkConnection, int> relayA;
        private NetworkRelay<LocalNetworkConnection, int> relayB;

        [TestInitialize]
        public void SetUp()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            var factoryA = hub.CreateNodeFactory();
            var factoryB = hub.CreateNodeFactory();

            relayA = new NetworkRelay<LocalNetworkConnection, int>(factoryA);
            relayB = new NetworkRelay<LocalNetworkConnection, int>(factoryB);
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
            await relayA.ConnectToNodeAsync(1);

            await Task.Delay(100);

            Assert.AreEqual(1, relayA.GetConnectedClientsCount());
            Assert.AreEqual(1, relayB.GetConnectedClientsCount());
        }
    }
}
