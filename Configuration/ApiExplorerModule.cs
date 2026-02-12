using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace PropMT5ConnectionService.Configuration
{
    /// <summary>
    /// API Explorer Module - Lists all available REST API endpoints
    /// Combines both Nancy and Web API endpoints for comprehensive documentation
    /// </summary>
    public class ApiExplorerModule : NancyModule
    {
        public ApiExplorerModule()
        {
            // Primary API Explorer endpoint (Nancy can handle this)
            Get("/explorer", _ =>
            {
                var html = GenerateApiExplorerPage();
                return Response.AsText(html, "text/html");
            });

            // Alternative route for convenience
            Get("/endpoints", _ =>
            {
                return Response.AsRedirect("/explorer");
            });

            // JSON endpoint for programmatic access (Nancy can handle this)
            Get("/explorer/json", _ =>
            {
                var endpoints = DiscoverAllEndpoints();
                return Response.AsJson(endpoints);
            });
        }

        private object DiscoverAllEndpoints()
        {
            var webApiEndpoints = DiscoverWebApiEndpoints();
            var nancyEndpoints = DiscoverNancyEndpoints();

            return new
            {
                timestamp = DateTime.UtcNow,
                totalEndpoints = webApiEndpoints.Count + nancyEndpoints.Count,
                webApi = new
                {
                    count = webApiEndpoints.Count,
                    endpoints = webApiEndpoints
                },
                nancy = new
                {
                    count = nancyEndpoints.Count,
                    endpoints = nancyEndpoints
                }
            };
        }

        private List<object> DiscoverWebApiEndpoints()
        {
            var endpoints = new List<object>();

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var controllerTypes = assembly.GetTypes()
                    .Where(t => typeof(ApiController).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var controllerType in controllerTypes)
                {
                    var routePrefix = controllerType.GetCustomAttributes(typeof(RoutePrefixAttribute), true)
                        .Cast<RoutePrefixAttribute>()
                        .FirstOrDefault()?.Prefix ?? "";

                    var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.DeclaringType == controllerType);

                    foreach (var method in methods)
                    {
                        var routeAttributes = method.GetCustomAttributes(typeof(RouteAttribute), true)
                            .Cast<RouteAttribute>();

                        var httpMethod = GetHttpMethod(method);
                        if (httpMethod != null)
                        {
                            foreach (var routeAttr in routeAttributes)
                            {
                                var fullRoute = string.IsNullOrEmpty(routePrefix)
                                    ? routeAttr.Template
                                    : $"{routePrefix}/{routeAttr.Template}".TrimEnd('/');

                                endpoints.Add(new
                                {
                                    controller = controllerType.Name.Replace("Controller", ""),
                                    method = httpMethod,
                                    route = "/" + fullRoute.TrimStart('/'),
                                    action = method.Name,
                                    framework = "Web API"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue
                System.Diagnostics.Debug.WriteLine($"Error discovering Web API endpoints: {ex.Message}");
            }

            return endpoints.OrderBy(e => ((dynamic)e).route).ToList();
        }

        private List<object> DiscoverNancyEndpoints()
        {
            var endpoints = new List<object>();

            // Manually add known Nancy endpoints
            // Nancy doesn't provide easy reflection-based discovery, so we list them explicitly
            endpoints.Add(new
            {
                module = "Documentation",
                method = "GET",
                route = "/docs",
                description = "Swagger API documentation UI",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Documentation",
                method = "GET",
                route = "/docs/swagger.json",
                description = "OpenAPI/Swagger JSON specification",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Documentation",
                method = "GET",
                route = "/swagger",
                description = "Swagger UI (redirects to /docs)",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Explorer",
                method = "GET",
                route = "/explorer",
                description = "API endpoint explorer",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Explorer",
                method = "GET",
                route = "/explorer/json",
                description = "JSON export of all endpoints",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Explorer",
                method = "GET",
                route = "/endpoints",
                description = "API endpoints (redirects to /explorer)",
                framework = "Nancy"
            });

            endpoints.Add(new
            {
                module = "Welcome",
                method = "GET",
                route = "/",
                description = "Welcome page",
                framework = "Nancy"
            });

            return endpoints.OrderBy(e => ((dynamic)e).route).ToList();
        }

        private string GetHttpMethod(MethodInfo method)
        {
            if (method.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()) return "GET";
            if (method.GetCustomAttributes(typeof(HttpPostAttribute), true).Any()) return "POST";
            if (method.GetCustomAttributes(typeof(HttpPutAttribute), true).Any()) return "PUT";
            if (method.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Any()) return "DELETE";
            if (method.GetCustomAttributes(typeof(HttpPatchAttribute), true).Any()) return "PATCH";
            return null;
        }

        private string GenerateApiExplorerPage()
        {
            var endpoints = DiscoverAllEndpoints();
            var webApiEndpoints = ((dynamic)endpoints).webApi.endpoints as List<object>;
            var nancyEndpoints = ((dynamic)endpoints).nancy.endpoints as List<object>;

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>API Explorer - Prop MT5 Connection Service</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #333;
            line-height: 1.6;
        }}

        .container {{
            max-width: 1400px;
            margin: 30px auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            overflow: hidden;
        }}

        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px;
            text-align: center;
        }}

        .header h1 {{
            font-size: 2.5em;
            margin-bottom: 10px;
        }}

        .header p {{
            font-size: 1.2em;
            opacity: 0.9;
        }}

        .stats {{
            display: flex;
            justify-content: center;
            gap: 30px;
            margin-top: 20px;
        }}

        .stat-card {{
            background: rgba(255, 255, 255, 0.2);
            padding: 15px 30px;
            border-radius: 10px;
        }}

        .stat-number {{
            font-size: 2em;
            font-weight: bold;
        }}

        .stat-label {{
            font-size: 0.9em;
            opacity: 0.9;
        }}

        .nav {{
            background: #f8f9fa;
            padding: 20px 40px;
            border-bottom: 2px solid #e9ecef;
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
        }}

        .nav-link {{
            padding: 10px 20px;
            background: white;
            border: 2px solid #667eea;
            border-radius: 25px;
            color: #667eea;
            text-decoration: none;
            font-weight: 600;
            transition: all 0.3s;
        }}

        .nav-link:hover {{
            background: #667eea;
            color: white;
        }}

        .content {{
            padding: 40px;
        }}

        .section {{
            margin-bottom: 40px;
        }}

        .section h2 {{
            color: #667eea;
            font-size: 2em;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 3px solid #667eea;
        }}

        .framework-badge {{
            display: inline-block;
            padding: 5px 15px;
            border-radius: 15px;
            font-size: 0.8em;
            font-weight: 600;
            margin-left: 10px;
        }}

        .framework-webapi {{
            background: #007bff;
            color: white;
        }}

        .framework-nancy {{
            background: #28a745;
            color: white;
        }}

        .endpoint-table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background: white;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            border-radius: 8px;
            overflow: hidden;
        }}

        .endpoint-table thead {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }}

        .endpoint-table th {{
            padding: 15px;
            text-align: left;
            font-weight: 600;
        }}

        .endpoint-table td {{
            padding: 15px;
            border-bottom: 1px solid #e9ecef;
        }}

        .endpoint-table tr:hover {{
            background: #f8f9fa;
        }}

        .endpoint-table tbody tr.clickable {{
            cursor: pointer;
        }}

        .endpoint-table tbody tr.clickable:hover {{
            background: #e3f2fd;
        }}

        .details-row {{
            display: none;
            background: #f8f9fa !important;
        }}

        .details-row.visible {{
            display: table-row;
        }}

        .details-content {{
            padding: 20px;
            border-left: 4px solid #667eea;
        }}

        .details-section {{
            margin-bottom: 20px;
        }}

        .details-section h4 {{
            color: #667eea;
            margin-bottom: 10px;
            font-size: 1.1em;
        }}

        .code-example {{
            background: #2d2d2d;
            color: #f8f8f2;
            padding: 15px;
            border-radius: 5px;
            overflow-x: auto;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
            margin-top: 10px;
            white-space: pre-wrap;
        }}

        .param-table {{
            width: 100%;
            margin-top: 10px;
            border-collapse: collapse;
        }}

        .param-table th {{
            background: #667eea;
            color: white;
            padding: 8px;
            text-align: left;
            font-size: 0.9em;
        }}

        .param-table td {{
            padding: 8px;
            border: 1px solid #dee2e6;
            font-size: 0.9em;
        }}

        .expand-icon {{
            font-size: 0.8em;
            margin-left: 8px;
            color: #667eea;
            transition: transform 0.3s;
            display: inline-block;
        }}

        .expand-icon.expanded {{
            transform: rotate(90deg);
        }}

        .response-status {{
            display: inline-block;
            padding: 3px 10px;
            border-radius: 5px;
            font-size: 0.85em;
            font-weight: 600;
            margin-right: 10px;
        }}

        .response-status.success {{
            background: #d4edda;
            color: #155724;
        }}

        .response-status.error {{
            background: #f8d7da;
            color: #721c24;
        }}

        .try-button {{
            padding: 8px 16px;
            background: #667eea;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
            margin-top: 10px;
            transition: all 0.3s;
        }}

        .try-button:hover {{
            background: #5568d3;
            transform: translateY(-2px);
        }}

        .method {{
            padding: 5px 15px;
            border-radius: 5px;
            font-weight: 700;
            font-size: 0.85em;
            color: white;
            display: inline-block;
            min-width: 60px;
            text-align: center;
        }}

        .method.get {{ background: #28a745; }}
        .method.post {{ background: #007bff; }}
        .method.put {{ background: #ffc107; color: #333; }}
        .method.delete {{ background: #dc3545; }}
        .method.patch {{ background: #17a2b8; }}

        .route {{
            font-family: 'Courier New', monospace;
            color: #667eea;
            font-weight: 600;
        }}

        .search-box {{
            width: 100%;
            padding: 15px;
            border: 2px solid #dee2e6;
            border-radius: 8px;
            font-size: 1em;
            margin-bottom: 20px;
        }}

        .search-box:focus {{
            outline: none;
            border-color: #667eea;
        }}

        .filter-buttons {{
            display: flex;
            gap: 10px;
            margin-bottom: 20px;
            flex-wrap: wrap;
        }}

        .filter-btn {{
            padding: 10px 20px;
            border: 2px solid #dee2e6;
            background: white;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
            transition: all 0.3s;
        }}

        .filter-btn.active {{
            background: #667eea;
            color: white;
            border-color: #667eea;
        }}

        .footer {{
            background: #2d2d2d;
            color: white;
            text-align: center;
            padding: 30px;
        }}

        @media (max-width: 768px) {{
            .container {{
                margin: 10px;
            }}

            .header h1 {{
                font-size: 1.8em;
            }}

            .content {{
                padding: 20px;
            }}

            .endpoint-table {{
                font-size: 0.9em;
            }}

            .stats {{
                flex-direction: column;
                gap: 15px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>&#128200; API Explorer</h1>
            <p>Complete REST API Endpoint Reference</p>
            <div class=""stats"">
                <div class=""stat-card"">
                    <div class=""stat-number"">{((dynamic)endpoints).totalEndpoints}</div>
                    <div class=""stat-label"">Total Endpoints</div>
                </div>
                <div class=""stat-card"">
                    <div class=""stat-number"">{((dynamic)endpoints).webApi.count}</div>
                    <div class=""stat-label"">Web API</div>
                </div>
                <div class=""stat-card"">
                    <div class=""stat-number"">{((dynamic)endpoints).nancy.count}</div>
                    <div class=""stat-label"">Nancy</div>
                </div>
            </div>
        </div>

        <div class=""nav"">
            <a href=""/"" class=""nav-link"">&#127968; Home</a>
            <a href=""/docs"" class=""nav-link"">&#128214; Swagger Docs</a>
            <a href=""/api/health"" class=""nav-link"">&#127973; Health Check</a>
            <a href=""/explorer/json"" class=""nav-link"">&#128190; JSON Export</a>
        </div>

        <div class=""content"">
            <div class=""section"">
                <input type=""text"" id=""searchBox"" class=""search-box"" placeholder=""&#128269; Search endpoints by route, controller, or method..."" onkeyup=""filterEndpoints()"">
                
                <div class=""filter-buttons"">
                    <button class=""filter-btn active"" onclick=""filterByFramework('all')"">All ({((dynamic)endpoints).totalEndpoints})</button>
                    <button class=""filter-btn"" onclick=""filterByFramework('webapi')"">Web API ({((dynamic)endpoints).webApi.count})</button>
                    <button class=""filter-btn"" onclick=""filterByFramework('nancy')"">Nancy ({((dynamic)endpoints).nancy.count})</button>
                    <button class=""filter-btn"" onclick=""filterByMethod('GET')"">GET</button>
                    <button class=""filter-btn"" onclick=""filterByMethod('POST')"">POST</button>
                    <button class=""filter-btn"" onclick=""filterByMethod('PUT')"">PUT</button>
                    <button class=""filter-btn"" onclick=""filterByMethod('DELETE')"">DELETE</button>
                </div>
            </div>

            <div class=""section"">
                <h2>&#128225; Web API Endpoints<span class=""framework-badge framework-webapi"">ASP.NET Web API</span></h2>
                <table class=""endpoint-table"" id=""webapiTable"">
                    <thead>
                        <tr>
                            <th>Method</th>
                            <th>Route</th>
                            <th>Controller</th>
                            <th>Action</th>
                            <th style=""text-align: center;"">Details</th>
                        </tr>
                    </thead>
                    <tbody>
{GenerateWebApiRows(webApiEndpoints)}
                    </tbody>
                </table>
            </div>

            <div class=""section"">
                <h2>&#128640; Nancy Endpoints<span class=""framework-badge framework-nancy"">Nancy Framework</span></h2>
                <table class=""endpoint-table"" id=""nancyTable"">
                    <thead>
                        <tr>
                            <th>Method</th>
                            <th>Route</th>
                            <th>Module</th>
                            <th>Description</th>
                            <th style=""text-align: center;"">Details</th>
                        </tr>
                    </thead>
                    <tbody>
{GenerateNancyRows(nancyEndpoints)}
                    </tbody>
                </table>
            </div>
        </div>

        <div class=""footer"">
            <p style=""font-size: 1.2em; margin-bottom: 10px;""><strong>Prop MT5 Connection Service</strong></p>
            <p>API Explorer | Version 1.0.0</p>
            <p style=""margin-top: 15px; opacity: 0.8;"">
                Built with &#10084; for Professional Trading | &copy; 2025
            </p>
        </div>
    </div>

    <script>
        let currentFrameworkFilter = 'all';
        let currentMethodFilter = null;

        function filterEndpoints() {{
            const searchTerm = document.getElementById('searchBox').value.toLowerCase();
            filterTable('webapiTable', searchTerm);
            filterTable('nancyTable', searchTerm);
        }}

        function filterTable(tableId, searchTerm) {{
            const table = document.getElementById(tableId);
            const rows = table.getElementsByTagName('tr');

            for (let i = 1; i < rows.length; i++) {{
                const row = rows[i];
                const text = row.textContent.toLowerCase();
                
                let matchesSearch = searchTerm === '' || text.includes(searchTerm);
                let matchesFramework = currentFrameworkFilter === 'all' || 
                    (currentFrameworkFilter === 'webapi' && tableId === 'webapiTable') ||
                    (currentFrameworkFilter === 'nancy' && tableId === 'nancyTable');
                let matchesMethod = currentMethodFilter === null || text.includes(currentMethodFilter.toLowerCase());

                row.style.display = (matchesSearch && matchesFramework && matchesMethod) ? '' : 'none';
            }}
        }}

        function filterByFramework(framework) {{
            currentFrameworkFilter = framework;
            
            // Update button states
            document.querySelectorAll('.filter-btn').forEach(btn => {{
                btn.classList.remove('active');
            }});
            event.target.classList.add('active');

            const webapiTable = document.getElementById('webapiTable').parentElement.parentElement;
            const nancyTable = document.getElementById('nancyTable').parentElement.parentElement;

            if (framework === 'all') {{
                webapiTable.style.display = 'block';
                nancyTable.style.display = 'block';
            }} else if (framework === 'webapi') {{
                webapiTable.style.display = 'block';
                nancyTable.style.display = 'none';
            }} else if (framework === 'nancy') {{
                webapiTable.style.display = 'none';
                nancyTable.style.display = 'block';
            }}

            filterEndpoints();
        }}

        function filterByMethod(method) {{
            if (currentMethodFilter === method) {{
                currentMethodFilter = null;
                event.target.classList.remove('active');
            }} else {{
                currentMethodFilter = method;
                document.querySelectorAll('.filter-btn').forEach(btn => {{
                    if (btn.textContent.includes('GET') || btn.textContent.includes('POST') || 
                        btn.textContent.includes('PUT') || btn.textContent.includes('DELETE')) {{
                        btn.classList.remove('active');
                    }}
                }});
                event.target.classList.add('active');
            }}
            filterEndpoints();
        }}

        function toggleDetails(rowId) {{
            const detailsRow = document.getElementById('details-' + rowId);
            const icon = document.getElementById('icon-' + rowId);
            
            if (detailsRow.classList.contains('visible')) {{
                detailsRow.classList.remove('visible');
                icon.classList.remove('expanded');
            }} else {{
                detailsRow.classList.add('visible');
                icon.classList.add('expanded');
            }}
        }}

        async function tryEndpoint(route, method) {{
            try {{
                const url = 'http://localhost:8086' + route;
                const response = await fetch(url, {{
                    method: method,
                    headers: {{
                        'Content-Type': 'application/json'
                    }}
                }});

                const contentType = response.headers.get('content-type');
                let data;
                
                if (contentType && contentType.includes('application/json')) {{
                    data = await response.json();
                }} else {{
                    data = await response.text();
                }}

                alert('Response Status: ' + response.status + '\\n\\n' + 
                      'Response Data:\\n' + 
                      (typeof data === 'object' ? JSON.stringify(data, null, 2) : data.substring(0, 500)));
            }} catch (error) {{
                alert('Error: ' + error.message);
            }}
        }}
    </script>
</body>
</html>
";
        }

        private string GenerateWebApiRows(List<object> endpoints)
        {
            var rows = "";
            var rowId = 0;
            foreach (dynamic endpoint in endpoints)
            {
                string methodClass = endpoint.method.ToString().ToLower();
                string route = endpoint.route.ToString();
                string method = endpoint.method.ToString();
                string controller = endpoint.controller.ToString();
                string action = endpoint.action.ToString();

                // Generate request/response examples
                var examples = GenerateRequestResponseExamples(method, route, controller, action);

                rows += $@"
                        <tr class=""clickable"" onclick=""toggleDetails({rowId})"">
                            <td><span class=""method {methodClass}"">{method}</span></td>
                            <td><span class=""route"">{route}</span></td>
                            <td>{controller}</td>
                            <td>{action}</td>
                            <td style=""text-align: center;"">
                                <span class=""expand-icon"" id=""icon-{rowId}"">?</span>
                            </td>
                        </tr>
                        <tr class=""details-row"" id=""details-{rowId}"">
                            <td colspan=""5"">
                                <div class=""details-content"">
                                    {examples}
                                </div>
                            </td>
                        </tr>";
                rowId++;
            }
            return rows;
        }

        private string GenerateNancyRows(List<object> endpoints)
        {
            var rows = "";
            var rowId = 1000; // Start at 1000 to avoid conflicts with Web API rows
            foreach (dynamic endpoint in endpoints)
            {
                string methodClass = endpoint.method.ToString().ToLower();
                string route = endpoint.route.ToString();
                string method = endpoint.method.ToString();
                string module = endpoint.module.ToString();
                string description = endpoint.description.ToString();

                // Generate request/response examples
                var examples = GenerateNancyRequestResponseExamples(method, route, module);

                rows += $@"
                        <tr class=""clickable"" onclick=""toggleDetails({rowId})"">
                            <td><span class=""method {methodClass}"">{method}</span></td>
                            <td><span class=""route"">{route}</span></td>
                            <td>{module}</td>
                            <td>{description}</td>
                            <td style=""text-align: center;"">
                                <span class=""expand-icon"" id=""icon-{rowId}"">?</span>
                            </td>
                        </tr>
                        <tr class=""details-row"" id=""details-{rowId}"">
                            <td colspan=""5"">
                                <div class=""details-content"">
                                    {examples}
                                </div>
                            </td>
                        </tr>";
                rowId++;
            }
            return rows;
        }

        private string GenerateRequestResponseExamples(string method, string route, string controller, string action)
        {
            // Generate example request and response based on endpoint
            var examples = "";

            // Request section
            examples += @"
                <div class=""details-section"">
                    <h4>?? Request</h4>
                    <div><strong>URL:</strong> <code>http://localhost:8086" + route + @"</code></div>
                    <div><strong>Method:</strong> <span class=""method " + method.ToLower() + @""">" + method + @"</span></div>";

            // Add parameters based on route
            if (route.Contains("{"))
            {
                examples += @"
                    <table class=""param-table"">
                        <thead>
                            <tr><th>Parameter</th><th>Type</th><th>Required</th><th>Description</th></tr>
                        </thead>
                        <tbody>";

                // Extract parameters from route
                var paramMatches = System.Text.RegularExpressions.Regex.Matches(route, @"\{([^}]+)\}");
                foreach (System.Text.RegularExpressions.Match match in paramMatches)
                {
                    string paramName = match.Groups[1].Value;
                    examples += $@"
                            <tr>
                                <td><code>{paramName}</code></td>
                                <td>string</td>
                                <td>Yes</td>
                                <td>The {paramName} identifier</td>
                            </tr>";
                }

                examples += @"
                        </tbody>
                    </table>";
            }

            // Add request body example for POST/PUT
            if (method == "POST" || method == "PUT")
            {
                examples += @"
                    <div><strong>Request Body:</strong></div>
                    <div class=""code-example"">{
  ""property1"": ""value1"",
  ""property2"": ""value2""
}</div>";
            }

            examples += @"</div>";

            // Response section
            examples += @"
                <div class=""details-section"">
                    <h4>?? Response</h4>
                    <div>
                        <span class=""response-status success"">200 OK</span>
                        <span style=""color: #666;"">Successful response</span>
                    </div>
                    <div class=""code-example"">";

            // Generate response example based on controller/action
            if (controller.Contains("Health"))
            {
                examples += @"{
  ""status"": ""Healthy"",
  ""timestamp"": ""2025-02-12T10:30:00Z"",
  ""uptime"": ""1d 2h 30m"",
  ""version"": ""1.0.0""
}";
            }
            else if (controller.Contains("Account"))
            {
                examples += @"{
  ""success"": true,
  ""data"": {
    ""login"": 12345,
    ""balance"": 10000.00,
    ""equity"": 10500.00,
    ""credit"": 0.00
  }
}";
            }
            else
            {
                examples += @"{
  ""success"": true,
  ""data"": {},
  ""message"": ""Operation completed successfully""
}";
            }

            examples += @"</div>
                </div>";

            // Try it button
            examples += @"
                <button class=""try-button"" onclick=""tryEndpoint('" + route + @"', '" + method + @"')"">
                    ?? Try it out
                </button>";

            return examples;
        }

        private string GenerateNancyRequestResponseExamples(string method, string route, string module)
        {
            var examples = "";

            // Request section
            examples += @"
                <div class=""details-section"">
                    <h4>?? Request</h4>
                    <div><strong>URL:</strong> <code>http://localhost:8086" + route + @"</code></div>
                    <div><strong>Method:</strong> <span class=""method " + method.ToLower() + @""">" + method + @"</span></div>
                </div>";

            // Response section
            examples += @"
                <div class=""details-section"">
                    <h4>?? Response</h4>
                    <div>
                        <span class=""response-status success"">200 OK</span>
                        <span style=""color: #666;"">Successful response</span>
                    </div>
                    <div class=""code-example"">";

            if (route.Contains("/docs"))
            {
                examples += @"<!-- HTML Swagger UI Page -->";
            }
            else if (route.Contains("/explorer"))
            {
                if (route.EndsWith("json"))
                {
                    examples += @"{
  ""timestamp"": ""2025-02-12T10:30:00Z"",
  ""totalEndpoints"": 50,
  ""webApi"": {
    ""count"": 44,
    ""endpoints"": [...]
  },
  ""nancy"": {
    ""count"": 6,
    ""endpoints"": [...]
  }
}";
                }
                else
                {
                    examples += @"<!-- HTML API Explorer Page -->";
                }
            }
            else if (route == "/")
            {
                examples += @"<!-- HTML Welcome Page -->";
            }
            else
            {
                examples += @"{
  ""status"": ""success"",
  ""data"": {}
}";
            }

            examples += @"</div>
                </div>";

            // Try it button
            examples += @"
                <button class=""try-button"" onclick=""tryEndpoint('" + route + @"', '" + method + @"')"">
                    ?? Try it out
                </button>";

            return examples;
        }
    }
}
