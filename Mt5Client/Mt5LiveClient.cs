using PropMT5Service.Helpers;

namespace PropMT5Service.Mt5Client
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
        /// <param name="libraryPath">Path to MT5 libraries (default: C:\dll_dot\MT5Libs)</param>
        public Mt5LiveClient(string libraryPath = @"C:\dll_dot\MT5Libs")
            : base(libraryPath)
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
