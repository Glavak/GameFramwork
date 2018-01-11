using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameFrameworkTests
{
    [TestClass]
    public class LocalConnectionsTests
    {
        private LocalNetworkConnectionFactory factoryA;
        private LocalNetworkConnectionFactory factoryB;

        [TestInitialize]
        public void SetUp()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            factoryA = hub.CreateNodeFactory();
            factoryB = hub.CreateNodeFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            factoryA?.Dispose();
            factoryB?.Dispose();
        }
        
        [TestMethod]
        public async Task TestConnect()
        {
            // Connect B to A:
            LocalNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connection) => { connectionOnA = connection; };

            await factoryB.ConnectToAsync(0);

            await Task.Delay(100);

            // Assert:
            Assert.IsNotNull(connectionOnA);
            Assert.AreEqual(1, connectionOnA.Address);
        }

        [TestMethod]
        public async Task TestSimpleSendRecieve()
        {
            // Connect B to A:
            LocalNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            LocalNetworkConnection connectionOnB = await factoryB.ConnectToAsync(0);

            await Task.Delay(100);

            // Send message from B to A:
            INetworkMessage recievedOnA = null;
            connectionOnA.OnRecieve += (sender, message) => { recievedOnA = message; };

            INetworkMessage sendFromB = new DirectNetworkMessage(DhtUtils.GeneratePlayerId(), DhtUtils.GeneratePlayerId(), new byte[] { 42 });
            connectionOnB.Send(sendFromB);

            await Task.Delay(100);

            // Assert:
            Assert.IsNotNull(recievedOnA);
            Assert.AreEqual(42, ((DirectNetworkMessage)recievedOnA).Data[0]);
        }

        [TestMethod]
        public async Task TestConnectionDropped()
        {
            // Connect B to A:
            LocalNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            LocalNetworkConnection connectionOnB = await factoryB.ConnectToAsync(0);

            await Task.Delay(100);

            bool called = false;
            connectionOnA.OnConnectionDropped = (sender, args) => { called = true; };

            // Kill B:
            connectionOnB.Dispose();

            await Task.Delay(100);

            // Assert:
            Assert.IsTrue(called);
        }
    }
}
