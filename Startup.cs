using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5Service
{
    /// <summary>
    /// OWIN Startup configuration for the Web API
    /// Production-ready template with OWIN + Nancy + DI + Serilog
    /// Features: CORS, Request Logging, Error Handling, Compression, Health Checks, Security Headers
    /// </summary>
    public class Startup
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stopwatch _startupTimer;

        public Startup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _startupTimer = Stopwatch.StartNew();

        }

        public void Configuration(IAppBuilder app)
        {

            // 1. Request ID and Timing Middleware (First)
            app.Use<RequestIdMiddleware>();
            app.Use<RequestTimingMiddleware>();

            // 2. Global Exception Handler (Must be early in pipeline)
            ConfigureGlobalExceptionHandler(app);

            // 3. Request/Response Logging Middleware
            ConfigureRequestLogging(app);

            // 4. Security Headers Middleware
            ConfigureSecurityHeaders(app);

            // 5. CORS Middleware (Before Web API)
            ConfigureCorsMiddleware(app);

            // 6. Compression Middleware (Optional - requires package)
            ConfigureCompression(app);

            // 7. Custom Application Middleware
            app.Use(typeof(CustomMiddleware));

            // 8. Health Check Endpoint (Before Web API routing)
            ConfigureHealthCheck(app);

            // 9. Configure Web API (including WelcomeController for root path)
            var config = new HttpConfiguration();
            ConfigureWebApi(config);
            app.UseWebApi(config);

            // 10. File Server (for static files/Swagger UI)
            ConfigureFileServer(app);

            // 11. Nancy Framework (Last - catches remaining routes)
            ConfigureNancy(app);

            _startupTimer.Stop();
        }

        #region Global Exception Handler

        private void ConfigureGlobalExceptionHandler(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            });

        }

        private async Task HandleExceptionAsync(Microsoft.Owin.IOwinContext context, Exception ex)
        {
            var requestId = context.Environment.ContainsKey("RequestId")
                ? context.Environment["RequestId"]?.ToString()
                : Guid.NewGuid().ToString();


            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                success = false,
                message = "An internal server error occurred",
                requestId = requestId,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value
            };

            var json = JsonConvert.SerializeObject(errorResponse, Formatting.Indented);
            await context.Response.WriteAsync(json);
        }

        #endregion

        #region Request Logging

        private void ConfigureRequestLogging(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var requestId = context.Environment.ContainsKey("RequestId")
                    ? context.Environment["RequestId"]?.ToString()
                    : Guid.NewGuid().ToString();

                var sw = Stopwatch.StartNew();
                var method = context.Request.Method;
                var path = context.Request.Path.Value;
                var queryString = context.Request.QueryString.Value;


                await next.Invoke();

                sw.Stop();
                var statusCode = context.Response.StatusCode;
                var levelPrefix = statusCode >= 500 ? "[ERROR]"
                                : statusCode >= 400 ? "[WARNING]"
                                : "[INFO]";

            });

        }

        #endregion

        #region Security Headers

        private void ConfigureSecurityHeaders(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // Security Headers
                context.Response.Headers["X-Content-Type-Options"] = MT5Constants.SecurityHeaders.XContentTypeOptions;
                context.Response.Headers["X-Frame-Options"] = MT5Constants.SecurityHeaders.XFrameOptions;
                context.Response.Headers["X-XSS-Protection"] = MT5Constants.SecurityHeaders.XXSSProtection;
                context.Response.Headers["Referrer-Policy"] = MT5Constants.SecurityHeaders.ReferrerPolicy;
                context.Response.Headers["Permissions-Policy"] = MT5Constants.SecurityHeaders.PermissionsPolicy;

                // Remove server information
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");

                // Custom headers
                context.Response.Headers["Product"] = MT5Constants.ServiceInfo.ProductHeader;
                context.Response.Headers["X-Powered-By"] = MT5Constants.ServiceInfo.PoweredBy;
                context.Response.Headers["X-Application-Version"] = GetApplicationVersion();

                await next.Invoke();
            });

        }

        private string GetApplicationVersion()
        {
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
            catch
            {
                return MT5Constants.ServiceInfo.Version;
            }
        }

        #endregion

        #region CORS Configuration

        private void ConfigureCorsMiddleware(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var allowedOrigins = MT5Constants.Cors.AllowedOrigins;
                var allowedMethods = MT5Constants.Cors.AllowedMethods;
                var allowedHeaders = MT5Constants.Cors.AllowedHeaders;

                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigins });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { allowedMethods });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { allowedHeaders });
                context.Response.Headers.Add("Access-Control-Max-Age", new[] { MT5Constants.Cors.MaxAgeSeconds });

                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 204;
                    return;
                }

                await next.Invoke();
            });

        }

        #endregion

        #region Compression

        private void ConfigureCompression(IAppBuilder app)
        {
            try
            {
                // Enable compression if available (requires Microsoft.Owin.Compression or similar)
                // app.Use(typeof(CompressionMiddleware));
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Health Check

        private void ConfigureHealthCheck(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals(MT5Constants.HealthCheck.PathSimple, StringComparison.OrdinalIgnoreCase) ||
                    context.Request.Path.Value.Equals(MT5Constants.HealthCheck.PathApi, StringComparison.OrdinalIgnoreCase))
                {
                    var health = new
                    {
                        status = MT5Constants.HealthCheck.StatusHealthy,
                        timestamp = DateTime.UtcNow,
                        uptime = GetUptime(),
                        version = GetApplicationVersion(),
                        environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production",
                        checks = new
                        {
                            owin = MT5Constants.HealthCheck.ComponentOk,
                            webapi = MT5Constants.HealthCheck.ComponentOk,
                            nancy = MT5Constants.HealthCheck.ComponentOk
                        }
                    };

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(health, Formatting.Indented));
                    return;
                }

                await next.Invoke();
            });

        }

        private string GetUptime()
        {
            var uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        #endregion


        #region Web API Configuration

        private void ConfigureWebApi(HttpConfiguration config)
        {
            // Dependency Injection
            config.DependencyResolver = new DependencyResolver(_serviceProvider);

            // JSON Formatter configuration
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            // Remove XML formatter (JSON only API)
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Configure routes
            ConfigureRoutes(config);

            // Message handlers (for authentication, logging, etc.)
            ConfigureMessageHandlers(config);

            // Filters (for authorization, exception handling)
            ConfigureFilters(config);

        }

        private void ConfigureRoutes(HttpConfiguration config)
        {
            // Attribute routing (recommended for modern APIs)
            config.MapHttpAttributeRoutes();

            // Convention-based routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }

        private void ConfigureMessageHandlers(HttpConfiguration config)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void ConfigureFilters(HttpConfiguration config)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region File Server Configuration

        private void ConfigureFileServer(IAppBuilder app)
        {
            try
            {
                var options = new FileServerOptions
                {
                    EnableDirectoryBrowsing = false, // Disabled for security
                    EnableDefaultFiles = true,
                    StaticFileOptions = { ContentTypeProvider = new CustomContentTypeProvider() }
                };
                app.UseFileServer(options);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Nancy Configuration

        private void ConfigureNancy(IAppBuilder app)
        {
            try
            {
                // Nancy should only handle routes that Web API doesn't handle
                // Web API takes priority for /api/* routes
                app.MapWhen(
                    context =>
                    {
                        var path = context.Request.Path.Value;
                        return !path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) &&
                               !path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) &&
                               !path.Equals(MT5Constants.HealthCheck.PathSimple, StringComparison.OrdinalIgnoreCase);
                    },
                    nancyApp => nancyApp.UseNancy()
                );
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }

    #region Custom Middleware Classes

    /// <summary>
    /// Middleware to add unique request ID to each request
    /// </summary>
    public class RequestIdMiddleware : OwinMiddleware
    {
        public RequestIdMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(Microsoft.Owin.IOwinContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            context.Environment["RequestId"] = requestId;
            context.Response.Headers.Add("X-Request-Id", new[] { requestId });
            await Next.Invoke(context);
        }
    }

    /// <summary>
    /// Middleware to track request processing time
    /// </summary>
    public class RequestTimingMiddleware : OwinMiddleware
    {
        public RequestTimingMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(Microsoft.Owin.IOwinContext context)
        {
            var sw = Stopwatch.StartNew();
            await Next.Invoke(context);
            sw.Stop();
            context.Response.Headers.Add("X-Response-Time", new[] { $"{sw.ElapsedMilliseconds}ms" });
        }
    }

    #endregion
}

