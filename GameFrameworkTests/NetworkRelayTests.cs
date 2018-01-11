﻿using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameFrameworkTests
{
    [TestClass]
    public class NetworkRelayTests
    {
        private NetworkRelay<LocalNetworkConnection, int> relay0;
        private NetworkRelay<LocalNetworkConnection, int> relay1;
        private NetworkRelay<LocalNetworkConnection, int> relay2;

        private LocalNetworkConnectionFactory factory0;
        private LocalNetworkConnectionFactory factory1;
        private LocalNetworkConnectionFactory factory2;

        [TestInitialize]
        public void SetUp()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            factory0 = hub.CreateNodeFactory();
            factory1 = hub.CreateNodeFactory();
            factory2 = hub.CreateNodeFactory();

            relay0 = new NetworkRelay<LocalNetworkConnection, int>(factory0);
            relay1 = new NetworkRelay<LocalNetworkConnection, int>(factory1);
            relay2 = new NetworkRelay<LocalNetworkConnection, int>(factory2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            relay0?.Dispose();
            relay1?.Dispose();
            relay2?.Dispose();
        }

        [TestMethod]
        public async Task TestConnects()
        {
            await relay0.ConnectToNodeAsync(1);

            await Task.Delay(100);

            Assert.AreEqual(1, relay0.GetConnectedClientsCount());
            Assert.AreEqual(1, relay1.GetConnectedClientsCount());
        }

        [TestMethod]
        public async Task TestConnectsToNeighbours()
        {
            await relay0.ConnectToNodeAsync(1);
            await relay0.ConnectToNodeAsync(2);

            await Task.Delay(100);

            Assert.AreEqual(2, relay0.GetConnectedClientsCount());
            Assert.AreEqual(2, relay1.GetConnectedClientsCount());
            Assert.AreEqual(2, relay2.GetConnectedClientsCount());
        }

        [TestMethod]
        public async Task TestConnectionDropped()
        {
            await relay0.ConnectToNodeAsync(1);
            await relay0.ConnectToNodeAsync(2);

            await Task.Delay(100);

            factory1.Dispose();

            await Task.Delay(100);

            Assert.AreEqual(1, relay0.GetConnectedClientsCount());
            Assert.AreEqual(1, relay2.GetConnectedClientsCount());
        }
    }
}
