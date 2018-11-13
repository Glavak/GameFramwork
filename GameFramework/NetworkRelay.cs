using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GameFramework.Files;
using GameFramework.Logging;

namespace GameFramework
{
    public sealed class NetworkRelay<TNetworkConnection, TNetworkAddress> : INetworkRelay<TNetworkAddress>
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        public EventHandler<byte[]> OnDirectMessage
        {
            get => onDirectMessage;
            set
            {
                onDirectMessage = value;
                if (onDirectMessage != null && pendingDirectMessages.Count > 0)
                {
                    foreach (var message in pendingDirectMessages)
                    {
                        onDirectMessage.Invoke(message.Origin, message.Data);
                    }
                }
            }
        }

        private EventHandler<byte[]> onDirectMessage;
        private readonly HashSet<DirectNetworkMessage> pendingDirectMessages = new HashSet<DirectNetworkMessage>();

        private ILogger Logger;

        public List<Contact<TNetworkAddress>>[] KBuckets { get; set; }
        public Guid OwnId { get; }

        private readonly INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory;

        private bool disposed;

        private readonly Dictionary<Guid, NetworkFile> files = new Dictionary<Guid, NetworkFile>();

        private readonly Dictionary<Guid, FileSearchRequest> fileSearchRequests =
            new Dictionary<Guid, FileSearchRequest>();

        public NetworkRelay(INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory,
            Guid matchmakingFileId)
        {
            KBuckets = new List<Contact<TNetworkAddress>>[128];
            for (int i = 0; i < KBuckets.Length; i++)
            {
                KBuckets[i] = new List<Contact<TNetworkAddress>>();
            }

            this.connectionFactory = connectionFactory;
            this.connectionFactory.OnClientConnected += OnClientConnected;

            OwnId = DhtUtils.GeneratePlayerId();

            var builder = ImmutableDictionary.CreateBuilder<string, string>();
            builder.Add("nickname", "client");
            var ownFile = new NetworkFile(OwnId, OwnId,
                DateTime.Now, FileType.PlayerData,
                builder.ToImmutable());
            files.Add(OwnId, ownFile);

            Logger = new PrefixedLogger(new ConsoleLogger(), OwnId + " | ");

            files.Add(matchmakingFileId,
                new NetworkFile(matchmakingFileId, Guid.Empty, DateTime.MinValue, FileType.Matchmaking,
                    ImmutableDictionary<string, string>.Empty));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                connectionFactory.Dispose();
            }

