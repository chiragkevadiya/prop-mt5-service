using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropMT5ConnectionService.Configuration;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Mt5Client;
using PropMT5ConnectionService.Services;
using System;
using Topshelf;

namespace PropMT5ConnectionService
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTopshelf();
        }

        static void StartTopshelf()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            Console.WriteLine($"[INFO] Running in {environment} environment");

            // Build DI container
            var services = new ServiceCollection();

            // Load configuration based on environment
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Register the MT5 Manager API as a Singleton for Live
            services.AddSingleton<CIMTManagerAPI>(provider =>
            {
                var connector = new Mt5LiveClient();
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
                
                // Initialize static factory for backward compatibility (will be phased out)
                Mt5ManagerFactory.InitializeManager(connector.m_manager);
                
                return connector.m_manager;
            });

            // Register Core Services
            services.AddSingleton<ILoggingService>(provider => 
            {
                // Use composite logging (console + file)
                var consoleLogger = new ConsoleLoggingService("MT5Service");
                var fileLogger = new FileLoggingService();
                return new CompositeLoggingService(consoleLogger, fileLogger);
            });
            services.AddSingleton<ICachingService, MemoryCachingService>();
            services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddScoped<ILiquidationService, LiquidationService>();
            services.AddScoped<IMT5AccountService, MT5AccountService>();
            services.AddScoped<IMT5TradingService, MT5TradingService>();

            // Register the WebServer service itself
            services.AddSingleton<WebServer>();

            // Build provider
            var serviceProvider = services.BuildServiceProvider();

            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name => serviceProvider.GetService<WebServer>());
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

