using PropMT5ConnectionService.Helpers;
using Serilog;

namespace PropMT5ConnectionService.Mt5Client
{
    /// <summary>
    /// MT5 Demo server client providing connection and management capabilities
    /// </summary>
    public class Mt5DemoClient : Mt5ClientBase
    {
        protected override string ClientType => "Demo";

        /// <summary>
        /// Create a new Demo client instance
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <param name="libraryPath">Path to MT5 libraries (default: C:\dll_dot\MT5Libs)</param>
        public Mt5DemoClient(ILogger logger, string libraryPath = @"C:\dll_dot\MT5Libs")
            : base(logger, libraryPath)
        {
        }

        protected override void InitializeManagerFactory()
        {
            if (Manager != null)
            {
                Mt5DemoManagerFactory.InitializeDemoManager(Manager);
            }
        }
    }
}
