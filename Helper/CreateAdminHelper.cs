using MetaQuotes.MT5ManagerAPI;


namespace NaptunePropTrading_Service.Helper
{
    public static class CreateAdminHelper
    {
        private static CIMTAdminAPI _adminInstance { get; set; }

        public static void InitializeAdmin(CIMTAdminAPI entity)
        {
            _adminInstance = entity;
        }

        public static CIMTAdminAPI GetAdmin()
        {
            return _adminInstance;
        }
    }
}
