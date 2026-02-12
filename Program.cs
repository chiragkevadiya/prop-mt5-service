using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropMT5ConnectionService.Mt5Client;
using PropMT5ConnectionService.Services;
using Serilog;
using System;
using System.Linq;
using Topshelf;

namespace PropMT5ConnectionService
{
    /// <summary>
    /// Main entry point for Prop MT5 Connection Service
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            ConfigureSerilog();
            StartTopshelf();
        }

        /// <summary>
        /// Configure Serilog logging framework
        /// Note: Requires Serilog.Sinks.Console and Serilog.Sinks.File packages for full functionality
        /// </summary>
        static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .CreateLogger();

            Console.WriteLine("=== Prop MT5 Connection Service Starting ===");
        }

        /// <summary>
        /// Start the Topshelf Windows Service host
        /// </summary>
        static void StartTopshelf()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
                Console.WriteLine($"[INFO] Running in {environment} environment");

                var services = new ServiceCollection();
                ConfigureServices(services, environment);

                var serviceProvider = services.BuildServiceProvider();

                HostFactory.Run(x =>
                {
                    x.Service<WebServer>(s =>
                    {
                        s.ConstructUsing(name => serviceProvider.GetRequiredService<WebServer>());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc =>
                        {
                            tc.Stop();
                            Console.WriteLine("[INFO] Service stopped");
                            Log.CloseAndFlush();
                        });
                    });

                    x.RunAsLocalSystem();
                    x.SetDescription("Manages connections to MT5 (MetaTrader 5) servers for trading operations");
                    x.SetDisplayName("Prop MT5 Connection Service");
                    x.SetServiceName("PropMT5ConnectionService");
                    x.StartAutomatically();

                    x.EnableServiceRecovery(rc =>
                    {
                        rc.RestartService(1);
                        rc.SetResetPeriod(1);
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Application terminated unexpectedly: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Log.CloseAndFlush();
                throw;
            }
        }

        /// <summary>
        /// Configure dependency injection services
        /// </summary>
        static void ConfigureServices(IServiceCollection services, string environment)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(Log.Logger);

            RegisterMT5Clients(services);
            RegisterApplicationServices(services);

            Console.WriteLine("[INFO] Service configuration completed");
        }

        /// <summary>
        /// Register MT5 Live and Demo clients
        /// </summary>
        static void RegisterMT5Clients(IServiceCollection services)
        {
            services.AddSingleton<Mt5LiveClient>(provider => CreateMT5LiveClient(provider));
            services.AddSingleton<Mt5DemoClient>(provider => CreateMT5DemoClient(provider));

            // Register CIMTManagerAPI from Live Client for controllers
            services.AddSingleton(provider =>
            {
                var liveClient = provider.GetRequiredService<Mt5LiveClient>();
                var manager = liveClient.GetManager();
                if (manager == null)
                {
                    throw new InvalidOperationException("MT5 Live Client Manager is not initialized");
                }
                return manager;
            });
        }

        /// <summary>
        /// Create and configure MT5 Live Client
        /// </summary>
        static Mt5LiveClient CreateMT5LiveClient(IServiceProvider provider)
        {
            var logger = provider.GetRequiredService<ILogger>();
            var config = provider.GetRequiredService<IConfiguration>();

            var libraryPath = config["MT5:LibraryPath"] ?? @"C:\dll_dot\MT5Libs";
            var client = new Mt5LiveClient(logger, libraryPath);

            var initResult = client.Initialize();
            if (initResult != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine($"[FATAL] MT5 Live Client initialization failed: {initResult}");
                throw new InvalidOperationException($"Failed to initialize MT5 Live Client: {initResult}");
            }

            var server = config["MT5:Live:Server"];
            var login = ulong.Parse(config["MT5:Live:Login"]);
            var password = config["MT5:Live:Password"];
            var timeout = uint.Parse(config["MT5:Live:Timeout"] ?? "30000");

            Console.WriteLine($"[INFO] Connecting to MT5 Live server {server} with login {login}");

            var connectResult = client.Connect(server, login, password, timeout);
            if (connectResult != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine($"[FATAL] MT5 Live Client connection failed: {connectResult}");
                client.Dispose();
                throw new InvalidOperationException($"Failed to connect MT5 Live Client: {connectResult}");
            }

            Console.WriteLine("[INFO] Successfully connected to MT5 Live server");
            return client;
        }

        /// <summary>
        /// Create and configure MT5 Demo Client (optional)
        /// </summary>
        static Mt5DemoClient CreateMT5DemoClient(IServiceProvider provider)
        {
            var logger = provider.GetRequiredService<ILogger>();
            var config = provider.GetRequiredService<IConfiguration>();

            var demoServer = config["MT5:Demo:Server"];
            if (string.IsNullOrEmpty(demoServer))
            {
                Console.WriteLine("[INFO] Demo MT5 server not configured, skipping");
                return null;
            }

            var libraryPath = config["MT5:LibraryPath"] ?? @"C:\dll_dot\MT5Libs";
            var client = new Mt5DemoClient(logger, libraryPath);

            var initResult = client.Initialize();
            if (initResult != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine($"[WARNING] MT5 Demo Client initialization failed: {initResult}");
                client?.Dispose();
                return null;
            }

            var login = ulong.Parse(config["MT5:Demo:Login"]);
            var password = config["MT5:Demo:Password"];
            var timeout = uint.Parse(config["MT5:Demo:Timeout"] ?? "30000");

            var connectResult = client.Connect(demoServer, login, password, timeout);
            if (connectResult == MTRetCode.MT_RET_OK)
            {
                Console.WriteLine("[INFO] Successfully connected to MT5 Demo server");
                return client;
            }

            Console.WriteLine($"[WARNING] MT5 Demo Client connection failed: {connectResult}");
            client?.Dispose();
            return null;
        }

        /// <summary>
        /// Register application services
        /// </summary>
        static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddSingleton<ICachingService, MemoryCachingService>();
            services.AddSingleton<IPerformanceMonitoringService, PerformanceMonitoringService>();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddScoped<ILiquidationService, LiquidationService>();
            services.AddScoped<IMT5AccountService, MT5AccountService>();
            services.AddScoped<IMT5TradingService, MT5TradingService>();
            services.AddSingleton<WebServer>();

            // Register all API controllers
            RegisterControllers(services);
        }

        /// <summary>
        /// Register all Web API controllers in the DI container
        /// </summary>
        static void RegisterControllers(IServiceCollection services)
        {
            // Get all controller types from the assembly
            var controllerTypes = typeof(Program).Assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                              typeof(System.Web.Http.ApiController).IsAssignableFrom(type));

            foreach (var controllerType in controllerTypes)
            {
                services.AddTransient(controllerType);
                Console.WriteLine($"[INFO] Registered controller: {controllerType.Name}");
            }
        }
    }
}
