namespace PropMT5Service.Constants
{
    /// <summary>
    /// Constants for MT5 operations
    /// </summary>
    public static class MT5Constants
    {
        /// <summary>
        /// Password configuration
        /// </summary>
        public static class PasswordConfig
        {
            public const int DefaultMasterPasswordLength = 11;
            public const int DefaultInvestorPasswordLength = 9;
            public const int MinPasswordLength = 8;
            public const int MaxPasswordLength = 20;
        }

        /// <summary>
        /// Configuration for account creation
        /// </summary>
        public class AccountCreationConfig
        {
            public string LoginPrefix { get; set; } = "555";
            public int MasterPasswordLength { get; set; } = 11;
            public int InvestorPasswordLength { get; set; } = 9;
            public string ServerName { get; set; } = "MT5 Contest Service";
            public string AccountType { get; set; } = "Live";
        }

        /// <summary>
        /// API Response messages
        /// </summary>
        public static class ResponseMessages
        {
            // Success messages
            public const string AccountCreatedSuccess = "Account created successfully";
            public const string AccountRetrievedSuccess = "Account retrieved successfully";
            public const string AccountsRetrievedSuccess = "Accounts retrieved successfully";
            public const string PasswordChangedSuccess = "{0} password changed successfully";
            public const string StatusUpdatedSuccess = "Account status updated successfully";

            // Error messages
            public const string ModelCannotBeNull = "Model cannot be null";
            public const string InvalidModelState = "Invalid model state";
            public const string PasswordCannotBeEmpty = "Password cannot be empty";
            public const string InvalidPasswordType = "Password type must be 0 (Master) or 1 (Investor)";
            public const string AccountCreationFailed = "Failed to create account after {0} attempts";
            public const string UnexpectedError = "An unexpected error occurred: {0}";
        }
        
        /// <summary>
        /// MT5 Live connection settings
        /// </summary>
        public static class MT5Live
        {
            public const string LibraryPath = @"C:\dll_dot\MT5Libs";
            public const string Server = "37.27.232.54:1950";
            public const ulong Login = 1000;
            public const string Password = "Rock@1000";
            public const uint Timeout = 30000;
        }

        /// <summary>
        /// Logging path settings
        /// </summary>
        public static class Logging
        {
            public const string BasePath = @"C:\PropMT5Service\MT5ServicesLogSave";
            public const string SuccessPath = @"C:\PropMT5Service\MT5ServicesLogSave\Success";
            public const string FailedPath = @"C:\PropMT5Service\MT5ServicesLogSave\Failed";
            public const string ReconnectPath = @"C:\PropMT5Service\MT5ServicesLogSave\Reconnect";
        }

        /// <summary>
        /// Web server settings
        /// </summary>
        public static class WebServer
        {
            public const string BaseUri = "http://localhost:8086";
            public const bool AutoOpenBrowser = true;
            public const string WelcomePath = "/welcome";
        }

        /// <summary>
        /// CORS policy defaults
        /// </summary>
        public static class Cors
        {
            public const string AllowedOrigins = "*";
            public const string AllowedMethods = "GET, POST, PUT, DELETE, OPTIONS";
            public const string AllowedHeaders = "Content-Type, Authorization, X-Requested-With, X-Api-Key";
            public const string MaxAgeSeconds = "3600";
        }

        /// <summary>
        /// Background job timing settings
        /// </summary>
        public static class BackgroundJobs
        {
            public const int LiquidationCheckIntervalSeconds = 3600;
            public const int InitialDelaySeconds = 5;
            public const int ErrorRetryDelaySeconds = 10;
            public const int ShutdownTimeoutSeconds = 15;
        }

        /// <summary>
        /// Service identity and metadata
        /// </summary>
        public static class ServiceInfo
        {
            public const string Version = "1.0.0";
            public const string Name = "PropMT5Service";
            public const string DisplayName = "MT5 Contest Service";
            public const string Description = "Manages connections to MT5 (MetaTrader 5) servers for trading operations";
            public const string Author = "Amit Kumar";
            public const string ProductHeader = "MT5 Contest Services Connection";
            public const string PoweredBy = "OWIN/ASP.NET Web API";
        }

