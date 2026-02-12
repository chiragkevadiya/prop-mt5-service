using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PropMT5ConnectionService.Configuration
{
    /// <summary>
    /// Swagger documentation module for Nancy Framework APIs
    /// Provides OpenAPI/Swagger specification and interactive UI
    /// </summary>
    public class NancySwaggerModule : NancyModule
    {
        public NancySwaggerModule()
        {
            // Swagger JSON endpoint (Nancy can handle this)
            Get("/docs/swagger.json", _ =>
            {
                var swaggerSpec = GenerateSwaggerSpec();
                return Response.AsJson(swaggerSpec);
            });

            // Swagger UI endpoint (Nancy can handle this)
            Get("/docs", _ =>
            {
                var html = GenerateSwaggerUI();
                return Response.AsText(html, "text/html");
            });

            // Alternative route for convenience
            Get("/swagger", _ =>
            {
                return Response.AsRedirect("/docs");
            });
        }

        private object GenerateSwaggerSpec()
        {
            var spec = new
            {
                openapi = "3.0.0",
                info = new
                {
                    title = "Prop MT5 Connection Service - Nancy API",
                    description = "Nancy Framework REST API endpoints for MetaTrader 5 operations",
                    version = "1.0.0",
                    contact = new
                    {
                        name = "Amit Kumar",
                        email = "support@propmt5.com"
                    }
                },
                servers = new[]
                {
                    new
                    {
                        url = "http://localhost:8086",
                        description = "Development Server"
                    }
                },
                paths = GeneratePaths(),
                components = new
                {
                    schemas = new Dictionary<string, object>
                    {
                        ["HealthResponse"] = new
                        {
                            type = "object",
                            properties = new Dictionary<string, object>
                            {
                                ["status"] = new { type = "string", example = "Healthy" },
                                ["timestamp"] = new { type = "string", format = "date-time" },
                                ["uptime"] = new { type = "string", example = "0d 0h 5m 30s" },
                                ["version"] = new { type = "string", example = "1.0.0" },
                                ["environment"] = new { type = "string", example = "Production" }
                            }
                        },
                        ["ErrorResponse"] = new
                        {
                            type = "object",
                            properties = new Dictionary<string, object>
                            {
                                ["error"] = new { type = "string" },
                                ["message"] = new { type = "string" },
                                ["statusCode"] = new { type = "integer" }
                            }
                        }
                    }
                }
            };

            return spec;
        }

        private Dictionary<string, object> GeneratePaths()
        {
            var paths = new Dictionary<string, object>();

            // Sample Nancy endpoints - you can expand this based on your actual Nancy modules
            paths["/health"] = new
            {
                get = new
                {
                    tags = new[] { "Health" },
                    summary = "Health check endpoint",
                    description = "Returns service health status and diagnostics",
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new
                        {
                            description = "Successful response",
                            content = new Dictionary<string, object>
                            {
                                ["application/json"] = new
                                {
                                    schema = new { @ref = "#/components/schemas/HealthResponse" }
                                }
                            }
                        }
                    }
                }
            };

            paths["/api/health"] = new
            {
                get = new
                {
                    tags = new[] { "Health" },
                    summary = "API health check",
                    description = "Returns detailed health information",
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new
                        {
                            description = "Service is healthy",
                            content = new Dictionary<string, object>
                            {
                                ["application/json"] = new
                                {
                                    schema = new { @ref = "#/components/schemas/HealthResponse" }
                                }
                            }
                        }
                    }
                }
            };

            // Add more Nancy endpoints here
            // You can dynamically discover Nancy modules if needed

            return paths;
        }

        private string GenerateSwaggerUI()
        {
            return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Prop MT5 API Documentation - Nancy Framework</title>
    <link rel=""stylesheet"" href=""https://unpkg.com/swagger-ui-dist@5.11.0/swagger-ui.css"" />
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .topbar {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 15px 30px;
            color: white;
            display: flex;
            align-items: center;
            justify-content: space-between;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .topbar h1 {
            margin: 0;
            font-size: 1.5em;
        }

        .topbar .badge {
            background: rgba(255, 255, 255, 0.2);
            padding: 5px 15px;
            border-radius: 15px;
            font-size: 0.9em;
        }

        #swagger-ui {
            max-width: 1400px;
            margin: 0 auto;
        }

        .swagger-ui .topbar {
            display: none;
        }

        .info-section {
            background: #f8f9fa;
            padding: 30px;
            margin: 20px 0;
            border-radius: 10px;
            border-left: 5px solid #667eea;
        }

        .info-section h2 {
            color: #667eea;
            margin-top: 0;
        }

        .endpoint-list {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }

        .endpoint-card {
            background: white;
            padding: 20px;
            border-radius: 8px;
            border-left: 4px solid #28a745;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .endpoint-card h3 {
            margin: 0 0 10px 0;
            color: #667eea;
            font-size: 1.1em;
        }

        .endpoint-card .method {
            display: inline-block;
            padding: 3px 10px;
            background: #28a745;
            color: white;
            border-radius: 5px;
            font-size: 0.8em;
            font-weight: 600;
        }

        .endpoint-card .path {
            font-family: 'Courier New', monospace;
            color: #666;
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <div class=""topbar"">
        <h1>?? Prop MT5 API Documentation</h1>
        <div>
            <span class=""badge"">Nancy Framework</span>
            <span class=""badge"">Version 1.0.0</span>
        </div>
    </div>

    <div style=""max-width: 1400px; margin: 0 auto; padding: 20px;"">
        <div class=""info-section"">
            <h2>?? Nancy Framework REST API</h2>
            <p>
                This documentation covers Nancy Framework endpoints running on the Prop MT5 Connection Service.
                Nancy is a lightweight web framework for building HTTP-based services on .NET.
            </p>

            <div class=""endpoint-list"">
                <div class=""endpoint-card"">
                    <h3><span class=""method"">GET</span> Health Check</h3>
                    <div class=""path"">/health</div>
                    <p style=""margin-top: 10px; color: #666; font-size: 0.9em;"">
                        Service health status and diagnostics
                    </p>
                </div>

                <div class=""endpoint-card"">
                    <h3><span class=""method"">GET</span> API Health</h3>
                    <div class=""path"">/api/health</div>
                    <p style=""margin-top: 10px; color: #666; font-size: 0.9em;"">
                        Detailed health information
                    </p>
                </div>

                <div class=""endpoint-card"">
                    <h3><span class=""method"">GET</span> Documentation</h3>
                    <div class=""path"">/docs</div>
                    <p style=""margin-top: 10px; color: #666; font-size: 0.9em;"">
                        Interactive API documentation (this page)
                    </p>
                </div>
            </div>
        </div>
    </div>

    <div id=""swagger-ui""></div>

    <div style=""text-align: center; padding: 30px; color: #666; border-top: 2px solid #e9ecef; margin-top: 40px;"">
        <p><strong>Prop MT5 Connection Service</strong></p>
        <p>Developed by Amit Kumar | Version 1.0.0</p>
        <p style=""margin-top: 10px; font-size: 0.9em;"">
            Built with Nancy Framework + OWIN | © 2025
        </p>
    </div>

    <script src=""https://unpkg.com/swagger-ui-dist@5.11.0/swagger-ui-bundle.js""></script>
    <script src=""https://unpkg.com/swagger-ui-dist@5.11.0/swagger-ui-standalone-preset.js""></script>
    <script>
        window.onload = function() {
            const ui = SwaggerUIBundle({
                url: '/docs/swagger.json',
                dom_id: '#swagger-ui',
                deepLinking: true,
                presets: [
                    SwaggerUIBundle.presets.apis,
                    SwaggerUIStandalonePreset
                ],
                plugins: [
                    SwaggerUIBundle.plugins.DownloadUrl
                ],
                layout: 'StandaloneLayout',
                defaultModelsExpandDepth: 1,
                defaultModelExpandDepth: 1,
                docExpansion: 'list',
                filter: true,
                tryItOutEnabled: true
            });

            window.ui = ui;
        }
    </script>
</body>
</html>
";
        }
    }
}
