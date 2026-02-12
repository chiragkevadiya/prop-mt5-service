using Microsoft.Owin.StaticFiles;
using MT5ConnectionService.Middleware;
using Owin;
using PropMT5ConnectionService.Helper;
using System;
using System.Web.Http;

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

            app.UseNancy();
        }
    }
}
