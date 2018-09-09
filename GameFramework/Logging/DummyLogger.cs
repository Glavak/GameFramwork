namespace GameFramework.Logging
{
    public class DummyLogger : ILogger
    {
        public void Info(string format, params object[] args)
        {
        }

        public void Warn(string format, params object[] args)
        {
        }

        public void Error(string format, params object[] args)
        {
        }
    }
}
