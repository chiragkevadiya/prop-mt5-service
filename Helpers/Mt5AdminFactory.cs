using MetaQuotes.MT5ManagerAPI;

namespace PropMT5ConnectionService.Helpers
{
    public static class Mt5AdminFactory
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
