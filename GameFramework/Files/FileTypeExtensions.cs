using System;

namespace GameFramework
{
    public static class FileTypeExtensions
    {
        public static TimeSpan GetCacheLifetime(this FileType fileType)
        {
            switch (fileType)
            {
                case FileType.PlayerData:
                    return TimeSpan.FromSeconds(5);
                case FileType.Leaderboard:
                    return TimeSpan.FromSeconds(5);
                case FileType.Custom:
                    return TimeSpan.FromSeconds(1);
                case FileType.Matchmaking:
                    return TimeSpan.FromSeconds(5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }
    }
}
