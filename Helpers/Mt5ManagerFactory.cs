using MetaQuotes.MT5ManagerAPI;

namespace PropMT5Service.Helpers
{
    public static class Mt5ManagerFactory
    {
        private static CIMTManagerAPI _managerInstance { get; set; }

        public static void InitializeManager(CIMTManagerAPI cIMT)
        {
            _managerInstance = cIMT;
        }

        public static CIMTManagerAPI GetManager()
        {
            return _managerInstance;
        }
    }


}
