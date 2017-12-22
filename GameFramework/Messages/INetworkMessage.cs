using System;

namespace GameFramework
{
    public interface INetworkMessage
    {
        Guid From { get; }
    }
}
