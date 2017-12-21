using GameFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameFrameworkTests
{
    [TestClass]
    public class TcpConnectionsTests
    {
        TcpNetworkConnectionFactory factoryA = new TcpNetworkConnectionFactory(4242);
        TcpNetworkConnectionFactory factoryB = new TcpNetworkConnectionFactory(4243);

        [TestMethod]
        public async Task TestConnect()
        {
            // Connect B to A:
            TcpNetworkConnection connectionOnA = null;
            factoryA.StartListening();
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            TcpNetworkConnection connectionOnB = await factoryB.ConnectToAsync(new IPEndPoint(IPAddress.Loopback, 4242));

            await Task.Delay(100);

            Assert.IsNotNull(connectionOnA);
            Assert.IsTrue(IPAddress.IsLoopback(connectionOnA.Address.Address));
        }

        [TestMethod]
        public async Task TestSimpleSendRecieve()
        {
            // Connect B to A:
            TcpNetworkConnection connectionOnA = null;
            factoryA.StartListening();
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            TcpNetworkConnection connectionOnB = await factoryB.ConnectToAsync(new IPEndPoint(IPAddress.Loopback, 4242));

            await Task.Delay(100);

            Assert.IsNotNull(connectionOnA, "OnClientConnected callback haven't called");

            INetworkMessage recievedOnA = null;
            connectionOnA.OnRecieve += (sender, message) => { recievedOnA = message; };

            INetworkMessage sendFromB = new DirectNetworkMessage(DhtUtils.GeneratePlayerId(), DhtUtils.GeneratePlayerId(), new byte[] { 42 });
            connectionOnB.Send(sendFromB);

            await Task.Delay(100);

            Assert.IsNotNull(recievedOnA);
            Assert.AreEqual(42, ((DirectNetworkMessage)recievedOnA).Data[0]);
        }
    }
}
