using System;

namespace GameFramework.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string format, params object[] args)
        {
            Console.WriteLine("[INFO] " + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Console.WriteLine("[WARN] " + format, args);
        }

        public void Error(string format, params object[] args)
        {
            Console.WriteLine("[ERROR] " + format, args);
        }
    }
}
