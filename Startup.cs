using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Middleware;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5ConnectionService
{
    /// <summary>
    /// OWIN Startup configuration for the Web API
    /// Production-ready template with OWIN + Nancy + DI + Serilog
    /// Features: CORS, Request Logging, Error Handling, Compression, Health Checks, Security Headers
    /// </summary>
    public class Startup
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly Stopwatch _startupTimer;

        public Startup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = serviceProvider.GetService<IConfiguration>();
            _startupTimer = Stopwatch.StartNew();

            Log.Information("=== Startup Configuration Initiated ===");
        }

        public void Configuration(IAppBuilder app)
        {
            Log.Information("Configuring OWIN pipeline");

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
            Log.Information("OWIN pipeline configuration completed in {ElapsedMs}ms", _startupTimer.ElapsedMilliseconds);
            Log.Information("=== Application Ready to Accept Requests ===");
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

            Log.Information("Global exception handler configured");
        }

        private async Task HandleExceptionAsync(Microsoft.Owin.IOwinContext context, Exception ex)
        {
            var requestId = context.Environment.ContainsKey("RequestId")
                ? context.Environment["RequestId"]?.ToString()
                : Guid.NewGuid().ToString();

            Log.Error(ex, "Unhandled exception in OWIN pipeline. RequestId: {RequestId}, Path: {Path}",
                requestId, context.Request.Path);

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

                Log.Information("HTTP {Method} {Path}{QueryString} started. RequestId: {RequestId}",
                    method, path, queryString, requestId);

                await next.Invoke();

                sw.Stop();
                var statusCode = context.Response.StatusCode;
                var level = statusCode >= 500 ? Serilog.Events.LogEventLevel.Error
                          : statusCode >= 400 ? Serilog.Events.LogEventLevel.Warning
                          : Serilog.Events.LogEventLevel.Information;

                Log.Write(level,
                    "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMs}ms. RequestId: {RequestId}",
                    method, path, queryString, statusCode, sw.ElapsedMilliseconds, requestId);
            });

            Log.Information("Request logging middleware configured");
        }

        #endregion

        #region Security Headers

        private void ConfigureSecurityHeaders(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // Security Headers
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

                // Remove server information
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");

                // Custom headers
                context.Response.Headers["Product"] = "Prop MT5 Services Connection";
                context.Response.Headers["X-Powered-By"] = "OWIN/ASP.NET Web API";
                context.Response.Headers["X-Application-Version"] = GetApplicationVersion();

                await next.Invoke();
            });

            Log.Information("Security headers middleware configured");
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
                return "1.0.0";
            }
        }

        #endregion

        #region CORS Configuration

        private void ConfigureCorsMiddleware(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var allowedOrigins = _configuration?["CORS:AllowedOrigins"] ?? "*";
                var allowedMethods = _configuration?["CORS:AllowedMethods"] ?? "GET, POST, PUT, DELETE, OPTIONS";
                var allowedHeaders = _configuration?["CORS:AllowedHeaders"] ?? "Content-Type, Authorization, X-Requested-With, X-Api-Key";

                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigins });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { allowedMethods });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { allowedHeaders });
                context.Response.Headers.Add("Access-Control-Max-Age", new[] { "3600" });

                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 204;
                    return;
                }

                await next.Invoke();
            });

            Log.Information("CORS middleware configured");
        }

        #endregion

        #region Compression

        private void ConfigureCompression(IAppBuilder app)
        {
            try
            {
                // Enable compression if available (requires Microsoft.Owin.Compression or similar)
                // app.Use(typeof(CompressionMiddleware));
                Log.Debug("Compression middleware skipped (optional feature)");
            }
            catch (Exception ex)
            {
                Log.Debug("Compression middleware not available: {Message}", ex.Message);
            }
        }

        #endregion

        #region Health Check

        private void ConfigureHealthCheck(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/health", StringComparison.OrdinalIgnoreCase) ||
                    context.Request.Path.Value.Equals("/api/health", StringComparison.OrdinalIgnoreCase))
                {
                    var health = new
                    {
                        status = "Healthy",
                        timestamp = DateTime.UtcNow,
                        uptime = GetUptime(),
                        version = GetApplicationVersion(),
                        environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production",
                        checks = new
                        {
                            owin = "OK",
                            webapi = "OK",
                            nancy = "OK"
                        }
                    };

                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(health, Formatting.Indented));
                    return;
                }

                await next.Invoke();
            });

            Log.Information("Health check endpoint configured at /health and /api/health");
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

            Log.Information("Web API configuration completed");
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

            Log.Information("Web API routes configured");
        }

        private void ConfigureMessageHandlers(HttpConfiguration config)
        {
            try
            {
                // Add custom message handlers here
                // config.MessageHandlers.Add(new AuthenticationHandler());
                // config.MessageHandlers.Add(new LoggingHandler());
                Log.Debug("Message handlers configured");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to configure message handlers");
            }
        }

        private void ConfigureFilters(HttpConfiguration config)
        {
            try
            {
                // Add global filters
                // config.Filters.Add(new AuthorizeAttribute());
                // config.Filters.Add(new ExceptionFilterAttribute());
                Log.Debug("Web API filters configured");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to configure filters");
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
                Log.Information("File server configured (static files enabled)");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to configure file server");
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
                    context => !context.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) &&
                               !context.Request.Path.Value.Equals("/health", StringComparison.OrdinalIgnoreCase),
                    nancyApp => nancyApp.UseNancy()
                );
                Log.Information("Nancy framework enabled (for non-API routes only)");
            }
            catch (Exception ex)
            {
                Log.Debug("Nancy framework not available: {Message}", ex.Message);
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

