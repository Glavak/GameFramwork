using System;
using System.Collections.Generic;

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

        public Dictionary<string, string> entries = new Dictionary<string, string>();

        public NetworkFile(Guid id)
        {
            Id = id;
        }
    }
}
