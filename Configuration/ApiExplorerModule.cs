using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace PropMT5Service.Configuration
{
    public class ApiExplorerModule : NancyModule
    {
        public ApiExplorerModule()
        {
            Get("/explorer", _ =>
            {
                return Response.AsText(GeneratePage(), "text/html");
            });
        }

        private List<EndpointInfo> GetEndpoints()
        {
            var endpoints = new List<EndpointInfo>();

            var assembly = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes()
                .Where(t => typeof(ApiController).IsAssignableFrom(t));

            foreach (var ctrl in controllers)
            {
                var prefix = ctrl.GetCustomAttributes(typeof(RoutePrefixAttribute), true)
                    .Cast<RoutePrefixAttribute>()
                    .FirstOrDefault()?.Prefix ?? "";

                foreach (var method in ctrl.GetMethods())
                {
                    var routeAttr = method.GetCustomAttributes(typeof(RouteAttribute), true)
                        .Cast<RouteAttribute>().FirstOrDefault();

                    var http = GetHttp(method);

                    if (routeAttr == null || http == null) continue;

                    endpoints.Add(new EndpointInfo
                    {
                        ActionName = method.Name,
                        Method = http,
                        Route = "/" + (prefix + "/" + routeAttr.Template).Trim('/'),
                        RequestBody = GetRequestBody(method),
                        Parameters = GetApiParameters(method)
                    });
                }
            }

            return endpoints;
        }

        private string GetHttp(MethodInfo m)
        {
            if (m.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()) return "GET";
            if (m.GetCustomAttributes(typeof(HttpPostAttribute), true).Any()) return "POST";
            if (m.GetCustomAttributes(typeof(HttpPutAttribute), true).Any()) return "PUT";
            if (m.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Any()) return "DELETE";
            return null;
        }

        // 🔥 Extract Route and Query Parameters
        private List<ParameterInfoModel> GetApiParameters(MethodInfo method)
        {
            return method.GetParameters()
                .Where(p => p.ParameterType.IsPrimitive ||
                            p.ParameterType == typeof(string) ||
                            p.ParameterType.IsValueType)
                .Select(p => new ParameterInfoModel
                {
                    Name = p.Name,
                    Type = p.ParameterType.Name
                }).ToList();
        }

        // 🔥 AUTO DTO → JSON for POST/PUT Body
        private string GetRequestBody(MethodInfo method)
        {
            var bodyParam = method.GetParameters()
                .FirstOrDefault(p =>
                    !p.ParameterType.IsPrimitive &&
                    !p.ParameterType.IsValueType &&
                    p.ParameterType != typeof(string));

            if (bodyParam == null)
                return "";

            return GenerateJsonSchema(bodyParam.ParameterType);
        }

        private string GenerateJsonSchema(Type type)
        {
            if (type == null || type == typeof(string) || type.IsPrimitive || type.IsValueType)
                return "{}";

            var props = type.GetProperties();

            var json = "{\n" + string.Join(",\n", props.Select(p =>
            {
                string value = "null";

                if (p.PropertyType == typeof(string))
                    value = "\"string\"";
                else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(long))
                    value = "0";
                else if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(double))
                    value = "0.0";
                else if (p.PropertyType == typeof(bool))
                    value = "false";
                else if (p.PropertyType.IsClass)
                    value = GenerateJsonSchema(p.PropertyType);

                return $"  \"{p.Name}\": {value}";
            })) + "\n}";

            return json;
        }

        private string GeneratePage()
        {
            var endpoints = GetEndpoints();

            return $@"
<!DOCTYPE html>
<html>
<head>
<title>MT5 API Explorer (Swagger Style)</title>
<meta charset=""utf-8"">
<style>
    body {{ margin: 0; font-family: sans-serif; color: #3b4151; background: #fafafa; }}
    * {{ box-sizing: border-box; }}
    
    /* Topbar */
    .topbar {{ background: #1b1b1b; padding: 15px 30px; display: flex; align-items: center; gap: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.2); }}
    .topbar b {{ color: #85ea2d; font-size: 22px; margin-right: 20px; font-weight: 700; display: flex; align-items: center; gap: 10px; }}
    .topbar input, .topbar select {{ padding: 8px 12px; border-radius: 4px; border: 1px solid #ccc; font-size: 14px; width: 250px; background: white; }}
    
    /* Layout */
    .container {{ max-width: 1200px; margin: 40px auto; padding: 0 20px; }}
    .header-info {{ margin-bottom: 30px; }}
    .header-info h2 {{ font-size: 32px; margin: 0 0 10px 0; }}
    .header-info span {{ display: inline-block; background: #89bf04; color: white; padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: bold; margin-left: 10px; vertical-align: middle; }}

    /* Opblocks (Endpoints) */
    .opblock {{ margin: 0 0 15px; border: 1px solid #000; border-radius: 4px; box-shadow: 0 0 3px rgba(0,0,0,.1); background: #fff; }}
    .opblock-summary {{ display: flex; align-items: center; padding: 8px; cursor: pointer; user-select: none; transition: background 0.2s; }}
    .opblock-summary:hover {{ background: rgba(0,0,0,0.02); }}
    .opblock-summary-method {{ font-size: 14px; font-weight: 700; min-width: 80px; padding: 6px 15px; text-align: center; border-radius: 3px; color: #fff; margin-right: 15px; font-family: sans-serif; }}
    .opblock-summary-path {{ font-size: 16px; font-weight: 600; font-family: monospace; color: #3b4151; }}
    .opblock-summary-desc {{ font-size: 14px; color: #555; margin-left: auto; padding-right: 15px; font-weight: normal; font-family: sans-serif; }}

    /* Colors by Method */
    .opblock.post {{ border-color: #49cc90; background: rgba(73,204,144,.1); }}
    .opblock.post .opblock-summary-method {{ background: #49cc90; }}
    
    .opblock.get {{ border-color: #61affe; background: rgba(97,175,254,.1); }}
    .opblock.get .opblock-summary-method {{ background: #61affe; }}
    
    .opblock.put {{ border-color: #fca130; background: rgba(252,161,48,.1); }}
    .opblock.put .opblock-summary-method {{ background: #fca130; }}
    
    .opblock.delete {{ border-color: #f93e3e; background: rgba(249,62,62,.1); }}
    .opblock.delete .opblock-summary-method {{ background: #f93e3e; }}

    /* Expanding Body */
    .opblock-body {{ padding: 20px; display: none; background: #fff; border-top: 1px solid rgba(0,0,0,.1); }}
    .section-header {{ padding: 8px 0; border-bottom: 1px solid #eee; margin-bottom: 15px; font-weight: 700; font-size: 14px; color: #3b4151; }}
    
    /* Inputs & Parameters */
    .params-table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
    .params-table th, .params-table td {{ text-align: left; padding: 10px; border-bottom: 1px solid #eee; }}
    .params-table input {{ width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-family: monospace; }}
    .param-type {{ color: #888; font-size: 12px; font-weight: normal; }}

    textarea {{ width: 100%; height: 160px; font-family: monospace; font-size: 14px; padding: 15px; border: none; border-radius: 4px; background: #41444e; color: #fff; outline: none; margin-bottom: 15px; resize: vertical; }}
    .execute-btn {{ background: #49cc90; color: #fff; border: none; padding: 8px 30px; border-radius: 4px; cursor: pointer; font-weight: bold; font-size: 14px; transition: background 0.2s; display: block; width: 100%; }}
    .execute-btn:hover {{ background: #3b9b6d; }}
    
    /* Responses & Request URL */
    .response-area {{ display: none; margin-top: 30px; border-top: 1px solid #eee; padding-top: 20px; }}
    .request-url-text {{ background: #41444e; color: #fff; padding: 10px 15px; border-radius: 4px; font-family: monospace; font-size: 13px; word-break: break-all; margin-bottom: 20px; }}
    pre {{ background: #333; color: #fff; padding: 15px; border-radius: 4px; overflow-x: auto; max-height: 400px; font-family: monospace; font-size: 14px; margin: 0; }}
    .status-code {{ font-weight: bold; margin-bottom: 10px; display: inline-block; }}
</style>
</head>
<body>

<div class='topbar'>
    <b><span style='font-size:28px'>{""}</span> MT5 Swagger UI</b>
    <select id='env'>
        <option value='http://localhost:8086'>Server: http://localhost:8086</option>
    </select>
    <input id='token' placeholder='Authorize: Bearer Token'/>
</div>

<div class='container'>
    <div class='header-info'>
        <h2>MT5 Contest Service <span>OAS3</span></h2>
        <p>Interactive API documentation and explorer.</p>
    </div>

    {string.Join("", endpoints.Select(e =>
            {
                var id = $"{e.Method}_{e.Route}".Replace("/", "_").Replace("{", "").Replace("}", "").Replace("-", "_").Replace(":", "_");
                var methodLower = e.Method.ToLower();

                return $@"
        <div class='opblock {methodLower}' id='op-{id}'>
            <div class='opblock-summary' onclick=""toggleBody('{id}')"">
                <span class='opblock-summary-method'>{e.Method}</span>
                <span class='opblock-summary-path'>{e.Route}</span>
                <span class='opblock-summary-desc'>{e.ActionName}</span>
            </div>
            
            <div class='opblock-body' id='body-{id}'>
                
                {(e.Parameters.Any() ? $@"
                <div class='section-header'>Parameters (Query / Route)</div>
                <table class='params-table'>
                    <thead><tr><th style='width:30%'>Name</th><th>Value</th></tr></thead>
                    <tbody>
                        {string.Join("", e.Parameters.Select(p => $@"
                        <tr>
                            <td><b>{p.Name}</b> <br><span class='param-type'>({p.Type})</span></td>
                            <td><input type='text' data-param-name='{p.Name}' class='param-input-{id}' placeholder='{p.Name}' /></td>
                        </tr>
                        "))}
                    </tbody>
                </table>
                " : "")}

                {(string.IsNullOrEmpty(e.RequestBody) ? "" : $@"
                <div class='section-header'>Request body</div>
                <textarea id='req-{id}'>{e.RequestBody}</textarea>
                ")}
                
                <button class='execute-btn' onclick=""executeApi('{id}', '{e.Method}', '{e.Route}')"">Execute</button>

                <div class='response-area' id='res-area-{id}'>
                    <div class='section-header' style='margin-bottom: 8px;'>Request URL</div>
                    <div class='request-url-text' id='req-url-{id}'></div>

                    <div class='section-header'>Server Response</div>
                    <div class='status-code' id='res-status-{id}'></div>
                    <pre id='res-{id}'></pre>
                </div>

            </div>
        </div>";
            }))}
</div>

<script>
    // Expand / Collapse
    function toggleBody(id) {{
        var body = document.getElementById('body-' + id);
        if (body.style.display === 'block') {{
            body.style.display = 'none';
        }} else {{
            body.style.display = 'block';
        }}
    }}

    // Try it out Execution
    function executeApi(id, method, path) {{
        var base = document.getElementById('env').value;
        var token = document.getElementById('token').value;
        
        // 1. Process Route and Query Parameters
        var finalPath = path;
        var queryParams = [];
        var paramInputs = document.querySelectorAll('.param-input-' + id);
        
        paramInputs.forEach(input => {{
            var pName = input.getAttribute('data-param-name');
            var pVal = input.value;
            
            if (pVal) {{
                // Create a Regex to match either {{paramName}} OR {{paramName:typeConstraint}}
                var routeRegex = new RegExp('\\{{' + pName + '(:[^}}]+)?\\}}', 'i');
                
                if (routeRegex.test(finalPath)) {{
                    // It's a route parameter, replace it directly in the path
                    finalPath = finalPath.replace(routeRegex, encodeURIComponent(pVal));
                }} else {{
                    // Otherwise, add it to the query string list
                    queryParams.push(encodeURIComponent(pName) + '=' + encodeURIComponent(pVal));
                }}
            }}
        }});
        
        if (queryParams.length > 0) {{
            finalPath += '?' + queryParams.join('&');
        }}

        var url = base + finalPath;

        // 2. Process Request Body
        var reqEl = document.getElementById('req-' + id);
        var bodyData = reqEl ? reqEl.value : null;

        var resArea = document.getElementById('res-area-' + id);
        var reqUrlEl = document.getElementById('req-url-' + id);
        var resPre = document.getElementById('res-' + id);
        var resStatus = document.getElementById('res-status-' + id);

        // Show loading state
        resArea.style.display = 'block';
        reqUrlEl.innerText = url;
        resStatus.innerText = 'Executing...';
        resStatus.style.color = '#3b4151';
        resPre.innerText = 'Awaiting response...';

        var options = {{
            method: method,
            headers: {{
                'Content-Type': 'application/json',
                ...(token && {{ 'Authorization': 'Bearer ' + token }})
            }}
        }};

        if (method !== 'GET' && method !== 'HEAD' && bodyData) {{
            options.body = bodyData;
        }}

        fetch(url, options)
        .then(async r => {{
            let text = await r.text();
            
            // Try formatting JSON nicely
            try {{ text = JSON.stringify(JSON.parse(text), null, 2); }} catch(e) {{}}
            
            resStatus.innerText = 'Code: ' + r.status;
            resStatus.style.color = (r.status >= 200 && r.status < 300) ? '#49cc90' : '#f93e3e';
            resPre.innerText = text || '<Empty Response>';
        }})
        .catch(e => {{
            resStatus.innerText = 'Error';
            resStatus.style.color = '#f93e3e';
            resPre.innerText = 'Failed to fetch: ' + e;
        }});
    }}
</script>

</body>
</html>";
        }
    }

    public class EndpointInfo
    {
        public string ActionName { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string RequestBody { get; set; }
        public List<ParameterInfoModel> Parameters { get; set; } = new List<ParameterInfoModel>();
    }

    public class ParameterInfoModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}