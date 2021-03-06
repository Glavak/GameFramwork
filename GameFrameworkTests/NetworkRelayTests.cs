﻿using System;
using System.Collections.Generic;
using GameFramework;
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

        private readonly Guid matchmakingFileId = DhtUtils.GenerateFileId();

        [TestInitialize]
        public void SetUp()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            factory0 = hub.CreateNodeFactory();
            factory1 = hub.CreateNodeFactory();
            factory2 = hub.CreateNodeFactory();

            relay0 = new NetworkRelay<LocalNetworkConnection, int>(factory0, matchmakingFileId);
            relay1 = new NetworkRelay<LocalNetworkConnection, int>(factory1, matchmakingFileId);
            relay2 = new NetworkRelay<LocalNetworkConnection, int>(factory2, matchmakingFileId);
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
        public async Task TestConnectsToNeighbours1()
        {
            await relay0.ConnectToNodeAsync(1);
            await relay0.ConnectToNodeAsync(2);

            await Task.Delay(100);

            Assert.AreEqual(2, relay0.GetConnectedClientsCount());
            Assert.AreEqual(2, relay1.GetConnectedClientsCount());
            Assert.AreEqual(2, relay2.GetConnectedClientsCount());
        }

        [TestMethod]
        public async Task TestConnectsToNeighbours2()
        {
            await relay1.ConnectToNodeAsync(2);
            await relay0.ConnectToNodeAsync(1);

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

        [TestMethod]
        public async Task TestDirectMessage()
        {
            await relay0.ConnectToNodeAsync(1);

            await Task.Delay(100);

            byte[] recieved = null;
            Guid sender = Guid.Empty;
            relay1.OnDirectMessage += (s, d) =>
            {
                sender = (Guid)s;
                recieved = d;
            };

            relay0.SendDirectMessage(relay1.OwnId, new[] { (byte)42 });

            await Task.Delay(100);

            Assert.IsNotNull(recieved);
            CollectionAssert.AreEqual(new[] { (byte)42 }, recieved);
            Assert.AreEqual(relay0.OwnId, sender);
        }

        [TestMethod]
        public async Task TestDirectMessageForwarded()
        {
            // So, that 1 and 2 had no way to connect
            factory1.NatSimulation = true;
            factory2.NatSimulation = true;

            await relay1.ConnectToNodeAsync(0);
            await relay2.ConnectToNodeAsync(0);

            await Task.Delay(100);

            byte[] recieved = null;
            Guid sender = Guid.Empty;
            relay2.OnDirectMessage += (s, d) =>
            {
                sender = (Guid)s;
                recieved = d;
            };

            relay1.SendDirectMessage(relay2.OwnId, new[] { (byte)42 });

            await Task.Delay(100);

            Assert.IsNotNull(recieved);
            CollectionAssert.AreEqual(new[] { (byte)42 }, recieved);
            Assert.AreEqual(relay1.OwnId, sender);
        }

        [TestMethod]
        public async Task TestGetOwnFile()
        {
            await relay0.ConnectToNodeAsync(1);

            await Task.Delay(100);

            // Get file from node 0 on node 1:
            NetworkFile file = null;
            relay1.GetFile(relay0.OwnId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(relay0.OwnId, file.Id);
            Assert.AreEqual(FileType.PlayerData, file.FileType);
        }

        [TestMethod]
        public async Task TestGetOwnFileForwarded()
        {
            // So, that 1 and 2 had no way to connect
            factory1.NatSimulation = true;
            factory2.NatSimulation = true;

            await relay1.ConnectToNodeAsync(0);
            await relay2.ConnectToNodeAsync(0);

            await Task.Delay(100);

            // Get file from node 2 on node 1:
            NetworkFile file = null;
            relay1.GetFile(relay2.OwnId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(relay2.OwnId, file.Id);
            Assert.AreEqual(FileType.PlayerData, file.FileType);
        }

        [TestMethod]
        public async Task TestFileCaching()
        {
            // Set nickname
            relay0.UpdateFile(relay0.OwnId, new Dictionary<string, string> { { "nickname", "old :c" } });
            await relay0.ConnectToNodeAsync(1);

            await Task.Delay(100);

            // Get PlayerData file from node 0 on node 1:
            NetworkFile file = null;
            relay1.GetFile(relay0.OwnId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual("old :c", file.Entries["nickname"]);

            // Modify PlayerData file at node 0:
            relay0.UpdateFile(relay0.OwnId, new Dictionary<string, string>{{"nickname", "new!"}});

            // Get PlayerData file again (should be taken from cache):
            file = null;
            relay1.GetFile(relay0.OwnId, (s, f) => file = f);

            await Task.Delay(100);
            
            Assert.AreEqual("old :c", file.Entries["nickname"]);
        }

        [TestMethod]
        public async Task TestGetOtherFile()
        {
            // Create file:
            var entries = new Dictionary<string, string> { { "field", "value" } };
            Guid fileId = relay0.CreateNewFile(entries);

            // Connect nodes:
            await relay1.ConnectToNodeAsync(0);

            await Task.Delay(100);

            // Get file from node 0 on node 1:
            NetworkFile file = null;
            relay1.GetFile(fileId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(fileId, file.Id);
            Assert.AreEqual(FileType.Custom, file.FileType);
            Assert.AreEqual("value", file.Entries["field"]);
        }

        [TestMethod]
        public async Task TestGetUpdatedFile()
        {
            // Create file:
            var entries = new Dictionary<string, string> { { "field", "value" } };
            Guid fileId = relay0.CreateNewFile(entries);

            // Connect nodes:
            await relay1.ConnectToNodeAsync(0);

            await Task.Delay(100);

            // Get file from node 0 on node 1:
            NetworkFile file = null;
            relay1.GetFile(fileId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(fileId, file.Id);
            Assert.AreEqual("value", file.Entries["field"]);

            // Update file on node 0:
            entries = new Dictionary<string, string> { { "field", "new value" } };
            relay0.UpdateFile(fileId, entries);

            // Get file again:
            file = null;
            relay1.GetFile(fileId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(fileId, file.Id);
            Assert.AreEqual("new value", file.Entries["field"]);
        }

        [TestMethod]
        public async Task TestGetNotExistingFile()
        {
            await relay1.ConnectToNodeAsync(0);
            await relay1.ConnectToNodeAsync(2);

            await Task.Delay(100);

            // Get not existing file on node 1:
            NetworkFile file = null;
            bool callbackCalled = false;
            relay1.GetFile(DhtUtils.GenerateFileId(), (s, f) =>
            {
                file = f;
                callbackCalled = true;
            });

            await Task.Delay(100);

            Assert.IsNull(file);
            Assert.IsTrue(callbackCalled);
        }

        [TestMethod]
        public async Task TestGetOfflinePlayerFile()
        {
            await relay0.ConnectToNodeAsync(1);
            await relay0.ConnectToNodeAsync(2);

            await Task.Delay(100);

            // Get file from node 0 on node 1:
            relay1.GetFile(relay0.OwnId, (s, f) => { Console.WriteLine(); });

            await Task.Delay(100);

            // Disable node 0:
            relay0.Dispose();

            await Task.Delay(100);

            // Get file from node 0 on node 2:
            NetworkFile file = null;
            relay2.GetFile(relay0.OwnId, (s, f) => file = f);

            await Task.Delay(100);

            Assert.AreEqual(relay0.OwnId, file.Id);
            Assert.AreEqual(FileType.PlayerData, file.FileType);
        }

        [TestMethod]
        public async Task TestGetMergedMatchmaking()
        {
            await relay0.ConnectToNodeAsync(1);
            await relay0.ConnectToNodeAsync(2);
            
            var entries = new Dictionary<string, string> { { "field1", "value1" } };
            relay0.UpdateFile(matchmakingFileId, entries);

            entries = new Dictionary<string, string> { { "field2", "value2" } };
            relay1.UpdateFile(matchmakingFileId, entries);

            await Task.Delay(5000); // Wait for cached files to invalidate

            // Get file on node 2, it should ask other node, and it should ask remaning node for it and merge:
            NetworkFile file = null;
            relay2.GetFile(matchmakingFileId, (s, f) => { file = f; });

            await Task.Delay(100);

            Assert.AreEqual(FileType.Matchmaking, file.FileType);
            Assert.AreEqual("value1", file.Entries["field1"]);
            Assert.AreEqual("value2", file.Entries["field2"]);
        }
    }
}
