using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class FileSearchRequest
    {
        public Guid FileId { get; }

        public HashSet<Guid> NodesWithoutFile { get; } = new HashSet<Guid>();

        public EventHandler<NetworkFile> OnFound { get; set; }

        public FileSearchRequest(Guid fileId, EventHandler<NetworkFile> onFound)
        {
            FileId = fileId;
            OnFound = onFound;
        }
    }
}
