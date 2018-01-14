using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace GameFrameworkTests
{
    [TestClass]
    public class TcpConnectionsTests
    {
        private TcpNetworkConnectionFactory factoryA;
        private TcpNetworkConnectionFactory factoryB;

        [TestInitialize]
        public void SetUp()
        {
            factoryA = new TcpNetworkConnectionFactory(4242);
            factoryB = new TcpNetworkConnectionFactory(4243);
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
            TcpNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            await factoryB.ConnectToAsync(IPAddress.Loopback);

            await Task.Delay(100);

            // Assert:
            Assert.IsNotNull(connectionOnA);
            Assert.IsTrue(IPAddress.IsLoopback(connectionOnA.Address));
        }

        [TestMethod]
        public async Task TestSimpleSendRecieve()
        {
            // Connect B to A:
            TcpNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            TcpNetworkConnection connectionOnB = await factoryB.ConnectToAsync(IPAddress.Loopback);

            await Task.Delay(100);

            // Send message from B to A:
            INetworkMessage recievedOnA = null;
            connectionOnA.OnRecieve += (sender, message) => { recievedOnA = message; };

            INetworkMessage sendFromB = new DirectNetworkMessage(
                DhtUtils.GeneratePlayerId(),
                DhtUtils.GeneratePlayerId(),
                DhtUtils.GeneratePlayerId(),
                new byte[] {42});
            connectionOnB.Send(sendFromB);

            await Task.Delay(100);

            // Assert:
            Assert.IsNotNull(recievedOnA);
            Assert.AreEqual(42, ((DirectNetworkMessage) recievedOnA).Data[0]);
        }

        [TestMethod]
        public async Task TestConnectionDropped()
        {
            // Connect B to A:
            TcpNetworkConnection connectionOnA = null;
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            TcpNetworkConnection connectionOnB = await factoryB.ConnectToAsync(IPAddress.Loopback);

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
