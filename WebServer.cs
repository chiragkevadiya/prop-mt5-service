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

            #region infinityfx
            //Live
            //clientConnect.Connect("86.104.251.182:443", 1005, "OrGe!kH6", 30000);
            #endregion

            #region HorizoneFX
            //Live
            //clientConnect.Connect("86.104.251.192:443", 1007, "QfXs-hK4", 30000);

            // Demo
            //clientConnectDemo.Connect_demo("86.104.251.192:443", 1008, "FmJq-uA6", 30000);
            #endregion

            #region NeptuneFx
            //Demo
            clientConnect.Connect("176.9.16.247:443", 10000, "demo@10000", 30000);

            #endregion

            #region Pioneer
            //clientConnect.Connect("86.104.251.165:443", 1002, "Q*XaOoF1", 30000);
            #endregion

            #region Other
            //clientConnectDemo.Connect_demo("188.240.63.141:443", 1022, "U+2kCzLu", 30000);
            //clientConnect.Connect("188.240.63.141:443", 1029, "1uHqTeX@", 30000);
            #endregion

        }

        public void Stop()
        {
            _webapp?.Dispose();
        }
    }
}
