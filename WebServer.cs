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

            _webapp = WebApp.Start<Startup>("http://localhost:8083");

            // Live Maneger  Account
            ClientConnect clientConnect = new ClientConnect();
            clientConnect.Initialize();

            #region NeptuneFx
            //Demo
            clientConnect.Connect("176.9.16.247:443", 10000, "demo@10000", 30000);
            #endregion

            #region Pioneer
            //clientConnect.Connect("86.104.251.165:443", 1002, "Q*XaOoF1", 30000);
            #endregion

        }

        public void Stop()
        {
            _webapp?.Dispose();
        }
    }
}
