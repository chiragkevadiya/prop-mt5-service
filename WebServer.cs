//using Microsoft.Owin.Hosting;
//using MT5ConnectionService.ClientMT5;
//using PropMT5ConnectionService.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MT5ConnectionService
//{
//    //public class WebServer
//    //{
//    //    private IDisposable _webapp;


//    //    public void Start()
//    //    {

//    //        _webapp = WebApp.Start<Startup>("http://localhost:8086");

//    //        // Live Maneger  Account
//    //        ClientConnect clientConnect = new ClientConnect();
//    //        clientConnect.Initialize();

//    //        // Demo Manager Account
//    //        ClientConnectDemo clientConnectDemo = new ClientConnectDemo();
//    //        clientConnectDemo.Initialize_demo();

//    //        #region NeptuneFx
//    //        //Demo
//    //        clientConnect.Connect("37.27.232.54:1950", 1000, "Rock@1000", 30000);

//    //        #endregion

//    //    }

//    //    public void Stop()
//    //    {
//    //        _webapp?.Dispose();
//    //    }
//    //}

//    public class WebServer
//    {
//        private IDisposable _webapp;
//        private CancellationTokenSource _cts;
//        private Task _backgroundTask;

//        public void Start()
//        {
//            _webapp = WebApp.Start<Startup>("http://localhost:8086");

//            // Initialize MT5 Clients
//            ClientConnect clientConnect = new ClientConnect();
//            clientConnect.Initialize();
//            ClientConnectDemo clientConnectDemo = new ClientConnectDemo();
//            clientConnectDemo.Initialize_demo();
//            clientConnect.Connect("37.27.232.54:1950", 1000, "Rock@1000", 30000);

//            // Start background liquidation monitor
//            _cts = new CancellationTokenSource();
//            _backgroundTask = Task.Run(() => RunBackgroundJobs(_cts.Token));
//        }

//        private async Task RunBackgroundJobs(CancellationToken token)
//        {
//            while (!token.IsCancellationRequested)
//            {
//                try
//                {
//                    // 🔥 Call your liquidation logic
//                    var service = new LiquidationService();
//                    await service.CheckAndLiquidateAccountsMT5();

//                    // wait 30 seconds before checking again
//                    await Task.Delay(TimeSpan.FromSeconds(30), token);
//                }
//                catch (TaskCanceledException) { }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"[ERROR] Background job: {ex.Message}");
//                }
//            }
//        }

//        public void Stop()
//        {
//            _cts?.Cancel();
//            _backgroundTask?.Wait();
//            _webapp?.Dispose();
//        }
//    }

//}


//using Microsoft.Owin.Hosting;
//using MT5ConnectionService.ClientMT5;
//using PropMT5ConnectionService.Services;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MT5ConnectionService
//{
//    public class WebServer
//    {
//        private IDisposable _webapp;
//        private CancellationTokenSource _cts;
//        private Task _backgroundTask;
//        private readonly ILiqudationService _liquidationService;

//        // Inject services through constructor
//        public WebServer(ILiqudationService liquidationService)
//        {
//            _liquidationService = liquidationService;
//        }

//        public void Start()
//        {
//            _webapp = WebApp.Start<Startup>("http://localhost:8086");

//            // Initialize MT5 Clients
//            var clientConnect = new ClientConnect();
//            clientConnect.Initialize();

//            var clientConnectDemo = new ClientConnectDemo();
//            clientConnectDemo.Initialize_demo();

//            clientConnect.Connect("37.27.232.54:1950", 1000, "Rock@1000", 30000);

//            // Start background liquidation monitor
//            _cts = new CancellationTokenSource();
//            _backgroundTask = Task.Run(() => RunBackgroundJobs(_cts.Token));
//        }

//        private async Task RunBackgroundJobs(CancellationToken token)
//        {
//            while (!token.IsCancellationRequested)
//            {
//                try
//                {
//                    // Call liquidation logic
//                    await _liquidationService.CheckAndLiquidateAccounts();

//                    // wait 30 seconds before checking again
//                    await Task.Delay(TimeSpan.FromSeconds(3600), token);
//                }
//                catch (TaskCanceledException) { }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"[ERROR] Background job: {ex.Message}");
//                }
//            }
//        }

//        public void Stop()
//        {
//            _cts?.Cancel();
//            try
//            {
//                _backgroundTask?.Wait();
//            }
//            catch (AggregateException) { }
//            _webapp?.Dispose();
//        }
//    }
//}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Hosting;
using PropMT5ConnectionService.Services;

namespace MT5ConnectionService
{
    public class WebServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILiqudationService _liquidationService;
        private IDisposable _webapp;
        private CancellationTokenSource _cts;
        private Task _backgroundTask;

        // Inject both the service provider and the specific service you need.
        // This is a common pattern to bridge DI and OWIN.
        public WebServer(IServiceProvider serviceProvider, ILiqudationService liquidationService)
        {
            _serviceProvider = serviceProvider;
            _liquidationService = liquidationService;
        }

        public void Start()
        {
            Console.WriteLine("[INFO] WebServer starting...");
            string baseUri = "http://localhost:8086";

            // Pass the service provider to the OWIN Startup class
            _webapp = WebApp.Start(baseUri, appBuilder =>
            {
                new Startup(_serviceProvider).Configuration(appBuilder);
            });

            // Start background liquidation monitor
            _cts = new CancellationTokenSource();
            _backgroundTask = Task.Run(() => RunBackgroundJobs(_cts.Token));

            Console.WriteLine($"[INFO] WebServer started at {baseUri}");
        }

        private async Task RunBackgroundJobs(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("[INFO] Running liquidation job...");
                    await _liquidationService.CheckAndLiquidateAccounts();

                    // wait 60 minutes before checking again
                    await Task.Delay(TimeSpan.FromSeconds(3600), token);
                }
                catch (TaskCanceledException) { /* Swallow exception on shutdown */ }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Background job: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine("[INFO] WebServer stopping...");
            _cts?.Cancel();
            try
            {
                _backgroundTask?.Wait();
            }
            catch (AggregateException) { }
            _webapp?.Dispose();
            Console.WriteLine("[INFO] WebServer stopped.");
        }
    }
}