            disposed = true;
        }

        public async Task<bool> ConnectToNodeAsync(TNetworkAddress address)
        {
            if (IsAddressAlreadyConnected(address)) return true;

            try
            {
                TNetworkConnection connection = await connectionFactory.ConnectToAsync(address, OnMessageRecieved);
                connection.OnConnectionDropped += OnConnectionDropped;

                HelloNetworkMessage message = new HelloNetworkMessage(OwnId);
                connection.Send(message);

                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved)
        {
            GetFile(fileId, onFileRecieved, null);
        }

        private void GetFile(Guid fileId, EventHandler<NetworkFile> onFileRecieved, HashSet<Guid> nodesWithoutFile)
        {
            Logger.Info("Trying to get file {0}", fileId);
            if (files.TryGetValue(fileId, out NetworkFile file))
            {
                if (file.Owner != OwnId &&
                    file.RecievedFromOrigin + file.FileType.GetCacheLifetime() < DateTime.Now)
                {
                    // Expired
                    Logger.Info("File expired, trying to find newer");
                }
                else
                {
                    if (file.Owner == OwnId)
                    {
                        file = file.CopyRefreshingOriginated();
                    }

                    Logger.Info("File found, invoking callback");
                    onFileRecieved?.Invoke(this, file);
                    return;
                }
            }
            else Logger.Info("File not found, looking for contacts");

            if (!fileSearchRequests.TryGetValue(fileId, out FileSearchRequest searchRequest))
            {
                searchRequest = new FileSearchRequest(fileId, onFileRecieved);
                searchRequest.NodesWithoutFile.Add(OwnId);
            }
            else
            {
                searchRequest.OnFound += onFileRecieved;
            }

            if (nodesWithoutFile != null) searchRequest.NodesWithoutFile.UnionWith(nodesWithoutFile);

            var contact = GetClosestContactExcept(fileId, searchRequest.NodesWithoutFile);
            if (contact == null)
            {
                // No more contacts that can have this file, return expired one, or null
                Logger.Info("No suitable contacts found");
                onFileRecieved?.Invoke(this, file);
            }
            else
            {
                Logger.Info("Asking {0} for file", contact.Id);
                GetFileNetworkMessage message =
                    new GetFileNetworkMessage(OwnId, fileId, searchRequest.NodesWithoutFile);
                contact.NetworkConnection.Send(message);

                if (!fileSearchRequests.ContainsKey(fileId)) fileSearchRequests.Add(fileId, searchRequest);
            }
        }

        public Guid CreateNewFile(Dictionary<string, string> entries)
        {
            var file = new NetworkFile(DhtUtils.GenerateFileId(), OwnId, entries.ToImmutableDictionary());
            files.Add(file.Id, file);
            return file.Id;
        }

        public NetworkFile UpdateFile(Guid fileId, IDictionary<string, string> entries)
        {
            if (!files.TryGetValue(fileId, out NetworkFile file))
            {
                throw new FrameworkException("No such file exists");
            }

            /*if (file.Owner != Guid.Empty &&
                file.Owner != OwnId)
            {
                throw new FrameworkException("File modification not allowed");
            }*/

            var updatedFile = new NetworkFile(file.Id, file.Owner,
                DateTime.Now, file.FileType,
                entries.ToImmutableDictionary());
            files[file.Id] = updatedFile;

            Logger.Info($"File {fileId} updated");

            return updatedFile;
        }

        public void SendDirectMessage(Guid target, byte[] data)
        {
            var closestNode = GetClosestContactExcept(target);
            var message = new DirectNetworkMessage(OwnId, OwnId, target, data);
            closestNode.NetworkConnection.Send(message);
        }

        public int GetConnectedClientsCount()
        {
            return KBuckets.Sum(kBucket => kBucket.Count);
        }

        private void OnClientConnected(object sender, TNetworkConnection clientConnection)
        {
            clientConnection.OnRecieve += OnMessageRecieved;
            clientConnection.OnConnectionDropped += OnConnectionDropped;

            HelloNetworkMessage message = new HelloNetworkMessage(OwnId);
            clientConnection.Send(message);
        }

        private void OnConnectionDropped(object sender, EventArgs eventArgs)
        {
            TNetworkConnection senderConnection = (TNetworkConnection) sender;

            foreach (var kBucket in KBuckets)
            {
                if (kBucket.RemoveAll(c => c.NetworkConnection.Address.Equals(senderConnection.Address)) > 0)
                {
                    break;
                }
            }
        }

        private void OnMessageRecieved(object sender, INetworkMessage e)
        {
            Logger.Info("message recieved");

            TNetworkConnection senderConnection = (TNetworkConnection) sender;

            INetworkMessage replyMessage = null;
            Dictionary<Guid, TNetworkAddress> closestContacts;
            Contact<TNetworkAddress> contact = GetContact(e.From);
            // TODO: remake this switch
            switch (e)
            {
                case HelloNetworkMessage message:
                    int bucketIndex = DhtUtils.DistanceExp(OwnId, message.From);

                    if (contact == null)
                    {
                        contact = new Contact<TNetworkAddress>
                        {
                            Id = message.From,
                            NetworkConnection = senderConnection
                        };
                        KBuckets[bucketIndex].Add(contact);
                        CheckKBucket(bucketIndex);
                    }

                    closestContacts = GetClosestContacts(message.From, 10);
                    replyMessage = new NodeListNetworkMessage<TNetworkAddress>(OwnId, closestContacts);

                    break;

                case GetFileNetworkMessage message:
                    Logger.Info("GetFileNetworkMessage for file {0} recieved", message.FileId);

                    GetFile(message.FileId, (s, f) =>
                    {
                        var reply = new GotFileNetworkMessage(OwnId, message.FileId, f);
                        Logger.Info("Forwarding file {0} to requester {1}", message.FileId, message.From);
                        contact.NetworkConnection.Send(reply);
                    }, message.VisitedNodes);
                    break;

                case GetClosestNodesNetworkMessage message:
                    closestContacts = GetClosestContacts(message.FileId, 10);
                    replyMessage = new NodeListNetworkMessage<TNetworkAddress>(OwnId, closestContacts);
                    break;

                case NodeListNetworkMessage<TNetworkAddress> message:
                    contact.LastUseful = DateTime.Now;

                    foreach (var node in message.Nodes)
                    {
                        if (node.Key != OwnId)
                        {
                            // Can block here, all our other messages will be queued until we finish
                            ConnectToNodeAsync(node.Value).Wait();
                        }
                    }

                    contact.LastUseful = DateTime.Now;

                    break;

                case GotFileNetworkMessage message:
                    Logger.Info("GotFileNetworkMessage for file {0} recieved", message.FileId);

                    NetworkFile recievedFile = message.File;
                    bool hadStored = files.TryGetValue(message.FileId, out NetworkFile storedFile);

                    NetworkFile mergedFile = FilesMerger.MergeFiles(recievedFile, storedFile);

                    if (fileSearchRequests.TryGetValue(message.FileId, out FileSearchRequest request))
                    {
                        if (!hadStored)
                        {
                            files.Add(message.FileId, mergedFile); // Cache it, in case it's needed later
                        }
                        else
                        {
                            files[message.FileId] = mergedFile; // Replace cache with merged version
                        }

                        Logger.Info(mergedFile != null ? "File found" : "No such file found");
                        request.OnFound?.Invoke(this, mergedFile);
                        fileSearchRequests.Remove(message.FileId);

                        contact.LastUseful = DateTime.Now;
                    }

                    break;

                case StoreFileNetworkMessage message:
                    NetworkFile fileToStore = message.File;

                    files[fileToStore.Id] = fileToStore;

                    break;

                case DirectNetworkMessage message:
                    if (message.Destination == OwnId)
                    {
                        if (OnDirectMessage == null)
                        {
                            pendingDirectMessages.Add(message);
                            Logger.Warn(
                                "Direct message while no handler attached, storing it until handler availiable");
                        }
                        else
                        {
                            OnDirectMessage.Invoke(message.Origin, message.Data);
                        }

                        contact.LastUseful = DateTime.Now;
                    }
                    else
                    {
                        var forwardThrough = GetClosestContactExcept(message.Destination, new List<Guid>
                        {
                            message.From,
                            message.Origin // HACK: need more sophisticated routing algorithm
                        });
                        INetworkMessage forwardMessage =
                            new DirectNetworkMessage(OwnId, message.Origin, message.Destination, message.Data);

                        forwardThrough.NetworkConnection.Send(forwardMessage);
                        forwardThrough.LastUseful = DateTime.Now;
                    }

                    break;
            }

            if (replyMessage != null) senderConnection.Send(replyMessage);
        }

        private bool IsAddressAlreadyConnected(TNetworkAddress address)
        {
            return KBuckets.Any(b => b.Any(c => c.NetworkConnection.Address.Equals(address)));
        }

        private Contact<TNetworkAddress> GetContact(Guid id)
        {
            int bucketIndex = DhtUtils.DistanceExp(OwnId, id);

            return KBuckets[bucketIndex].FirstOrDefault(c => c.Id == id);
        }

        private void CheckKBucket(int index)
        {
            var kBucket = KBuckets[index];

            if (kBucket.Count > 10)
            {
                kBucket.Sort((a, b) => a.LastUseful.CompareTo(b.LastUseful));
                kBucket.RemoveAt(kBucket.Count - 1);
            }
        }

        private Dictionary<Guid, TNetworkAddress> GetClosestContacts(Guid nodeId, int maxContacts)
        {
            var allContacts = KBuckets.SelectMany(b => b);
            var closestContacts = allContacts.OrderBy(c => DhtUtils.XorDistance(c.Id, nodeId));
            return closestContacts
                .Take(maxContacts)
                .ToDictionary(c => c.Id, c => c.NetworkConnection.Address);
        }

        private Contact<TNetworkAddress> GetClosestContactExcept(Guid id, ICollection<Guid> excludedIds = null)
        {
            var allContacts = KBuckets.SelectMany(b => b);
            var filteredContacts = allContacts;
            if (excludedIds != null)
            {
                filteredContacts = filteredContacts.Where(c => !excludedIds.Contains(c.Id));
            }

            Contact<TNetworkAddress> closestContact = null;
            BigInteger closestDistance = BigInteger.Zero;

            foreach (var contact in filteredContacts)
            {
                var distance = DhtUtils.XorDistance(contact.Id, id);
                if (closestContact == null || distance < closestDistance)
                {
                    closestContact = contact;
                    closestDistance = distance;
                }
            }

            return closestContact;
        }
    }
}
