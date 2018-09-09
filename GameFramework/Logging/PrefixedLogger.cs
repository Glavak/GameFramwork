namespace GameFramework.Logging
{
    public class PrefixedLogger : ILogger
    {
        private readonly ILogger parent;
        private readonly string prefix;

        public PrefixedLogger(ILogger parent, string prefix)
        {
            this.parent = parent;
            this.prefix = prefix;
        }

        public void Info(string format, params object[] args)
        {
            parent.Info(prefix + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            parent.Warn(prefix + format, args);
        }

        public void Error(string format, params object[] args)
        {
            parent.Error(prefix + format, args);
        }
    }
}
