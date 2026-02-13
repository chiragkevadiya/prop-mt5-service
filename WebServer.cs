using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Hosting;
using PropMT5ConnectionService.Mt5Client;
using PropMT5ConnectionService.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PropMT5ConnectionService
{
    /// <summary>
    /// Web Server hosting OWIN pipeline and managing background jobs
    /// Integrates with MT5 Live and Demo clients for trading operations
    /// </summary>
    public class WebServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly Mt5LiveClient _mt5LiveClient;
        private readonly Mt5DemoClient _mt5DemoClient;
        private IDisposable _webapp;
        private CancellationTokenSource _cts;
        private Task _backgroundTask;

        public WebServer(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            Mt5LiveClient mt5LiveClient,
            Mt5DemoClient mt5DemoClient = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mt5LiveClient = mt5LiveClient ?? throw new ArgumentNullException(nameof(mt5LiveClient));
            _mt5DemoClient = mt5DemoClient; // Optional - can be null

            Log.Information("WebServer instance created");
            LogMT5ClientsStatus();
        }

        private void LogMT5ClientsStatus()
        {
            Log.Information("MT5 Clients Status:");
            Log.Information("  - Live Client: {Status}", _mt5LiveClient != null ? "Connected" : "Not Available");

            if (_mt5DemoClient != null)
            {
                Log.Information("  - Demo Client: Connected");
            }
            else
            {
                Log.Information("  - Demo Client: Not configured (optional)");
            }
        }


        public void Start()
        {
            Log.Information("=== WebServer Starting ===");

            string baseUri = _configuration["WebServer:BaseUri"];
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                Log.Fatal("WebServer:BaseUri is not configured in appsettings.json");
                throw new InvalidOperationException("WebServer:BaseUri is not configured");
            }

            try
            {
                // Verify MT5 clients are ready
                VerifyMT5ClientsReady();

                // Start OWIN web application
                _webapp = WebApp.Start(baseUri, appBuilder =>
                {
                    new Startup(_serviceProvider).Configuration(appBuilder);
                });

                Log.Information("OWIN web application started successfully at {BaseUri}", baseUri);

                // Start background liquidation monitor
                _cts = new CancellationTokenSource();
                _backgroundTask = Task.Run(() => RunBackgroundJobs(_cts.Token));

                Log.Information("=== WebServer Started Successfully ===");
                Log.Information("API Base URL: {BaseUri}", baseUri);
                Log.Information("Health Check: {BaseUri}/health", baseUri);
                Console.WriteLine($"[INFO] WebServer started at {baseUri}");

                // Automatically open browser to welcome page
                OpenBrowserToWelcomePage(baseUri);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to start WebServer");
                throw;
            }
        }

        private void VerifyMT5ClientsReady()
        {
            if (_mt5LiveClient == null)
            {
                throw new InvalidOperationException("MT5 Live Client is not initialized");
            }

            Log.Information("MT5 Live Client is ready for trading operations");

            if (_mt5DemoClient != null)
            {
                Log.Information("MT5 Demo Client is ready for trading operations");
            }
        }


        private async Task RunBackgroundJobs(CancellationToken token)
        {
            if (!int.TryParse(_configuration["BackgroundJobs:LiquidationCheckIntervalSeconds"], out int intervalSeconds))
            {
                Log.Warning("Invalid liquidation interval configuration, using default: 60 seconds");
                intervalSeconds = 60;
            }

            Log.Information("Background liquidation job scheduled to run every {IntervalSeconds} seconds", intervalSeconds);

            // Initial delay to allow system to fully initialize
            await Task.Delay(TimeSpan.FromSeconds(5), token);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    Log.Debug("Running scheduled liquidation check...");

                    // Get liquidation service from DI container (scoped service)
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var liquidationService = scope.ServiceProvider.GetService<ILiquidationService>();

                        if (liquidationService != null)
                        {
                            var result = await liquidationService.CheckAndLiquidateAccounts();

                            if (result.Success)
                            {
                                Log.Information("Liquidation check completed: {Message}", result.Message);
                            }
                            else
                            {
                                Log.Warning("Liquidation check failed: {Message}", result.Message);
                            }
                        }
                        else
                        {
                            Log.Warning("ILiquidationService not available in DI container");
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), token);
                }
                catch (TaskCanceledException)
                {
                    Log.Information("Background liquidation job cancelled (service stopping)");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Background liquidation job error: {Message}", ex.Message);

                    // Wait before retrying to avoid tight error loops
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), token);
                    }
                    catch (TaskCanceledException)
                    {
                        Log.Information("Background job cancelled during error recovery");
                        break;
                    }
                }
            }

            Log.Information("Background liquidation job stopped");
        }


        /// <summary>
        /// Automatically opens the default browser to the welcome page
        /// Works in both Development and Production environments
        /// </summary>
        private void OpenBrowserToWelcomePage(string baseUri)
        {
            try
            {
                // Get the environment setting
                var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
                
                // Check if auto-open is enabled in configuration (optional setting)
                var autoOpenBrowser = _configuration["WebServer:AutoOpenBrowser"];
                if (!string.IsNullOrWhiteSpace(autoOpenBrowser) && autoOpenBrowser.ToLower() == "false")
                {
                    Log.Information("Auto-open browser is disabled in configuration");
                    return;
                }

                // Construct the welcome page URL
                var welcomeUrl = baseUri.TrimEnd('/') + "/welcome";
                
                Log.Information("Opening browser to welcome page: {WelcomeUrl}", welcomeUrl);
                Console.WriteLine($"[INFO] Opening browser to: {welcomeUrl}");

                // Use Process.Start to open the default browser
                // This works for both Development and Production environments
                var psi = new ProcessStartInfo
                {
                    FileName = welcomeUrl,
                    UseShellExecute = true // Important: Use shell execute to open with default browser
                };

                Process.Start(psi);
                
                Log.Information("Browser opened successfully in {Environment} environment", environment);
            }
            catch (Exception ex)
            {
                // Don't fail the startup if browser opening fails
                Log.Warning(ex, "Failed to automatically open browser: {Message}", ex.Message);
                Console.WriteLine($"[WARNING] Could not automatically open browser: {ex.Message}");
            }
        }


        public void Stop()
        {
            Log.Information("=== WebServer Stopping ===");

            // Cancel background jobs
            _cts?.Cancel();

            try
            {
                // Wait for background task to complete with timeout
                if (_backgroundTask != null)
                {
                    Log.Information("Waiting for background tasks to complete...");
                    var completed = _backgroundTask.Wait(TimeSpan.FromSeconds(15));

                    if (!completed)
                    {
                        Log.Warning("Background tasks did not complete within timeout period");
                    }
                    else
                    {
                        Log.Information("Background tasks completed successfully");
                    }
                }
            }
            catch (AggregateException ex)
            {
                Log.Warning(ex, "Error while stopping background tasks");
            }
            finally
            {
                _backgroundTask?.Dispose();
            }

            // Dispose web app
            try
            {
                _webapp?.Dispose();
                Log.Information("OWIN web application stopped");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while disposing web application");
            }

            // Dispose cancellation token source
            _cts?.Dispose();

            Log.Information("=== WebServer Stopped Successfully ===");
            Console.WriteLine("[INFO] WebServer stopped.");
        }
    }
}
