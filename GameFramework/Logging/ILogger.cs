namespace GameFramework.Logging
{
    public interface ILogger
    {
        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Error(string format, params object[] args);
    }
}
