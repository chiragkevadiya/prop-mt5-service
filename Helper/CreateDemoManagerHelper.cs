using MetaQuotes.MT5ManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.Helper
{
    public class CreateDemoManagerHelper
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
