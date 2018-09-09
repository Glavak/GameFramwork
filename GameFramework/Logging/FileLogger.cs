using System.IO;

namespace GameFramework.Logging
{
    public class FileLogger : ILogger
    {
        private readonly StreamWriter writer;

        public FileLogger(string filename)
        {
            writer = new StreamWriter(filename);
        }

        public void Info(string format, params object[] args)
        {
            writer.WriteLine("[INFO] " + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            writer.WriteLine("[WARN] " + format, args);
        }

        public void Error(string format, params object[] args)
        {
            writer.WriteLine("[ERROR] " + format, args);
        }
    }
}
