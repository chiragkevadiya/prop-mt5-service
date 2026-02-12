namespace PropMT5ConnectionService.Configuration
{
    /// <summary>
    /// MT5 connection settings
    /// </summary>
    public class MT5ConnectionSettings
    {
        public LiveSettings Live { get; set; }
        public DemoSettings Demo { get; set; }
    }

    public class LiveSettings
    {
        public string Server { get; set; }
        public ulong Login { get; set; }
        public string Password { get; set; }
        public uint Timeout { get; set; } = 30000;
    }

    public class DemoSettings
    {
        public string Server { get; set; }
        public ulong Login { get; set; }
        public string Password { get; set; }
        public uint Timeout { get; set; } = 30000;
    }

    /// <summary>
    /// Web server settings
    /// </summary>
    public class WebServerSettings
    {
        public string BaseUri { get; set; }
        public int Port { get; set; }
        public bool EnableSwagger { get; set; }
    }

    /// <summary>
    /// Logging settings
    /// </summary>
    public class LoggingSettings
    {
        public string LogLevel { get; set; } = "Information";
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableConsoleLogging { get; set; } = true;
        public string LogDirectory { get; set; }
    }

    /// <summary>
    /// Background jobs settings
    /// </summary>
    public class BackgroundJobsSettings
    {
        public int LiquidationCheckIntervalSeconds { get; set; } = 60;
        public bool EnableLiquidationMonitor { get; set; } = true;
    }

    /// <summary>
    /// Email settings
    /// </summary>
    public class ClientEmailSetting
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; } = true;
    }
}
