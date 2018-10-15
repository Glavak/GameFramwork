using System;
using System.Collections.Immutable;
using System.Linq;

namespace GameFramework.Files
{
    public static class FilesMerger
    {
        public static NetworkFile MergeFiles(NetworkFile a, NetworkFile b)
        {
            if (b == null) return a;
            if (a == null) return b;

            if (a.Id != b.Id) throw new FrameworkException("Merging files with different ids");
            if (a.FileType != b.FileType) return NewerFile(a, b);

            switch (a.FileType)
            {
                case FileType.PlayerData:
                case FileType.Custom:
                    return NewerFile(a, b);
                case FileType.Leaderboard:
                    return NewerFile(a, b);
                    // TODO: merging files
                    break;
                case FileType.Matchmaking:
                    if (a.RecievedFromOrigin > b.RecievedFromOrigin)
                    {
                        return MergeMatchmakingFiles(a, b);
                    }
                    else
                    {
                        return MergeMatchmakingFiles(b, a);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static NetworkFile MergeMatchmakingFiles(NetworkFile highPriority, NetworkFile lowPriority)
        {
            var lowPriorityStripped = lowPriority.Entries
                .Where(kvp => !highPriority.Entries.ContainsKey(kvp.Key));
            var entries = highPriority.Entries
                .Concat(lowPriorityStripped)
                .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new NetworkFile(highPriority.Id, highPriority.Owner,
                highPriority.RecievedFromOrigin, highPriority.FileType,
                entries);
        }

        public static NetworkFile NewerFile(NetworkFile a, NetworkFile b)
        {
            return a.RecievedFromOrigin > b.RecievedFromOrigin ? a : b;
        }
    }
}
