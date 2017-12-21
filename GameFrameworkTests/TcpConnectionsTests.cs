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
        TcpNetworkConnectionFactory factoryA = new TcpNetworkConnectionFactory();
        TcpNetworkConnectionFactory factoryB = new TcpNetworkConnectionFactory();

        [TestMethod]
        public async Task TestSimpleSendRecieveAsync()
        {
            // Connect everything up:
            TcpNetworkConnection connectionOnA = null;
            factoryA.StartListening(4242);
            factoryA.OnClientConnected += (sender, connectoin) => { connectionOnA = connectoin; };

            TcpNetworkConnection connectionOnB = await factoryB.ConnectToAsync(IPAddress.Loopback, 4242);

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
