using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFramework
{
    [Serializable]
    public class GetFileNetworkMessage : INetworkMessage
    {
        public Guid From { get; }

        public Guid FileId { get; }

        public GetFileNetworkMessage(Guid from, Guid fileId)
        {
            From = from;
            FileId = fileId;
        }
    }
}
