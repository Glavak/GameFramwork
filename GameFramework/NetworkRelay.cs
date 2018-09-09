using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class NetworkRelay<TNetworkConnection, TNetworkAddress> :
        INetworkRelay<TNetworkConnection, TNetworkAddress>
        where TNetworkConnection : INetworkConnection<TNetworkAddress>
    {
        public EventHandler<byte[]> OnDirectMessage { get; set; }

        public List<Contact<TNetworkAddress>>[] KBuckets { get; set; }
        public Guid OwnId { get; }

        private readonly INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory;

        private bool disposed;

        private readonly Dictionary<Guid, NetworkFile> files = new Dictionary<Guid, NetworkFile>();

        private readonly Dictionary<Guid, FileSearchRequest> fileSearchRequests =
            new Dictionary<Guid, FileSearchRequest>();

        public NetworkRelay(INetworkConnectionFactory<TNetworkConnection, TNetworkAddress> connectionFactory)
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
            if (files.TryGetValue(fileId, out NetworkFile file))
            {
                if (file.Owner != OwnId &&
                    file.RecievedFromOrigin + file.FileType.GetCacheLifetime() < DateTime.Now)
                {
                    // Expired
                    // TODO: if file not found, still return expired one
                    files.Remove(fileId);
                }
                else
                {
                    if (file.Owner == OwnId)
                    {
                        file = file.CopyRefreshingOriginated();
                    }

                    onFileRecieved?.Invoke(this, file);
                    return;
                }
            }

            var searchRequest = new FileSearchRequest(fileId, onFileRecieved);
            fileSearchRequests.Add(fileId, searchRequest);

            var contact = GetClosestContactExcept(fileId);

            GetFileNetworkMessage message = new GetFileNetworkMessage(OwnId, fileId);
            contact.NetworkConnection.Send(message);

            searchRequest.NodesWithoutFile.Add(contact.Id);
        }

        public Guid CreateNewFile(Dictionary<string, string> entries)
        {
            var file = new NetworkFile(DhtUtils.GenerateFileId(), OwnId, entries.ToImmutableDictionary());
            files.Add(file.Id, file);
            return file.Id;
        }

        public void UpdateFile(Guid fileId, Dictionary<string, string> entries)
        {
            if (!files.TryGetValue(fileId, out NetworkFile file))
            {
                throw new FrameworkException("No such file exists");
            }

            if (file.Owner != Guid.Empty &&
                file.Owner != OwnId)
            {
                throw new FrameworkException("File modification not allowed");
            }

            files[file.Id] = new NetworkFile(file.Id, file.Owner,
                DateTime.Now, file.FileType,
                entries.ToImmutableDictionary());
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
                    if (files.TryGetValue(message.FileId, out NetworkFile file))
                    {
                        replyMessage = new GotFileNetworkMessage(OwnId, file);
                    }
                    else
                    {
                        GetFile(message.FileId, (s, f) =>
                        {
                            var reply = new GotFileNetworkMessage(OwnId, f);
                            senderConnection.Send(reply);
                        });
                    }

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
                            ConnectToNodeAsync(node.Value).Wait();
                        }
                    }

                    foreach (var searchRequest in fileSearchRequests.Values)
                    {
                        var closestContact =
                            GetClosestContactExcept(searchRequest.FileId, searchRequest.NodesWithoutFile);

                        if (closestContact == null)
                        {
                            //TODO: no file handling
                        }
                        else
                        {
                            GetFileNetworkMessage getFileMessage =
                                new GetFileNetworkMessage(OwnId, searchRequest.FileId);
                            closestContact.NetworkConnection.Send(getFileMessage);
                            searchRequest.NodesWithoutFile.Add(closestContact.Id);
                        }
                    }

                    break;

                case GotFileNetworkMessage message:
                    NetworkFile recievedFile = message.File;

                    if (fileSearchRequests.TryGetValue(recievedFile.Id, out FileSearchRequest request))
                    {
                        contact.LastUseful = DateTime.Now;

                        files.Add(recievedFile.Id, recievedFile); // Cache it, in case it's needed later
                        request.OnFound?.Invoke(this, recievedFile);
                        fileSearchRequests.Remove(recievedFile.Id);
                    }

                    break;

                case StoreFileNetworkMessage message:
                    NetworkFile fileToStore = message.File;

                    files[fileToStore.Id] = fileToStore;

                    break;

                case DirectNetworkMessage message:
                    if (message.Destination == OwnId)
                    {
                        OnDirectMessage?.Invoke(message.Origin, message.Data);
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
