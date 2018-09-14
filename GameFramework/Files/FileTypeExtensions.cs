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
                    return TimeSpan.FromSeconds(10);
                case FileType.Leaderboard:
                    return TimeSpan.FromMinutes(1);
                case FileType.Custom:
                    return TimeSpan.Zero;
                case FileType.Matchmaking:
                    return TimeSpan.FromSeconds(5);
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }
    }
}
