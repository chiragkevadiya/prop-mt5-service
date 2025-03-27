using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using NaptunePropTrading_Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service
{
    public class ClientConnect
    {
        // Manager API  
        public CIMTManagerAPI m_manager = null;

        // CIMT Admin API 
        //public CIMTAdminAPI m_admin = null;

        public MTRetCode Initialize()
        {
            MTRetCode res = MTRetCode.MT_RET_ERROR;

            //--- Initialize the factory 
            if ((res = SMTManagerAPIFactory.Initialize("C:\\dll_dot\\MT5Libs")) != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("SMTManagerAPIFactory.Initialize failed - {0}", res);
                return (res);
            }
            uint version = 0;

            if ((res = SMTManagerAPIFactory.GetVersion(out version)) != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("SMTManagerAPIFactory.GetVersion failed - {0}", res);
                return (res);
            }
            //--- Compare the obtained version with the library one 
            if (version != SMTManagerAPIFactory.ManagerAPIVersion)
            {
                Console.WriteLine("Manager API version mismatch - {0}!={1}", version, SMTManagerAPIFactory.ManagerAPIVersion);
                return (MTRetCode.MT_RET_ERROR);
            }
            //--- Create an instance Manager
            m_manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out res);
            if (res != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("SMTManagerAPIFactory.CreateManager failed - {0}", res);
                return (res);
            }
            //--- For some reasons, the creation method returned OK and the null pointer 
            if (m_manager == null)
            {
                Console.WriteLine("SMTManagerAPIFactory.CreateManager was ok, but ManagerAPI is null");
                return (MTRetCode.MT_RET_ERR_MEM);
            }
            //--- All is well 
            Console.WriteLine("Using ManagerAPI v. {0}", version);
            
            return (res);

            
        }


        public MTRetCode Connect(string server, UInt64 login, string password, uint timeout)
        {
            MTRetCode res = MTRetCode.MT_RET_ERROR;
            if (m_manager == null)
            {
                Console.WriteLine("Connection to {0} failed: .NET Manager API is NULL", EnMTLogCode.MTLogErr, server);
                return (res);
            }
            //---  
            res = m_manager.Connect(server, login, password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, timeout);

            CreateManagerHelper.InitializeManager(m_manager);

            if (res != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("Connection by Managed API to {0} failed: {1}", EnMTLogCode.MTLogErr, server, res);
                return (res);
            }
            //--- 
            Console.WriteLine("Connected Live Manager");
            return (res);
        }


        

    }
}
