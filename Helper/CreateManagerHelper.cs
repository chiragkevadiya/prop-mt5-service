using MetaQuotes.MT5ManagerAPI;


namespace NaptunePropTrading_Service.Helper
{
    public static class CreateManagerHelper
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
