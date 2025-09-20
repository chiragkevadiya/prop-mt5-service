using Microsoft.Owin.Hosting;
using MT5ConnectionService.ClientMT5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService
{
    public class WebServer
    {
        private IDisposable _webapp;


        public void Start()
        {

            _webapp = WebApp.Start<Startup>("http://localhost:8086");

            // Live Maneger  Account
            ClientConnect clientConnect = new ClientConnect();
            clientConnect.Initialize();

            // Demo Manager Account
            ClientConnectDemo clientConnectDemo = new ClientConnectDemo();
            clientConnectDemo.Initialize_demo();

            #region NeptuneFx
            //Demo
            clientConnect.Connect("37.27.232.54:1950", 1000, "Rock@1000", 30000);

            #endregion

        }

        public void Stop()
        {
            _webapp?.Dispose();
        }
    }
}
