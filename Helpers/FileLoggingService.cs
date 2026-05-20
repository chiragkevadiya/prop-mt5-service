using MT5ContestService.Constants;
using System;
using System.IO;
using System.Text;

namespace MT5ContestService.Helpers
{
    /// <summary>
    /// File-based logging service for application diagnostics and events
    /// Writes logs to configured directories for persistence
    /// </summary>
    public static class FileLoggingService
    {
        private static readonly object _lockObject = new object();

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Debug
        }

        /// <summary>
        /// Log a message to file and console
        /// </summary>
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string prefix = $"[{level.ToString().ToUpper()}]";
            string logMessage = $"{prefix} {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            // Write to console

            // Write to file
            WriteToFile(logMessage, level);
        }

        /// <summary>
        /// Log an exception with message
        /// </summary>
        public static void LogException(string message, Exception ex, LogLevel level = LogLevel.Error)
        {
            string logMessage = $"{message}{Environment.NewLine}Exception: {ex?.Message}{Environment.NewLine}StackTrace: {ex?.StackTrace}";
            Log(logMessage, level);
        }

        /// <summary>
        /// Log HTTP request/response details
        /// </summary>
        public static void LogHttpRequest(string method, string path, int statusCode, long elapsedMs, string requestId)
        {
            string message = $"HTTP {method} {path} responded {statusCode} in {elapsedMs}ms. RequestId: {requestId}";
            LogLevel level = statusCode >= 500 ? LogLevel.Error : statusCode >= 400 ? LogLevel.Warning : LogLevel.Info;
            Log(message, level);
        }

        /// <summary>
        /// Log MT5 operation result
        /// </summary>
        public static void LogMT5Operation(string operation, string result, string details = "")
        {
            string message = $"MT5 Operation: {operation} - Result: {result}";
            if (!string.IsNullOrEmpty(details))
                message += $" - {details}";

            LogLevel level = result.Contains("failed") || result.Contains("error") ? LogLevel.Error : LogLevel.Info;
            Log(message, level);
        }

        private static void WriteToFile(string message, LogLevel level)
        {
            try
            {
                lock (_lockObject)
                {
                    string logDirectory = GetLogDirectory(level);

                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    string fileName = $"Application_{DateTime.Now:yyyyMMdd}.log";
                    string filePath = Path.Combine(logDirectory, fileName);

                    using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                }
            }
            catch
            {
                // Silently fail to avoid infinite loop if logging itself fails
            }
        }

        private static string GetLogDirectory(LogLevel level)
        {
            return level switch
            {
                LogLevel.Error => MT5Constants.Logging.FailedPath,
                LogLevel.Warning => MT5Constants.Logging.FailedPath,
                LogLevel.Info => MT5Constants.Logging.SuccessPath,
                LogLevel.Debug => MT5Constants.Logging.SuccessPath,
                _ => MT5Constants.Logging.BasePath
            };
        }
    }
}
