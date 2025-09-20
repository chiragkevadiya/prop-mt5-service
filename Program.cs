using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name => new WebServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                //x.SetDescription("This is a demo of a Windows Service using Topshelf.");
                //x.SetDisplayName("Self Host Web API Demo");
                //x.SetServiceName("AspNetSelfHostDemo");

                x.SetDescription("This service manages connections to MT5 (MetaTrader 5).");
                x.SetDisplayName("Prop MT5 Connection Service");
                x.SetServiceName("Prop MT5 Connection Service");

                //x.RunAsLocalService();

            });
        }
    }
}
