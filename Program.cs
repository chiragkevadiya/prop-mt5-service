//using MetaQuotes.MT5ManagerAPI;
//using MT5ConnectionService.Helper;
//using PropMT5ConnectionService.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;
//using Topshelf;

//namespace MT5ConnectionService
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            StartTopshelf();
//        }

//        static void StartTopshelf()
//        {
//            HostFactory.Run(x =>
//            {
//                x.Service<WebServer>(s =>
//                {
//                    s.ConstructUsing(name => new WebServer());
//                    s.WhenStarted(tc => tc.Start());
//                    s.WhenStopped(tc => tc.Stop());
//                });
//                x.RunAsLocalSystem();

//                //x.SetDescription("This is a demo of a Windows Service using Topshelf.");
//                //x.SetDisplayName("Self Host Web API Demo");
//                //x.SetServiceName("AspNetSelfHostDemo");

//                x.SetDescription("This service manages connections to MT5 (MetaTrader 5).");
//                x.SetDisplayName("Prop MT5 Connection Service");
//                x.SetServiceName("Prop MT5 Connection Service");

//                //x.RunAsLocalService();

//            });
//        }
//    }
//}


using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT5ConnectionService.ClientMT5;
using MT5ConnectionService.Helper;
using PropMT5ConnectionService.Data;
using PropMT5ConnectionService.Services;
using System;
using System.IO;
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
            // Determine environment (default: Production)
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if(environment == null)
            {
                environment ="Development"; 
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

            // Register DbContext
            services.AddDbContext<PropTradingDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register services
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddScoped<ILiqudationService, LiquidationService>();
            services.AddSingleton<CIMTManagerAPI>(provider =>
            {
                // 1. Instantiate your connection class
                var connector = new ClientConnect();

                // 2. Initialize the API factory
                MTRetCode initResult = connector.Initialize();
                if (initResult != MTRetCode.MT_RET_OK)
                {
                    // Log the severe failure and throw to stop the application startup
                    throw new InvalidOperationException($"FATAL: MT5 API Factory initialization failed: {initResult}");
                }

                // 3. Connect/Login (Credentials should come from IConfiguration!)
                var configurationManager = provider.GetRequiredService<IConfiguration>();

                string server = "37.27.232.54:1950";
                ulong login = 1000;
                string password = "Rock@1000";
                uint timeout = 30000;

                MTRetCode connectResult = connector.Connect(server, login, password, timeout);

                if (connectResult != MTRetCode.MT_RET_OK)
                {
                    // Log the failure, clean up, and throw to stop the application startup
                    if (connector.m_manager != null)
                        connector.m_manager.Release();

                    throw new InvalidOperationException($"FATAL: MT5 Manager Login failed: {connectResult}");
                }

                // This is the successfully initialized and logged-in CIMTManagerAPI instance
                // that the DI container will provide to all services requesting it.
                return connector.m_manager;
            });

            // Build provider
            var serviceProvider = services.BuildServiceProvider();

            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name =>
                        new WebServer(serviceProvider.GetRequiredService<ILiqudationService>())
                    );
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
