using Microsoft.Owin.Hosting;
using NaptunePropTrading_Service.ClientMT5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service
{
    public class WebServer
    {
        private IDisposable _webapp;


        public void Start()
        {
            _webapp = WebApp.Start<Startup>("http://localhost:8080");

            // Live Maneger  Account
            ClientConnect clientConnect = new ClientConnect();
            clientConnect.Initialize();

            // Demo Manager Account
            ClientConnectDemo clientConnectDemo = new ClientConnectDemo();
            clientConnectDemo.Initialize_demo();

            #region Naptune_Prop_Trading 
            clientConnect.Connect("176.9.16.247:443", 10000, "demo@10000", 30000);
            #endregion

        }

        public void Stop()
        {
            _webapp?.Dispose();
        }
    }
}
