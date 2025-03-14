using System;
using System.IO;
using UCParser.Interface;

namespace UCParser.Core
{
    public class FileLogger : ILogger
    {
        private readonly string _logPath;
        private readonly object _lockObject = new object();

        public FileLogger(string logPath)
        {
            _logPath = logPath;
            
            string directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }
        }

        public void LogInfo(string message)
        {
            Log($"[INFO] {message}");
        }

        public void LogError(string message)
        {
            Log($"[ERROR] {message}");
        }

        public void LogDebug(string message)
        {
            Log($"[DEBUG] {message}");
        }

        private void Log(string message)
        {
            try
            {
                string formattedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                Console.WriteLine(formattedMessage);
                
                lock (_lockObject)
                {
                    File.AppendAllText(_logPath, formattedMessage + Environment.NewLine);
                }
            }
            catch
            {
                Console.WriteLine($"Failed to write to log file: {_logPath}");
            }
        }
    }
}
