using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameFrameworkTests
{
    [TestClass]
    public class NetworkRelayTests
    {
        private NetworkRelay<LocalNetworkConnection, int> relayA;
        private NetworkRelay<LocalNetworkConnection, int> relayB;
        private NetworkRelay<LocalNetworkConnection, int> relayC;

        private LocalNetworkConnectionFactory factoryA;
        private LocalNetworkConnectionFactory factoryB;
        private LocalNetworkConnectionFactory factoryC;

        [TestInitialize]
        public void SetUp()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            factoryA = hub.CreateNodeFactory();
            factoryB = hub.CreateNodeFactory();
            factoryC = hub.CreateNodeFactory();

            relayA = new NetworkRelay<LocalNetworkConnection, int>(factoryA);
            relayB = new NetworkRelay<LocalNetworkConnection, int>(factoryB);
            relayC = new NetworkRelay<LocalNetworkConnection, int>(factoryC);
        }

        [TestCleanup]
        public void Cleanup()
        {
            relayA?.Dispose();
            relayB?.Dispose();
            relayC?.Dispose();
        }

        [TestMethod]
        public async Task TestConnects()
        {
            await relayA.ConnectToNodeAsync(1);

            LocalNetworkConnectionHub.WaitForSettle();

            Assert.AreEqual(1, relayA.GetConnectedClientsCount());
            Assert.AreEqual(1, relayB.GetConnectedClientsCount());
        }

        [TestMethod]
        public async Task TestConnectsToNeighbours()
        {
            await relayA.ConnectToNodeAsync(1);
            await relayA.ConnectToNodeAsync(2);

            await Task.Delay(100);

            Assert.AreEqual(2, relayA.GetConnectedClientsCount());
            Assert.AreEqual(2, relayB.GetConnectedClientsCount());
            Assert.AreEqual(2, relayC.GetConnectedClientsCount());
        }

        [TestMethod]
        public async Task TestConnectionDropped()
        {
            await relayA.ConnectToNodeAsync(1);
            await relayA.ConnectToNodeAsync(2);

            await Task.Delay(100);

            factoryB.Dispose();

            await Task.Delay(100);

            Assert.AreEqual(1, relayA.GetConnectedClientsCount());
            Assert.AreEqual(1, relayC.GetConnectedClientsCount());
        }
    }
}