        /// <summary>
        /// Health check endpoint paths and status strings
        /// </summary>
        public static class HealthCheck
        {
            public const string PathSimple = "/health";
            public const string PathApi = "/api/health";
            public const string ApiRoutePrefix = "api/health";
            public const string StatusHealthy = "Healthy";
            public const string StatusDegraded = "Degraded";
            public const string StatusAlive = "Alive";
            public const string StatusReady = "Ready";
            public const string StatusNotReady = "Not Ready";
            public const string ComponentOk = "OK";
            public const string MT5Connected = "Connected";
            public const string MT5Disconnected = "Disconnected";
            public const string CacheRunning = "Running";
            public const string CacheDown = "Down";
            public const string MT5NotAvailable = "MT5 Connection Not Available";
            public const string MetricsResetSuccess = "Metrics reset successfully";
            public const string CacheTestKey = "__health_check__";
            public const string CacheTestValue = "test";
            public const int CacheTestTimeoutSeconds = 1;
        }

        /// <summary>
        /// HTTP security header values
        /// </summary>
        public static class SecurityHeaders
        {
            public const string XContentTypeOptions = "nosniff";
            public const string XFrameOptions = "DENY";
            public const string XXSSProtection = "1; mode=block";
            public const string ReferrerPolicy = "strict-origin-when-cross-origin";
            public const string PermissionsPolicy = "geolocation=(), microphone=(), camera=()";
        }

        /// <summary>
        /// In-memory cache timing and key prefix settings
        /// </summary>
        public static class Cache
        {
            public const int DefaultExpirationMinutes = 15;
            public const int CleanupIntervalMinutes = 1;
            public const string AccountPrefix = "account";
            public const string TradingHistoryPrefix = "trading:history";
            public const string TradingDataPrefix = "trading:data";
            public const string GroupPrefix = "group";
            public const string SymbolPrefix = "symbol";
            public const string LeaderboardPrefix = "leaderboard";
            public const string DashboardPrefix = "dashboard";
        }

        /// <summary>
        /// MT5 account and balance operation settings
        /// </summary>
        public static class AccountOperations
        {
            public const int MaxCreateAttempts = 100;
            public const string WildcardGroup = "*";
            public const int DealerBalanceType = 2;
            public const int LiquidationBatchSize = 100;
        }

        /// <summary>
        /// Logging service settings
        /// </summary>
        public static class LoggingConfig
        {
            public const string ServiceName = "MT5Service";
            public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
            public const string LogFilePrefix = "MT5Service_";
            public const string LogFileDateFormat = "yyyyMMdd";
            public const string LogFileExtension = ".log";
            public const string LogFolder = "Logs";
        }

        /// <summary>
        /// Accepted date format strings for the date parser
        /// </summary>
        public static class DateFormats
        {
            public const string NotAvailable = "NA";
            public static readonly string[] SupportedFormats =
            {
                "MMMM d, yyyy", "MMM d, yyyy",
                "M/d/yyyy",     "M/dd/yyyy",
                "MM/d/yyyy",    "MM/dd/yyyy",
                "dd-MM-yyyy",   "dd/MM/yyyy",
                "M/d/yyyy h:mm:ss tt",
                "MM-dd-yyyy"
            };
        }

        /// <summary>
        /// MT5 native library DLL file names
        /// </summary>
        public static class MT5Library
        {
            public static readonly string[] RequiredDlls =
            {
                "mtmanapi64.dll",
                "MT5CommonAPI.dll",
                "MT5ManagerAPI.dll"
            };
        }

        /// <summary>
        /// Scheduled maintenance task settings
        /// </summary>
        public static class ScheduledTasks
        {
            /// <summary>Hour (24-hour) at which the daily MT5 reconnect fires</summary>
            public const int DailyReconnectHour = 5;
            /// <summary>Minute at which the daily MT5 reconnect fires</summary>
            public const int DailyReconnectMinute = 0;
        }
    }
}
