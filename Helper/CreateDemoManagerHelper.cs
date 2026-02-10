using MetaQuotes.MT5ManagerAPI;

namespace MT5ConnectionService.Helper
{
    public static class CreateDemoManagerHelper
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
