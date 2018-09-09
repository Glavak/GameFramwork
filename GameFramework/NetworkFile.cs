using System;
using System.Collections.Immutable;

namespace GameFramework
{
    [Serializable]
    public class NetworkFile
    {
        public Guid Id { get; }

        /// <summary>
        /// Last time, at which this version of file was certanly correct and up to date
        /// </summary>
        public DateTime RecievedFromOrigin { get; }

        public ImmutableDictionary<string, string> Entries { get; }

        public NetworkFile(Guid id, ImmutableDictionary<string, string> entries)
        {
            Id = id;
            Entries = entries;
        }
    }
}
