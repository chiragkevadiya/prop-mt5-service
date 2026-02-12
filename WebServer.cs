using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Hosting;
using MT5ConnectionService.ClientMT5;
using PropMT5ConnectionService.Configuration;
using PropMT5ConnectionService.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT5ConnectionService
{
    public class WebServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILiquidationService _liquidationService;
        private IDisposable _webapp;
        private CancellationTokenSource _cts;
        private Task _backgroundTask;

        public WebServer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public void Start()
        {
            Console.WriteLine("[INFO] WebServer starting...");
            string baseUri = _configuration["WebServer:BaseUri"];

            // Initialize MT5 Clients
            ClientConnect clientConnect = new ClientConnect();
            clientConnect.Initialize();

            string server = _configuration["MT5:Live:Server"];
            ulong login = ulong.Parse(_configuration["MT5:Live:Login"]);
            string password = _configuration["MT5:Live:Password"];
            uint timeout = uint.Parse(_configuration["MT5:Live:Timeout"]);

            clientConnect.Connect(server, login, password, timeout);

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
            int intervalSeconds = int.Parse(_configuration["BackgroundJobs:LiquidationCheckIntervalSeconds"]);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("[INFO] Running liquidation job...");

                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), token);
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
