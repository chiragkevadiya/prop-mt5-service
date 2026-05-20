using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.DependencyInjection;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.Mt5Client;
using PropMT5Service.Services;
using System;
using System.Linq;
using Topshelf;

namespace PropMT5Service
{
    /// <summary>
    /// Main entry point for MT5 Contest Service
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            StartTopshelf();
        }

        /// <summary>
        /// Start the Topshelf Windows Service host
        /// </summary>
        static void StartTopshelf()
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);

                var serviceProvider = services.BuildServiceProvider();

                HostFactory.Run(x =>
                {
                    x.Service<WebServer>(s =>
                    {
                        s.ConstructUsing(name => serviceProvider.GetRequiredService<WebServer>());
                        s.WhenStarted((WebServer tc) => tc.Start());
                        s.WhenStopped((WebServer tc) =>
                        {
                            tc.Stop();
                            });
                    });

                    x.OnException(ex =>
                    {
                        // Silent failure - log to file via FileLoggingService if needed
                        Console.ReadKey();
                    });

                    x.RunAsLocalSystem();
                    x.SetDescription(MT5Constants.ServiceInfo.Description);
                    x.SetDisplayName(MT5Constants.ServiceInfo.DisplayName);
                    x.SetServiceName(MT5Constants.ServiceInfo.Name);
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
                throw;
            }
        }

        /// <summary>
        /// Configure dependency injection services
        /// </summary>
        static void ConfigureServices(IServiceCollection services)
        {
            RegisterMT5Clients(services);
            RegisterApplicationServices(services);

        }

        /// <summary>
        /// Register MT5 Live and Demo clients
        /// </summary>
        static void RegisterMT5Clients(IServiceCollection services)
        {
            services.AddSingleton<Mt5LiveClient>(provider => CreateMT5LiveClient(provider));

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
            var client = new Mt5LiveClient(MT5Constants.MT5Live.LibraryPath);

            var initResult = client.Initialize();
            if (initResult != MTRetCode.MT_RET_OK)
            {
                throw new InvalidOperationException($"Failed to initialize MT5 Live Client: {initResult}");
            }


            var connectResult = client.Connect(
                MT5Constants.MT5Live.Server,
                MT5Constants.MT5Live.Login,
                MT5Constants.MT5Live.Password,
                MT5Constants.MT5Live.Timeout);

            if (connectResult != MTRetCode.MT_RET_OK)
            {
                client.Dispose();
                throw new InvalidOperationException($"Failed to connect MT5 Live Client: {connectResult}");
            }

            Console.WriteLine("[INFO] Successfully connected to MT5 Live server");
            return client;
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
            }
        }
    }
}
