using System;
using System.Collections.Immutable;

namespace GameFramework
{
    [Serializable]
    public class NetworkFile
    {
        public Guid Id { get; }

        public Guid Owner { get; }

        /// <summary>
        /// Last time, at which this version of file was certanly correct and up to date
        /// </summary>
        public DateTime RecievedFromOrigin { get; }

        public FileType FileType { get; }

        public ImmutableDictionary<string, string> Entries { get; }

        public NetworkFile(Guid id, Guid owner, ImmutableDictionary<string, string> entries)
        {
            Id = id;
            Owner = owner;
            FileType = FileType.Custom;
            Entries = entries;
        }

        internal NetworkFile(Guid id, Guid owner,
            DateTime recievedFromOrigin, FileType fileType, 
            ImmutableDictionary<string, string> entries)
        {
            Id = id;
            Owner = owner;
            RecievedFromOrigin = recievedFromOrigin;
            FileType = fileType;
            Entries = entries;
        }
    }
}
