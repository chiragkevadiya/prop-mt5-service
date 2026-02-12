using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT5ConnectionService.ClientMT5;
using MT5ConnectionService.Helper;
using PropMT5ConnectionService.Configuration;
using PropMT5ConnectionService.Services;
using PropMT5ConnectionService.Services.Implementations;
using PropMT5ConnectionService.Services.Interfaces;
using System;
using Topshelf;

namespace MT5ConnectionService
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTopshelf();
        }

        static void StartTopshelf()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (environment == null)
            {
                environment = "Development";
            }
            Console.WriteLine($"[INFO] Running in {environment} environment");

            // Build DI container
            var services = new ServiceCollection();

            // Load configuration based on environment
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Configure strongly-typed settings
            services.Configure<MT5ConnectionSettings>(configuration.GetSection("MT5"));
            services.Configure<LoggingSettings>(configuration.GetSection("Logging"));
            services.Configure<WebServerSettings>(configuration.GetSection("WebServer"));
            services.Configure<BackgroundJobsSettings>(configuration.GetSection("BackgroundJobs"));
            services.Configure<ClientEmailSetting>(configuration.GetSection("ClientEmailSetting"));

            // Register the MT5 Manager API as a Singleton
            services.AddSingleton<CIMTManagerAPI>(provider =>
            {
                var connector = new ClientConnect();
                MTRetCode initResult = connector.Initialize();
                if (initResult != MTRetCode.MT_RET_OK)
                {
                    throw new InvalidOperationException($"FATAL: MT5 API Factory initialization failed: {initResult}");
                }

                var config = provider.GetRequiredService<IConfiguration>();
                string server = config["MT5:Live:Server"];
                ulong login = ulong.Parse(config["MT5:Live:Login"]);
                string password = config["MT5:Live:Password"];
                uint timeout = uint.Parse(config["MT5:Live:Timeout"]);

                Console.WriteLine($"[INFO] Attempting to connect to MT5 Manager API at {server} with login {login}...");

                MTRetCode connectResult = connector.Connect(server, login, password, timeout);

                if (connectResult != MTRetCode.MT_RET_OK)
                {
                    if (connector.m_manager != null)
                        connector.m_manager.Release();
                    throw new InvalidOperationException($"FATAL: MT5 Manager Login failed: {connectResult}");
                }

                Console.WriteLine($"[INFO] Successfully connected to MT5 Manager API.");
                return connector.m_manager;
            });

            // Register Core Services
            services.AddSingleton<IMT5ConnectionService, PropMT5ConnectionService.Services.Implementations.MT5ConnectionService>();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddScoped<ILiquidationService, LiquidationService>();

            // Register the WebServer service itself
            services.AddSingleton<WebServer>();

            // Build provider
            var serviceProvider = services.BuildServiceProvider();

            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name =>
                    {
                        return serviceProvider.GetService<WebServer>();
                    });
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("This service manages connections to MT5 (MetaTrader 5).");
                x.SetDisplayName("Prop MT5 Connection Service");
                x.SetServiceName("PropMT5ConnectionService");
            });
        }
    }
}
