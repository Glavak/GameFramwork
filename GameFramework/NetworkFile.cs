using System;
using System.Collections.Generic;

namespace GameFramework
{
    public abstract class NetworkFile
    {
        public Guid Id { get; }

        public DateTime RecievedFromOrigin { get; }

        private Dictionary<string, string> entries;

        public string GetEntry(string key)
        {
            return entries[key];
        }
    }
}
