using Microsoft.Owin.StaticFiles;
using PropMT5ConnectionService.Middleware;
using Owin;
using PropMT5ConnectionService.Helpers;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService
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
            config.DependencyResolver = new DependencyResolver(_serviceProvider);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Swagger (Swashbuckle) can be enabled here for Web API documentation.
            // To enable Swagger UI, install the Swashbuckle NuGet package for Web API
            // (for example: "Install-Package Swashbuckle -Version 5.6.0") and then add
            // the following code back into this method:
            //
            // config.EnableSwagger(c =>
            // {
            //     c.SingleApiVersion("v1", "Prop MT5 API");
            //     c.PrettyPrint();
            // })
            // .EnableSwaggerUi(c =>
            // {
            //     c.DocumentTitle = "Prop MT5 API Documentation";
            // });
            //
            // Leaving this commented avoids compile errors when the package is not installed.

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
