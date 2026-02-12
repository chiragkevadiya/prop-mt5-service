using PropMT5ConnectionService.Helpers;
using Serilog;

namespace PropMT5ConnectionService.Mt5Client
{
    /// <summary>
    /// MT5 Live server client providing connection and management capabilities
    /// </summary>
    public class Mt5LiveClient : Mt5ClientBase
    {
        protected override string ClientType => "Live";

        /// <summary>
        /// Create a new Live client instance
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <param name="libraryPath">Path to MT5 libraries (default: C:\dll_dot\MT5Libs)</param>
        public Mt5LiveClient(ILogger logger, string libraryPath = @"C:\dll_dot\MT5Libs")
            : base(logger, libraryPath)
        {
        }

        protected override void InitializeManagerFactory()
        {
            if (Manager != null)
            {
                Mt5ManagerFactory.InitializeManager(Manager);
            }
        }
    }
}
