using MetaQuotes.MT5ManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.Helper
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
