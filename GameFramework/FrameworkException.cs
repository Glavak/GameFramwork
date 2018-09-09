using System;

namespace GameFramework
{
    public class FrameworkException : Exception
    {
        public FrameworkException()
        {
        }

        public FrameworkException(string message) : base(message)
        {
        }

        public FrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
