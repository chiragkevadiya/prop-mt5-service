//using Microsoft.Owin.FileSystems;
//using Microsoft.Owin.StaticFiles;
//using MT5ConnectionService.Middleware;
//using Owin;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace MT5ConnectionService
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            // Adding to the pipeline with our own middleware
//            app.Use(async (context, next) =>
//            {
//                // Add Header
//                context.Response.Headers["Product"] = "Prop MT5 Services Connection"; //"Web Api Self Host";

//                // Call next middleware
//                await next.Invoke();
//            });

//            // Custom Middleare
//            app.Use(typeof(CustomMiddleware));

//            // Configure Web API for self-host. 
//            var config = new HttpConfiguration();
//            config.Routes.MapHttpRoute(
//                name: "DefaultApi",
//                routeTemplate: "api/{controller}/{id}",
//                defaults: new { id = RouteParameter.Optional }
//            );

//            // Web Api
//            app.UseWebApi(config);

//            //File Server
//            var options = new FileServerOptions
//            {
//                EnableDirectoryBrowsing = true,
//                EnableDefaultFiles = true,
//                //DefaultFilesOptions = { DefaultFileNames = { "index.html" } },
//                //FileSystem = new PhysicalFileSystem("Assets"),
//                StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() }
//            };

//            app.UseFileServer(options);

//            // Nancy
//            app.UseNancy();
//        }
//    }
//}

using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using MT5ConnectionService.Helper;
using MT5ConnectionService.Middleware;
using Owin;
using System;
using System.Web.Http;
using Microsoft.Extensions.DependencyInjection;
using PropMT5ConnectionService.Helper;

namespace MT5ConnectionService
{
    public class Startup
    {
        private readonly IServiceProvider _serviceProvider;

        public Startup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configuration(IAppBuilder app)
        {
            // Adding to the pipeline with our own middleware
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Product"] = "Prop MT5 Services Connection";
                await next.Invoke();
            });

            // Custom Middleware
            app.Use(typeof(CustomMiddleware));

            // Configure Web API for self-host.
            var config = new HttpConfiguration();

            // Set the dependency resolver to use the .NET Core DI container
            config.DependencyResolver = new ServiceProviderDependencyResolver(_serviceProvider);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Web Api
            app.UseWebApi(config);

            // File Server
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                EnableDefaultFiles = true,
                StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() }
            };
            app.UseFileServer(options);

            // Nancy (Note: This might conflict with Web API routes, so use with care)
            app.UseNancy();
        }
    }
}
