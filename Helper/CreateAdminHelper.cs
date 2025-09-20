using MetaQuotes.MT5ManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.Helper
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
