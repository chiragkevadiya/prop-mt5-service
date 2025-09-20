using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ClientMT5
{
    public class ClientConnectDemo
    {
        // Demo Manager API
        public CIMTManagerAPI m_manager_demo = null;


        public MTRetCode Initialize_demo()
        {
            MTRetCode res = MTRetCode.MT_RET_ERROR;

            //--- Initialize the factory 
            if ((res = SMTManagerAPIFactory.Initialize("C:\\dll_dot\\MT5Libs")) != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("Demo SMTManagerAPIFactory.Initialize failed - {0}", res);
                return (res);
            }
            uint version = 0;

            if ((res = SMTManagerAPIFactory.GetVersion(out version)) != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("Demo SMTManagerAPIFactory.GetVersion failed - {0}", res);
                return (res);
            }
            //--- Compare the obtained version with the library one 
            if (version != SMTManagerAPIFactory.ManagerAPIVersion)
            {
                Console.WriteLine("Demo Manager API version mismatch - {0}!={1}", version, SMTManagerAPIFactory.ManagerAPIVersion);
                return (MTRetCode.MT_RET_ERROR);
            }
            //--- Create an instance Manager
            m_manager_demo = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out res);
            if (res != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("Demo SMTManagerAPIFactory.CreateManager failed - {0}", res);
                return (res);
            }
            //--- For some reasons, the creation method returned OK and the null pointer 
            if (m_manager_demo == null)
            {
                Console.WriteLine("Demo SMTManagerAPIFactory.CreateManager was ok, but ManagerAPI is null");
                return (MTRetCode.MT_RET_ERR_MEM);
            }
            //--- All is well 
            Console.WriteLine("Demo Using ManagerAPI v. {0}", version);

            return (res);

        }


        public MTRetCode Connect_demo(string server, UInt64 login, string password, uint timeout)
        {
            MTRetCode res = MTRetCode.MT_RET_ERROR;
            if (m_manager_demo == null)
            {
                Console.WriteLine("Demo Connection to {0} failed: .NET Manager API is NULL", EnMTLogCode.MTLogErr, server);
                return (res);
            }
            //---  
            res = m_manager_demo.Connect(server, login, password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, timeout);

            CreateDemoManagerHelper.InitializeDemoManager(m_manager_demo);

            if (res != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("Demo Connection by Managed API to {0} failed: {1}", EnMTLogCode.MTLogErr, server, res);
                return (res);
            }
            //--- 
            Console.WriteLine("Connected Demo Manager");
            return (res);
        }


        private void OnRequestServerLogs(EnMTLogRequestMode requestMode, EnMTLogType logType, Int64 from, Int64 to, string filter = null)
        {
            if (m_manager_demo == null)
            {
                Console.WriteLine("ERROR: Manager was not created");
                return;
            }
            //Console.WriteLine(EnMTLogCode.MTLogAtt, "LogTests", "");
            try
            {
                MTRetCode result = MTRetCode.MT_RET_ERROR;
                //--- 
                MTLogRecord[] records = m_manager_demo.LoggerServerRequest(requestMode, logType, from, to, filter, out result);
                //--- 
                Console.WriteLine("LoggerServerRequest {0} ==> [{1}] return {2} record(s)",
                             (result == MTRetCode.MT_RET_OK ? "ok" : "failed"),
                             result, (records != null ? records.Length : 0));
                //--- 
                if ((result == MTRetCode.MT_RET_OK) && (records != null))
                {
                    foreach (MTLogRecord rec in records)
                        Console.WriteLine(rec);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

