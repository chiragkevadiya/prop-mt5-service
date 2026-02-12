using System;

namespace PropMT5ConnectionService.Services
{
    /// <summary>
    /// Interface for logging service
    /// </summary>
    public interface ILoggingService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception exception = null);
        void LogDebug(string message);
    }

    /// <summary>
    /// Console-based logging service implementation
    /// </summary>
    public class ConsoleLoggingService : ILoggingService
    {
        private readonly string _serviceName;

        public ConsoleLoggingService(string serviceName = "MT5Service")
        {
            _serviceName = serviceName;
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message, ConsoleColor.White);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message, ConsoleColor.Yellow);
        }

        public void LogError(string message, Exception exception = null)
        {
            WriteLog("ERROR", message, ConsoleColor.Red);
            if (exception != null)
            {
                WriteLog("ERROR", $"Exception: {exception.Message}", ConsoleColor.Red);
                WriteLog("ERROR", $"StackTrace: {exception.StackTrace}", ConsoleColor.DarkRed);
            }
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message, ConsoleColor.Gray);
        }

        private void WriteLog(string level, string message, ConsoleColor color)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var originalColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{timestamp}] [{level}] [{_serviceName}] {message}");
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }

    /// <summary>
    /// File-based logging service implementation
    /// </summary>
    public class FileLoggingService : ILoggingService
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public FileLoggingService(string logDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                logDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            var logsFolder = System.IO.Path.Combine(logDirectory, "Logs");
            if (!System.IO.Directory.Exists(logsFolder))
            {
                System.IO.Directory.CreateDirectory(logsFolder);
            }

            var logFileName = $"MT5Service_{DateTime.Now:yyyyMMdd}.log";
            _logFilePath = System.IO.Path.Combine(logsFolder, logFileName);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void LogError(string message, Exception exception = null)
        {
            WriteLog("ERROR", message);
            if (exception != null)
            {
                WriteLog("ERROR", $"Exception: {exception.Message}");
                WriteLog("ERROR", $"StackTrace: {exception.StackTrace}");
            }
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        private void WriteLog(string level, string message)
        {
            lock (_lockObject)
            {
                try
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var logEntry = $"[{timestamp}] [{level}] {message}";
                    System.IO.File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to write to log file: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Composite logging service that logs to multiple destinations
    /// </summary>
    public class CompositeLoggingService : ILoggingService
    {
        private readonly ILoggingService[] _loggers;

        public CompositeLoggingService(params ILoggingService[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        public void LogInfo(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogInfo(message);
            }
        }

        public void LogWarning(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogWarning(message);
            }
        }

        public void LogError(string message, Exception exception = null)
        {
            foreach (var logger in _loggers)
            {
                logger.LogError(message, exception);
            }
        }

        public void LogDebug(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.LogDebug(message);
            }
        }
    }
}
