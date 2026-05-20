using Microsoft.Owin.Hosting;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.Mt5Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PropMT5Service
{
    /// <summary>
    /// Web Server hosting OWIN pipeline and managing background jobs
    /// Integrates with MT5 Live and Demo clients for trading operations
    /// </summary>
    public class WebServer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mt5LiveClient _mt5LiveClient;
        private IDisposable _webapp;
        private CancellationTokenSource _cts;
        private Task _backgroundTask;
        private Task _reconnectTask;

        public WebServer(IServiceProvider serviceProvider, Mt5LiveClient mt5LiveClient)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _mt5LiveClient = mt5LiveClient ?? throw new ArgumentNullException(nameof(mt5LiveClient));
            LogMT5ClientsStatus();
        }

        private void LogMT5ClientsStatus()
        {
        }


        public void Start()
        {

            string baseUri = MT5Constants.WebServer.BaseUri;

            Console.WriteLine($"Starting web server at {baseUri}");

            try
            {
                // Verify MT5 clients are ready
                VerifyMT5ClientsReady();

                // Start OWIN web application
                _webapp = WebApp.Start(baseUri, appBuilder =>
                {
                    new Startup(_serviceProvider).Configuration(appBuilder);
                });



                // Start daily MT5 reconnect scheduler
                _cts = new CancellationTokenSource();
                StartScheduledReconnect();

                // Automatically open browser to welcome page
                OpenBrowserToWelcomePage(baseUri);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void VerifyMT5ClientsReady()
        {
            if (_mt5LiveClient == null)
            {
                throw new InvalidOperationException("MT5 Live Client is not initialized");
            }

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
                if (!MT5Constants.WebServer.AutoOpenBrowser)
                {
                    return;
                }

                // Construct the welcome page URL
                var welcomeUrl = baseUri.TrimEnd('/') + MT5Constants.WebServer.WelcomePath;


                // Use Process.Start to open the default browser
                // This works for both Development and Production environments
                var psi = new ProcessStartInfo
                {
                    FileName = welcomeUrl,
                    UseShellExecute = true // Important: Use shell execute to open with default browser
                };

                Process.Start(psi);

            }
            catch (Exception ex)
            {
                // Don't fail the startup if browser opening fails
            }
        }


        private void StartScheduledReconnect()
        {
            var reconnectHour = MT5Constants.ScheduledTasks.DailyReconnectHour;
            var reconnectMinute = MT5Constants.ScheduledTasks.DailyReconnectMinute;

            _reconnectTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    TimeSpan delay = GetDelayUntilNextReconnect(reconnectHour, reconnectMinute);

                    try
                    {
                        await Task.Delay(delay, _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    if (_cts.Token.IsCancellationRequested)
                        break;

                    PerformScheduledReconnect();
                }

            }, _cts.Token);
        }

        private static TimeSpan GetDelayUntilNextReconnect(int hour, int minute)
        {
            var now = DateTime.Now;
            var next = DateTime.Today.AddHours(hour).AddMinutes(minute);

            if (next <= now)
                next = next.AddDays(1);

            return next - now;
        }

        private void PerformScheduledReconnect()
        {

            try
            {
                var result = _mt5LiveClient.Reconnect();

                if (result == MetaQuotes.MT5CommonAPI.MTRetCode.MT_RET_OK)
                {
                    Helpers.AccountLogHelper.LogReconnect(success: true);
                }
                else
                {
                    Helpers.AccountLogHelper.LogReconnect(success: false, retCode: result.ToString());
                }
            }
            catch (Exception ex)
            {
                Helpers.AccountLogHelper.LogReconnect(success: false, exception: ex);
            }
        }

        public void Stop()
        {

            // Cancel background jobs and reconnect scheduler
            _cts?.Cancel();

            try
            {
                // Wait for background task to complete with timeout
                if (_backgroundTask != null)
                {
                    var completed = _backgroundTask.Wait(TimeSpan.FromSeconds(MT5Constants.BackgroundJobs.ShutdownTimeoutSeconds));

                    if (!completed)
                    {
                    }
                    else
                    {
                    }
                }
            }
            catch (AggregateException ex)
            {
            }
            finally
            {
                _backgroundTask?.Dispose();
                _reconnectTask?.Dispose();
            }

            // Dispose web app
            try
            {
                _webapp?.Dispose();
            }
            catch (Exception ex)
            {
            }

            // Dispose cancellation token source
            _cts?.Dispose();

        }
    }
}
