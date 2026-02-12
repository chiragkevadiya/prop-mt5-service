using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Hosting;
using PropMT5ConnectionService.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PropMT5ConnectionService
{
    public class WebServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IDisposable _webapp;
        private CancellationTokenSource _cts;
        private Task _backgroundTask;

        public WebServer(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void Start()
        {
            Console.WriteLine("[INFO] WebServer starting...");
            
            string baseUri = _configuration["WebServer:BaseUri"];
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                throw new InvalidOperationException("WebServer:BaseUri is not configured");
            }

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
            if (!int.TryParse(_configuration["BackgroundJobs:LiquidationCheckIntervalSeconds"], out int intervalSeconds))
            {
                Console.WriteLine("[WARNING] Invalid liquidation interval configuration, using default: 60 seconds");
                intervalSeconds = 60;
            }

            Console.WriteLine($"[INFO] Background liquidation job will run every {intervalSeconds} seconds");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("[INFO] Running liquidation job...");
                    
                    // TODO: Implement actual liquidation check logic here
                    // var liquidationService = _serviceProvider.GetService<ILiquidationService>();
                    // await liquidationService.CheckLiquidations();

                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), token);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("[INFO] Background job cancelled (service stopping)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Background job error: {ex.Message}");
                    Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                    
                    // Wait before retrying to avoid tight error loops
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), token);
                    }
                    catch (TaskCanceledException) { }
                }
            }
        }

        public void Stop()
        {
            Console.WriteLine("[INFO] WebServer stopping...");
            
            // Cancel background jobs
            _cts?.Cancel();
            
            try
            {
                // Wait for background task to complete with timeout
                _backgroundTask?.Wait(TimeSpan.FromSeconds(10));
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"[WARNING] Error while stopping background task: {ex.Message}");
            }
            
            // Dispose web app
            _webapp?.Dispose();
            
            Console.WriteLine("[INFO] WebServer stopped.");
        }
    }
}
