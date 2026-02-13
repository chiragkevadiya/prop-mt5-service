using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace PropMT5ConnectionService.Configuration
{
    /// <summary>
    /// API Explorer Module - Professional Swagger-style API documentation
    /// Lists all available REST API endpoints with interactive UI
    /// </summary>
    public class ApiExplorerModule : NancyModule
    {
        public ApiExplorerModule()
        {
            // Primary API Explorer endpoint
            Get("/explorer", _ =>
            {
                var html = GenerateApiExplorerPage();
                return Response.AsText(html, "text/html");
            });
        }

        private List<EndpointInfo> DiscoverAllEndpoints()
        {
            var endpoints = new List<EndpointInfo>();

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

                                var parameters = method.GetParameters()
                                    .Select(p => new ParameterInfo
                                    {
                                        Name = p.Name,
                                        Type = p.ParameterType.Name,
                                        Required = !p.IsOptional
                                    }).ToList();

                                endpoints.Add(new EndpointInfo
                                {
                                    Controller = controllerType.Name.Replace("Controller", ""),
                                    Method = httpMethod,
                                    Route = "/" + fullRoute.TrimStart('/'),
                                    Action = method.Name,
                                    Parameters = parameters,
                                    Description = GetMethodDescription(method)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error discovering endpoints: {ex.Message}");
            }

            return endpoints.OrderBy(e => e.Route).ToList();
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

        private string GetMethodDescription(MethodInfo method)
        {
            // Try to get XML documentation or use method name
            return method.Name.Replace("Get", "Get ")
                             .Replace("Post", "Create ")
                             .Replace("Put", "Update ")
                             .Replace("Delete", "Delete ");
        }

        private string GenerateApiExplorerPage()
        {
            var endpoints = DiscoverAllEndpoints();
            var groupedEndpoints = endpoints.GroupBy(e => e.Controller).OrderBy(g => g.Key);

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>API Explorer - Swagger Style</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            background: #fafafa;
            color: #3b4151;
            line-height: 1.6;
        }}

        .swagger-ui {{
            max-width: 1460px;
            margin: 0 auto;
        }}

        .topbar {{
            background: #1b1b1b;
            padding: 15px 40px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }}

        .topbar-logo {{
            color: #49cc90;
            font-size: 1.8em;
            font-weight: bold;
        }}

        .topbar-nav {{
            display: flex;
            gap: 20px;
        }}

        .topbar-link {{
            color: white;
            text-decoration: none;
            padding: 8px 16px;
            border-radius: 4px;
            transition: background 0.3s;
        }}

        .topbar-link:hover {{
            background: #2a2a2a;
        }}

        .info-section {{
            background: white;
            padding: 40px;
            border-bottom: 1px solid #e8e8e8;
        }}

        .info-title {{
            font-size: 2.5em;
            color: #3b4151;
            margin-bottom: 10px;
        }}

        .info-description {{
            font-size: 1.1em;
            color: #3b4151;
            opacity: 0.8;
            margin-bottom: 20px;
        }}

        .info-metadata {{
            display: flex;
            gap: 30px;
            margin-top: 20px;
        }}

        .info-item {{
            display: flex;
            flex-direction: column;
        }}

        .info-label {{
            font-size: 0.85em;
            color: #999;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}

        .info-value {{
            font-size: 1.2em;
            color: #3b4151;
            font-weight: 600;
        }}

        .filter-section {{
            background: white;
            padding: 20px 40px;
            border-bottom: 1px solid #e8e8e8;
            position: sticky;
            top: 0;
            z-index: 100;
        }}

        .search-input {{
            width: 100%;
            padding: 12px 20px;
            border: 1px solid #d9d9d9;
            border-radius: 4px;
            font-size: 1em;
            transition: border-color 0.3s;
        }}

        .search-input:focus {{
            outline: none;
            border-color: #49cc90;
        }}

        .filter-tags {{
            display: flex;
            gap: 10px;
            margin-top: 15px;
            flex-wrap: wrap;
        }}

        .filter-tag {{
            padding: 6px 14px;
            background: white;
            border: 1px solid #d9d9d9;
            border-radius: 20px;
            font-size: 0.9em;
            cursor: pointer;
            transition: all 0.3s;
        }}

        .filter-tag:hover {{
            border-color: #49cc90;
            color: #49cc90;
        }}

        .filter-tag.active {{
            background: #49cc90;
            color: white;
            border-color: #49cc90;
        }}

        .operations-section {{
            background: white;
            padding: 40px;
        }}

        .operation-group {{
            margin-bottom: 40px;
        }}

        .group-header {{
            background: #fafafa;
            padding: 15px 20px;
            border-left: 4px solid #49cc90;
            margin-bottom: 15px;
            cursor: pointer;
            transition: background 0.3s;
        }}

        .group-header:hover {{
            background: #f0f0f0;
        }}

        .group-title {{
            font-size: 1.5em;
            color: #3b4151;
            font-weight: 600;
        }}

        .group-count {{
            font-size: 0.9em;
            color: #999;
            margin-left: 10px;
        }}

        .operation {{
            border: 1px solid #e8e8e8;
            border-radius: 4px;
            margin-bottom: 10px;
            overflow: hidden;
            transition: all 0.3s;
            background: white;
        }}

        .operation:hover {{
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }}

        .operation-header {{
            display: flex;
            align-items: center;
            padding: 15px 20px;
            cursor: pointer;
            transition: background 0.3s;
            background: white;
            gap: 10px;
            min-height: 60px;
        }}

        .operation-header:hover {{
            background: #fafafa;
        }}

        .http-method {{
            padding: 6px 14px;
            border-radius: 4px;
            font-weight: 700;
            font-size: 0.85em;
            color: white;
            min-width: 70px;
            text-align: center;
            text-transform: uppercase;
            flex-shrink: 0;
        }}

        .method-get {{ background: #49cc90; }}
        .method-post {{ background: #61affe; }}
        .method-put {{ background: #fca130; }}
        .method-delete {{ background: #f93e3e; }}
        .method-patch {{ background: #50e3c2; }}

        .operation-path {{
            flex: 1;
            font-family: 'Monaco', 'Courier New', monospace;
            font-size: 1em;
            color: #3b4151;
            margin: 0 20px;
            font-weight: 500;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }}

        .operation-summary {{
            color: #999;
            font-size: 0.9em;
            white-space: nowrap;
            margin-right: 15px;
            max-width: 300px;
            overflow: hidden;
            text-overflow: ellipsis;
        }}

        .expand-arrow {{
            width: 20px;
            height: 20px;
            transition: transform 0.3s;
            flex-shrink: 0;
            margin-left: auto;
            cursor: pointer;
        }}

        .expand-arrow.expanded {{
            transform: rotate(180deg);
        }}

        .operation-details {{
            display: none;
            border-top: 1px solid #e8e8e8;
            background: #fafafa;
        }}

        .operation-details.visible {{
            display: block;
        }}

        .details-content {{
            padding: 30px;
        }}

        .section-title {{
            font-size: 1.2em;
            color: #3b4151;
            font-weight: 600;
            margin-bottom: 15px;
        }}

        .parameters-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 30px;
            background: white;
            border-radius: 4px;
            overflow: hidden;
        }}

        .parameters-table th {{
            background: #e8e8e8;
            padding: 12px;
            text-align: left;
            font-weight: 600;
            font-size: 0.9em;
            color: #3b4151;
        }}

        .parameters-table td {{
            padding: 12px;
            border-bottom: 1px solid #e8e8e8;
            font-size: 0.9em;
        }}

        .param-name {{
            font-family: 'Monaco', 'Courier New', monospace;
            color: #3b4151;
            font-weight: 600;
        }}

        .param-type {{
            color: #999;
            font-family: 'Monaco', 'Courier New', monospace;
        }}

        .param-required {{
            color: #f93e3e;
            font-weight: 600;
            font-size: 0.8em;
        }}

        .response-examples {{
            margin-bottom: 30px;
        }}

        .response-code {{
            display: inline-block;
            padding: 4px 10px;
            border-radius: 4px;
            font-weight: 600;
            font-size: 0.85em;
            margin-right: 10px;
        }}

        .code-200 {{ background: #d4edda; color: #155724; }}
        .code-400 {{ background: #fff3cd; color: #856404; }}
        .code-404 {{ background: #f8d7da; color: #721c24; }}
        .code-500 {{ background: #f8d7da; color: #721c24; }}

        .code-block {{
            background: #2d2d2d;
            color: #f8f8f2;
            padding: 20px;
            border-radius: 4px;
            overflow-x: auto;
            font-family: 'Monaco', 'Courier New', monospace;
            font-size: 0.85em;
            line-height: 1.5;
            margin-top: 10px;
        }}

        .try-section {{
            background: white;
            padding: 20px;
            border-radius: 4px;
            margin-bottom: 20px;
        }}

        .try-button {{
            background: #4990e2;
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 4px;
            font-size: 1em;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }}

        .try-button:hover {{
            background: #357abd;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(73, 144, 226, 0.3);
        }}

        .no-results {{
            text-align: center;
            padding: 60px 20px;
            color: #999;
        }}

        .no-results-icon {{
            font-size: 4em;
            margin-bottom: 20px;
        }}

        footer {{
            background: #1b1b1b;
            color: white;
            text-align: center;
            padding: 30px;
            margin-top: 40px;
        }}

        @media (max-width: 768px) {{
            .topbar {{
                flex-direction: column;
                gap: 15px;
            }}

            .info-section {{
                padding: 20px;
            }}

            .operations-section {{
                padding: 20px;
            }}

            .operation-header {{
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }}

            .info-metadata {{
                flex-direction: column;
                gap: 15px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""swagger-ui"">
        <!-- Top Navigation Bar -->
        <div class=""topbar"">
            <div class=""topbar-logo"">
                &#9889; API Explorer
            </div>
            <div class=""topbar-nav"">
                <a href=""/welcome"" class=""topbar-link"">&#127968; Home</a>
            </div>
        </div>

        <!-- API Info Section -->
        <div class=""info-section"">
            <h1 class=""info-title"">Prop MT5 Connection Service API</h1>
            <p class=""info-description"">
                Complete REST API documentation for MetaTrader 5 operations including accounts, trading, positions, orders, and analytics.
            </p>
            <div class=""info-metadata"">
                <div class=""info-item"">
                    <span class=""info-label"">Version</span>
                    <span class=""info-value"">1.0.0</span>
                </div>
                <div class=""info-item"">
                    <span class=""info-label"">Base URL</span>
                    <span class=""info-value"">http://localhost:8086</span>
                </div>
                <div class=""info-item"">
                    <span class=""info-label"">Total Endpoints</span>
                    <span class=""info-value"">{endpoints.Count}</span>
                </div>
                <div class=""info-item"">
                    <span class=""info-label"">Framework</span>
                    <span class=""info-value"">ASP.NET Web API</span>
                </div>
            </div>
        </div>

        <!-- Filter Section -->
        <div class=""filter-section"">
            <input type=""text"" id=""searchInput"" class=""search-input"" placeholder=""Search endpoints..."" onkeyup=""filterOperations()"">
            <div class=""filter-tags"">
                <span class=""filter-tag active"" onclick=""filterByMethod('ALL')"">All ({endpoints.Count})</span>
                <span class=""filter-tag"" onclick=""filterByMethod('GET')"">GET</span>
                <span class=""filter-tag"" onclick=""filterByMethod('POST')"">POST</span>
                <span class=""filter-tag"" onclick=""filterByMethod('PUT')"">PUT</span>
                <span class=""filter-tag"" onclick=""filterByMethod('DELETE')"">DELETE</span>
            </div>
        </div>

        <!-- Operations Section -->
        <div class=""operations-section"">
            {GenerateGroupsHtml(groupedEndpoints)}
        </div>

        <!-- Footer -->
        <footer>
            <p>© 2026 Prop MT5 Connection Service | Built with ASP.NET Web API & Nancy Framework</p>
            <p style=""margin-top: 10px; opacity: 0.7;"">Author: Amit Kumar | Version: 1.0.0</p>
        </footer>
    </div>

    <script>
        function toggleOperation(operationId) {{
            var details = document.getElementById('details-' + operationId);
            var arrow = document.getElementById('arrow-' + operationId);
            
            if (details && arrow) {{
                if (details.classList.contains('visible')) {{
                    details.classList.remove('visible');
                    arrow.classList.remove('expanded');
                }} else {{
                    details.classList.add('visible');
                    arrow.classList.add('expanded');
                }}
            }}
        }}

        function toggleGroup(groupId) {{
            var operations = document.querySelectorAll('.operation[data-group=""' + groupId + '""]');
            var anyVisible = false;
            
            for (var i = 0; i < operations.length; i++) {{
                if (operations[i].style.display !== 'none') {{
                    anyVisible = true;
                    break;
                }}
            }}

            for (var i = 0; i < operations.length; i++) {{
                operations[i].style.display = anyVisible ? 'none' : 'block';
            }}
        }}

        function filterOperations() {{
            var searchValue = document.getElementById('searchInput').value.toLowerCase();
            var operations = document.querySelectorAll('.operation');
            var visibleCount = 0;

            for (var i = 0; i < operations.length; i++) {{
                var operation = operations[i];
                var path = operation.querySelector('.operation-path').textContent.toLowerCase();
                var method = operation.querySelector('.http-method').textContent.toLowerCase();
                var summaryEl = operation.querySelector('.operation-summary');
                var summary = summaryEl ? summaryEl.textContent.toLowerCase() : '';
                
                if (path.indexOf(searchValue) !== -1 || method.indexOf(searchValue) !== -1 || summary.indexOf(searchValue) !== -1) {{
                    operation.style.display = 'block';
                    visibleCount++;
                }} else {{
                    operation.style.display = 'none';
                }}
            }}

            updateNoResults(visibleCount);
        }}

        function filterByMethod(method) {{
            var operations = document.querySelectorAll('.operation');
            var filterTags = document.querySelectorAll('.filter-tag');
            var visibleCount = 0;

            for (var i = 0; i < filterTags.length; i++) {{
                var tag = filterTags[i];
                if (tag.textContent.indexOf(method) !== -1) {{
                    tag.classList.add('active');
                }} else {{
                    tag.classList.remove('active');
                }}
            }}

            for (var i = 0; i < operations.length; i++) {{
                var operation = operations[i];
                var operationMethod = operation.querySelector('.http-method').textContent;
                
                if (method === 'ALL' || operationMethod === method) {{
                    operation.style.display = 'block';
                    visibleCount++;
                }} else {{
                    operation.style.display = 'none';
                }}
            }}

            updateNoResults(visibleCount);
        }}

        function updateNoResults(count) {{
            var noResults = document.getElementById('no-results');
            
            if (count === 0) {{
                if (!noResults) {{
                    noResults = document.createElement('div');
                    noResults.id = 'no-results';
                    noResults.className = 'no-results';
                    noResults.innerHTML = '<div class=""no-results-icon"">&#128269;</div><h3>No endpoints found</h3><p>Try adjusting your search or filters</p>';
                    document.querySelector('.operations-section').appendChild(noResults);
                }}
            }} else {{
                if (noResults) {{
                    noResults.remove();
                }}
            }}
        }}

        function tryEndpoint(method, path) {{
            var baseUrl = window.location.origin;
            var fullUrl = baseUrl + path;
            var curlCommand = 'curl -X ' + method + ' ""' + fullUrl + '"" -H ""Content-Type: application/json""';
            
            var modal = document.createElement('div');
            modal.style.cssText = 'position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0, 0, 0, 0.7); display: flex; align-items: center; justify-content: center; z-index: 9999;';
            modal.id = 'try-modal';
            
            var modalContent = document.createElement('div');
            modalContent.style.cssText = 'background: white; padding: 30px; border-radius: 8px; max-width: 600px; width: 90%; max-height: 80vh; overflow-y: auto;';
            
            modalContent.innerHTML = '<h2 style=""margin-bottom: 20px; color: #3b4151;"">Try it out</h2>' +
                '<div style=""margin-bottom: 20px;""><label style=""display: block; font-weight: 600; margin-bottom: 8px; color: #3b4151;"">Method</label>' +
                '<span class=""http-method method-' + method.toLowerCase() + '"" style=""display: inline-block;"">' + method + '</span></div>' +
                '<div style=""margin-bottom: 20px;""><label style=""display: block; font-weight: 600; margin-bottom: 8px; color: #3b4151;"">URL</label>' +
                '<div style=""background: #f5f5f5; padding: 12px; border-radius: 4px; font-family: monospace; word-break: break-all;"">' + fullUrl + '</div></div>' +
                '<div style=""margin-bottom: 20px;""><label style=""display: block; font-weight: 600; margin-bottom: 8px; color: #3b4151;"">cURL Command</label>' +
                '<div style=""background: #2d2d2d; color: #f8f8f2; padding: 15px; border-radius: 4px; font-family: monospace; font-size: 0.85em; overflow-x: auto; word-break: break-all;"">' + curlCommand + '</div></div>' +
                '<div style=""margin-bottom: 20px;""><p style=""color: #666; font-size: 0.95em; line-height: 1.6;"">' +
                'Copy the cURL command above or use these tools to test the endpoint:<br><br>' +
                '&#8226; <strong>Postman</strong>: Import as cURL or create new request<br>' +
                '&#8226; <strong>Browser</strong>: For GET requests only<br>' +
                '&#8226; <strong>Thunder Client</strong>: VS Code extension<br>' +
                '&#8226; <strong>Insomnia</strong>: REST API client</p></div>' +
                '<div style=""display: flex; gap: 10px; justify-content: flex-end;"">' +
                '<button id=""copyUrlBtn"" style=""padding: 10px 20px; background: #49cc90; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: 600;"">&#128203; Copy URL</button>' +
                '<button id=""copyCurlBtn"" style=""padding: 10px 20px; background: #4990e2; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: 600;"">&#128203; Copy cURL</button>' +
                '<button id=""closeBtn"" style=""padding: 10px 20px; background: #dc3545; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: 600;"">Close</button></div>';
            
            modal.appendChild(modalContent);
            document.body.appendChild(modal);
            
            document.getElementById('copyUrlBtn').onclick = function() {{ copyToClipboard(fullUrl, this); }};
            document.getElementById('copyCurlBtn').onclick = function() {{ copyToClipboard(curlCommand, this); }};
            document.getElementById('closeBtn').onclick = closeModal;
            
            modal.addEventListener('click', function(e) {{
                if (e.target === modal) {{
                    modal.remove();
                }}
            }});
        }}
        
        function closeModal() {{
            var modal = document.getElementById('try-modal');
            if (modal) {{
                modal.remove();
            }}
        }}
        
        function copyToClipboard(text, btn) {{
            var textarea = document.createElement('textarea');
            textarea.value = text;
            textarea.style.position = 'fixed';
            textarea.style.opacity = '0';
            document.body.appendChild(textarea);
            textarea.select();
            
            try {{
                document.execCommand('copy');
                var originalText = btn.textContent;
                var originalBg = btn.style.background;
                btn.textContent = '&#10004; Copied!';
                btn.style.background = '#28a745';
                
                setTimeout(function() {{
                    btn.textContent = originalText;
                    btn.style.background = originalBg;
                }}, 2000);
            }} catch (err) {{
                alert('Copy failed. Please copy manually.');
            }}
            
            document.body.removeChild(textarea);
        }}
    </script>
</body>
</html>";
        }

        private string GenerateGroupsHtml(IEnumerable<IGrouping<string, EndpointInfo>> groups)
        {
            var html = "";

            foreach (var group in groups)
            {
                var groupId = group.Key.Replace(" ", "");
                html += $@"
            <div class=""operation-group"">
                <div class=""group-header"" onclick=""toggleGroup('{groupId}')"">
                    <span class=""group-title"">{group.Key}</span>
                    <span class=""group-count"">({group.Count()} endpoints)</span>
                </div>
                {GenerateOperationsHtml(group, groupId)}
            </div>";
            }

            return html;
        }

        private string GenerateOperationsHtml(IEnumerable<EndpointInfo> endpoints, string groupId)
        {
            var html = "";
            var index = 0;

            foreach (var endpoint in endpoints)
            {
                var operationId = $"{groupId}-{index++}";
                var methodClass = $"method-{endpoint.Method.ToLower()}";

                html += $@"
                <div class=""operation"" data-group=""{groupId}"" data-method=""{endpoint.Method}"">
                    <div class=""operation-header"" onclick=""toggleOperation('{operationId}')"">
                        <span class=""http-method {methodClass}"">{endpoint.Method}</span>
                        <span class=""operation-path"">{endpoint.Route}</span>
                        <span class=""operation-summary"">{endpoint.Description}</span>
                        <svg id=""arrow-{operationId}"" class=""expand-arrow"" width=""20"" height=""20"" viewBox=""0 0 20 20"">
                            <path d=""M5 8l5 5 5-5"" fill=""none"" stroke=""currentColor"" stroke-width=""2""/>
                        </svg>
                    </div>
                    <div id=""details-{operationId}"" class=""operation-details"">
                        <div class=""details-content"">
                            {GenerateOperationDetailsHtml(endpoint)}
                        </div>
                    </div>
                </div>";
            }

            return html;
        }

        private string GenerateOperationDetailsHtml(EndpointInfo endpoint)
        {
            var parametersHtml = endpoint.Parameters.Any()
                ? $@"
                    <div class=""section-title"">Parameters</div>
                    <table class=""parameters-table"">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Located In</th>
                                <th>Type</th>
                                <th>Required</th>
                            </tr>
                        </thead>
                        <tbody>
                            {string.Join("", endpoint.Parameters.Select(p => $@"
                            <tr>
                                <td><span class=""param-name"">{p.Name}</span></td>
                                <td>path</td>
                                <td><span class=""param-type"">{p.Type}</span></td>
                                <td>{(p.Required ? "<span class=\"param-required\">required</span>" : "optional")}</td>
                            </tr>"))}
                        </tbody>
                    </table>"
                : "<p>No parameters required</p>";

            var exampleRequest = endpoint.Method == "POST" || endpoint.Method == "PUT"
                ? @"
                    <div class=""section-title"">Example Request Body</div>
                    <div class=""code-block"">{
  ""login"": 5550001,
  ""amount"": 1000.00,
  ""comment"": ""Transaction""
}</div>"
                : "";

            var exampleResponse = @"
                <div class=""section-title"">Response</div>
                <div class=""response-examples"">
                    <span class=""response-code code-200"">200</span> Success
                    <div class=""code-block"">{
  ""success"": true,
  ""message"": ""Operation successful"",
  ""data"": { ... }
}</div>
                </div>";

            return $@"
                <div class=""try-section"">
                    <button class=""try-button"" onclick=""tryEndpoint('{endpoint.Method.Replace("'", "\\'")}', '{endpoint.Route.Replace("'", "\\'")}')"">
                       Try it out
                    </button>
                </div>
                {parametersHtml}
                {exampleRequest}
                {exampleResponse}";
        }
    }

    public class EndpointInfo
    {
        public string Controller { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
    }
}
