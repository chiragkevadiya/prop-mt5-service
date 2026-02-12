using MetaQuotes.MT5ManagerAPI;

namespace PropMT5ConnectionService.Helpers
{
    public static class Mt5DemoManagerFactory
    {
        private static CIMTManagerAPI _managerDemoInstance { get; set; }

        public static void InitializeDemoManager(CIMTManagerAPI cIMT)
        {
            _managerDemoInstance = cIMT;
        }

        public static CIMTManagerAPI GetManagerDemo()
        {
            return _managerDemoInstance;
        }
    }
}
