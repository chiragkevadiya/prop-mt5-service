using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Welcome page controller - serves dynamic welcome/documentation page
    /// Author: Amit Kumar
    /// Version: 1.0
    /// </summary>
    [RoutePrefix("")]
    public class WelcomeController : ApiController
    {
        private const string Version = "1.0.0";
        private const string Author = "Amit Kumar";

        [HttpGet]
        [Route("")]
        [Route("welcome")]
        [Route("home")]
        public HttpResponseMessage GetWelcomePage()
        {
            try
            {
                var html = GenerateWelcomePageHtml();

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(html, Encoding.UTF8, "text/html")
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html") { CharSet = "utf-8" };
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true,
                    NoStore = true,
                    MustRevalidate = true
                };

                return response;
            }
            catch (Exception)
            {
                // Fallback: Return simple welcome page if main page fails
                var fallbackHtml = GenerateFallbackWelcomePageHtml();

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(fallbackHtml, Encoding.UTF8, "text/html")
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html") { CharSet = "utf-8" };

                return response;
            }
        }

        private string GenerateWelcomePageHtml()
        {
            var serverTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var currentDate = DateTime.UtcNow.ToString("MMMM dd, yyyy");

            return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Prop MT5 Connection Service - API Documentation</title>
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
            max-width: 1200px;
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

        .nav {{
            background: #f8f9fa;
            padding: 20px 40px;
            border-bottom: 2px solid #e9ecef;
            display: flex;
            gap: 20px;
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
            transform: translateY(-2px);
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

        .status-card {{
            background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
            border-left: 5px solid #28a745;
            padding: 25px;
            border-radius: 10px;
            margin-bottom: 30px;
        }}

        .status-item {{
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid rgba(0, 0, 0, 0.1);
        }}

        .status-item:last-child {{
            border-bottom: none;
        }}

        .status-label {{
            font-weight: 600;
            color: #155724;
        }}

        .status-value {{
            color: #155724;
            font-family: 'Courier New', monospace;
        }}

        .status-healthy {{
            color: #28a745 !important;
            font-weight: 700;
        }}

        .status-unhealthy {{
            color: #dc3545 !important;
            font-weight: 700;
        }}

        .endpoint-category {{
            margin-bottom: 30px;
        }}

        .endpoint-category h3 {{
            color: #764ba2;
            font-size: 1.5em;
            margin-bottom: 15px;
            padding-left: 10px;
            border-left: 4px solid #764ba2;
        }}

        .endpoint {{
            background: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 15px;
            border: 1px solid #dee2e6;
            transition: all 0.3s;
        }}

        .endpoint:hover {{
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
            transform: translateY(-2px);
        }}

        .endpoint-header {{
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 10px;
        }}

        .method {{
            padding: 5px 15px;
            border-radius: 5px;
            font-weight: 700;
            font-size: 0.9em;
            color: white;
            display: inline-block;
            min-width: 60px;
            text-align: center;
        }}

        .method.get {{
            background: #28a745;
        }}

        .method.post {{
            background: #007bff;
        }}

        .method.put {{
            background: #ffc107;
            color: #333;
        }}

        .method.delete {{
            background: #dc3545;
        }}

        .endpoint-path {{
            font-family: 'Courier New', monospace;
            font-size: 1.1em;
            color: #667eea;
            font-weight: 600;
        }}

        .endpoint-description {{
            color: #666;
            margin-bottom: 15px;
        }}

        .code-block {{
            background: #2d2d2d;
            color: #f8f8f2;
            padding: 20px;
            border-radius: 8px;
            overflow-x: auto;
            margin: 10px 0;
            font-family: 'Courier New', monospace;
            font-size: 0.9em;
        }}

        .feature-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }}

        .feature-card {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 25px;
            border-radius: 10px;
            text-align: center;
        }}

        .feature-card h4 {{
            font-size: 1.3em;
            margin-bottom: 10px;
        }}

        .tech-stack {{
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 15px;
        }}

        .tech-badge {{
            background: #667eea;
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.9em;
            font-weight: 600;
        }}

        .footer {{
            background: #2d2d2d;
            color: white;
            text-align: center;
            padding: 30px;
            margin-top: 40px;
        }}

        .loading-spinner {{
            display: inline-block;
            animation: pulse 1.5s ease-in-out infinite;
        }}

        @keyframes pulse {{
            0%, 100% {{ opacity: 1; }}
            50% {{ opacity: 0.3; }}
        }}

        button {{
            padding: 12px 24px;
            background: #667eea;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 600;
            transition: all 0.3s;
        }}

        button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
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
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <!-- Header -->
        <div class=""header"">
            <h1>&#128640; Prop MT5 Connection Service</h1>
            <p>MetaTrader 5 API Integration &amp; Trading Management Platform</p>
            <p style=""margin-top: 10px; opacity: 0.8;"">Generated: " + serverTime + @" UTC</p>
        </div>

        <!-- Navigation -->
        <div class=""nav"">
            <a href=""#overview"" class=""nav-link"">Overview</a>
            <a href=""#status"" class=""nav-link"">Status</a>
            <a href=""#endpoints"" class=""nav-link"">API Endpoints</a>
            <a href=""/api/health"" class=""nav-link"">Health Check</a>
        </div>

        <!-- Content -->
        <div class=""content"">
            <!-- Project Overview -->
            <section id=""overview"" class=""section"">
                <h2>&#128203; Project Overview</h2>
                <p style=""font-size: 1.1em; margin-bottom: 20px;"">
                    The Prop MT5 Connection Service is a robust, production-ready Windows Service that provides 
                    RESTful API endpoints for managing MetaTrader 5 (MT5) trading accounts, executing trades, 
                    monitoring positions, and performing automated liquidation checks.
                </p>

                <div class=""feature-grid"">
                    <div class=""feature-card"">
                        <h4>&#128268; MT5 Integration</h4>
                        <p>Direct connection to MetaTrader 5 servers using official MT5 Manager API</p>
                    </div>
                    <div class=""feature-card"">
                        <h4>&#128260; Real-time Operations</h4>
                        <p>Live account management, trading operations, and market data access</p>
                    </div>
                    <div class=""feature-card"">
                        <h4>&#9889; High Performance</h4>
                        <p>Optimized for high-frequency operations with caching and monitoring</p>
                    </div>
                    <div class=""feature-card"">
                        <h4>&#128737; Production Ready</h4>
                        <p>Enterprise-grade security, logging, error handling, and monitoring</p>
                    </div>
                </div>

                <h3 style=""margin-top: 30px; color: #764ba2;"">Technology Stack</h3>
                <div class=""tech-stack"">
                    <span class=""tech-badge"">.NET Framework 4.8</span>
                    <span class=""tech-badge"">C# 7.3</span>
                    <span class=""tech-badge"">OWIN</span>
                    <span class=""tech-badge"">ASP.NET Web API</span>
                    <span class=""tech-badge"">Nancy Framework</span>
                    <span class=""tech-badge"">Serilog</span>
                    <span class=""tech-badge"">Topshelf</span>
                    <span class=""tech-badge"">MetaTrader 5 API</span>
                </div>
            </section>

            <!-- Service Status -->
            <section id=""status"" class=""section"">
                <h2>&#9989; Service Status</h2>
                
                <div class=""status-card"" id=""serviceStatusCard"">
                    <div class=""status-item"">
                        <span class=""status-label"">&#128994; Service Status:</span>
                        <span class=""status-value"" id=""serviceStatus"">
                            <span class=""loading-spinner"">&#9679;</span> Checking...
                        </span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#128205; Base URL:</span>
                        <span class=""status-value"">http://localhost:8086</span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#128290; API Version:</span>
                        <span class=""status-value"" id=""apiVersion"">v1.0</span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#128336; Server Time (UTC):</span>
                        <span class=""status-value"" id=""serverTime"">" + serverTime + @"</span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#9200; Service Uptime:</span>
                        <span class=""status-value"" id=""serviceUptime"">--</span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#128268; MT5 Live Connection:</span>
                        <span class=""status-value"" id=""mt5LiveStatus"">
                            <span class=""loading-spinner"">&#9679;</span> Checking...
                        </span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#127757; Environment:</span>
                        <span class=""status-value"" id=""environment"">Production</span>
                    </div>
                    <div class=""status-item"">
                        <span class=""status-label"">&#128260; Last Health Check:</span>
                        <span class=""status-value"" id=""lastHealthCheck"">--</span>
                    </div>
                </div>

                <div style=""margin-top: 20px; padding: 20px; background: #f8f9fa; border-radius: 8px; border: 1px solid #dee2e6;"">
                    <h3 style=""color: #764ba2; margin-bottom: 15px;"">&#128269; System Health Details</h3>
                    <div id=""healthDetails"" style=""font-family: 'Courier New', monospace; font-size: 0.9em; color: #666;"">
                        Loading health check data...
                    </div>
                </div>

                <div style=""margin-top: 20px; display: flex; gap: 15px; flex-wrap: wrap;"">
                    <button onclick=""refreshStatus()"">&#128260; Refresh Status</button>
                    <a href=""/api/health"" target=""_blank"" style=""padding: 12px 24px; background: #28a745; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: 600; text-decoration: none; display: inline-block;"">
                        &#127973; View Raw Health Data
                    </a>
                    <a href=""/api/mt5/accounts"" target=""_blank"" style=""padding: 12px 24px; background: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: 600; text-decoration: none; display: inline-block;"">
                        &#128101; View Accounts
                    </a>
                </div>
            </section>

            <!-- API Endpoints -->
            <section id=""endpoints"" class=""section"">
                <h2>&#128225; Key API Endpoints</h2>

                <div class=""endpoint-category"">
                    <h3>&#127973; Health &amp; Status</h3>
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/health</span>
                        </div>
                        <p class=""endpoint-description"">Service health check with system diagnostics</p>
                    </div>
                </div>

                <div class=""endpoint-category"">
                    <h3>&#128101; Account Management</h3>
                    
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/mt5/accounts</span>
                        </div>
                        <p class=""endpoint-description"">Get all MT5 trading accounts</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/mt5/account/{loginId}</span>
                        </div>
                        <p class=""endpoint-description"">Get specific account by login ID</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method post"">POST</span>
                            <span class=""endpoint-path"">/api/mt5/account/create</span>
                        </div>
                        <p class=""endpoint-description"">Create new MT5 trading account</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/accountperformance/performance</span>
                        </div>
                        <p class=""endpoint-description"">Get account performance metrics</p>
                    </div>
                </div>

                <div class=""endpoint-category"">
                    <h3>&#9889; Trading Operations</h3>
                    
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/liquidation/MT5Liquidation</span>
                        </div>
                        <p class=""endpoint-description"">Check and execute automated liquidations</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/opentradedetail</span>
                        </div>
                        <p class=""endpoint-description"">Get all open trading positions</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/livedealhistory</span>
                        </div>
                        <p class=""endpoint-description"">Get trading history and closed positions</p>
                    </div>
                </div>

                <div class=""endpoint-category"">
                    <h3>&#128176; Financial Operations</h3>
                    
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method post"">POST</span>
                            <span class=""endpoint-path"">/api/livewithdrawal</span>
                        </div>
                        <p class=""endpoint-description"">Process withdrawal requests</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method post"">POST</span>
                            <span class=""endpoint-path"">/api/accounttransfer</span>
                        </div>
                        <p class=""endpoint-description"">Transfer funds between accounts</p>
                    </div>
                </div>

                <div class=""endpoint-category"">
                    <h3>&#128200; Analytics &amp; Reports</h3>
                    
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/livedashboard</span>
                        </div>
                        <p class=""endpoint-description"">Get dashboard metrics and statistics</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/leaderboard</span>
                        </div>
                        <p class=""endpoint-description"">Get trader leaderboard rankings</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/profitbysymbol</span>
                        </div>
                        <p class=""endpoint-description"">Get profit analysis by trading symbol</p>
                    </div>
                </div>

                <div class=""endpoint-category"">
                    <h3>&#9881; Configuration</h3>
                    
                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/group</span>
                        </div>
                        <p class=""endpoint-description"">Get MT5 trading groups</p>
                    </div>

                    <div class=""endpoint"">
                        <div class=""endpoint-header"">
                            <span class=""method get"">GET</span>
                            <span class=""endpoint-path"">/api/symbol/GetAllSymbols</span>
                        </div>
                        <p class=""endpoint-description"">Get all available trading symbols</p>
                    </div>
                </div>
            </section>

            <!-- Quick Start -->
            <section class=""section"">
                <h2>&#128640; Quick Start Guide</h2>
                
                <h3 style=""color: #764ba2; margin-top: 20px;"">1. Check Service Health</h3>
                <div class=""code-block"">curl http://localhost:8086/api/health</div>

                <h3 style=""color: #764ba2; margin-top: 20px;"">2. List All Accounts</h3>
                <div class=""code-block"">curl http://localhost:8086/api/mt5/accounts</div>

                <h3 style=""color: #764ba2; margin-top: 20px;"">3. Get Account Performance</h3>
                <div class=""code-block"">curl http://localhost:8086/api/accountperformance/performance</div>

                <h3 style=""color: #764ba2; margin-top: 20px;"">4. Check Liquidations</h3>
                <div class=""code-block"">curl http://localhost:8086/api/liquidation/MT5Liquidation</div>
            </section>
        </div>

        <!-- Footer -->
        <div class=""footer"">
            <p style=""font-size: 1.2em; margin-bottom: 10px;""><strong>Prop MT5 Connection Service</strong></p>
            <p style=""font-size: 1.1em; color: #667eea; margin: 10px 0;"">&#128100; Developed by: " + Author + @"</p>
            <p>Version " + Version + @" | Released: " + currentDate + @"</p>
            <p style=""margin-top: 10px;"">Powered by OWIN + Web API + Serilog + Topshelf</p>
            <p style=""margin-top: 15px; opacity: 0.8;"">
                Built with &#10084; for Professional Trading | &copy; 2025
            </p>
        </div>
    </div>

    <script>
        // Update server time
        function updateTime() {
            const now = new Date();
            document.getElementById('serverTime').textContent = now.toISOString();
        }
        updateTime();
        setInterval(updateTime, 1000);

        // Fetch health status from API
        async function fetchHealthStatus() {
            try {
                const response = await fetch('/api/health');
                const data = await response.json();
                
                const statusElement = document.getElementById('serviceStatus');
                if (data.status === 'Healthy' || data.success) {
                    statusElement.innerHTML = '<span class=""status-healthy"">&#128994; RUNNING</span>';
                    document.getElementById('serviceStatusCard').style.background = 'linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%)';
                } else {
                    statusElement.innerHTML = '<span class=""status-unhealthy"">&#128308; UNHEALTHY</span>';
                    document.getElementById('serviceStatusCard').style.background = 'linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%)';
                }

                if (data.version) {
                    document.getElementById('apiVersion').textContent = data.version;
                }

                if (data.uptime) {
                    document.getElementById('serviceUptime').textContent = data.uptime;
                }

                if (data.environment) {
                    document.getElementById('environment').textContent = data.environment;
                }

                if (data.checks) {
                    const mt5Status = data.checks.mt5 || data.checks.owin || 'Unknown';
                    const mt5Element = document.getElementById('mt5LiveStatus');
                    if (mt5Status === 'OK' || mt5Status === 'Connected') {
                        mt5Element.innerHTML = '<span class=""status-healthy"">&#9989; CONNECTED</span>';
                    } else {
                        mt5Element.innerHTML = '<span class=""status-unhealthy"">&#10060; ' + mt5Status + '</span>';
                    }
                }

                const now = new Date();
                document.getElementById('lastHealthCheck').textContent = now.toLocaleTimeString();

                document.getElementById('healthDetails').innerHTML = '<pre style=""margin: 0; overflow-x: auto;"">' + 
                    JSON.stringify(data, null, 2) + '</pre>';

            } catch (error) {
                console.error('Failed to fetch health status:', error);
                document.getElementById('serviceStatus').innerHTML = '<span class=""status-unhealthy"">&#128308; UNREACHABLE</span>';
                document.getElementById('mt5LiveStatus').innerHTML = '<span class=""status-unhealthy"">&#10060; UNKNOWN</span>';
                document.getElementById('serviceStatusCard').style.background = 'linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%)';
                document.getElementById('healthDetails').textContent = 'Failed to fetch health data: ' + error.message;
            }
        }

        function refreshStatus() {
            document.getElementById('serviceStatus').innerHTML = '<span class=""loading-spinner"">&#9679;</span> Refreshing...';
            document.getElementById('mt5LiveStatus').innerHTML = '<span class=""loading-spinner"">&#9679;</span> Checking...';
            fetchHealthStatus();
        }

        fetchHealthStatus();
        setInterval(fetchHealthStatus, 30000);

        document.querySelectorAll('a[href^=""#""]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            });
        });
    </script>
</body>
</html>
";
        }

        private string GenerateFallbackWelcomePageHtml()
        {
            var currentDate = DateTime.UtcNow.ToString("MMMM dd, yyyy");
            var serverTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            return @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Prop MT5 Connection Service</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #333;
            line-height: 1.6;
            padding: 20px;
        }

        .container {
            max-width: 900px;
            margin: 40px auto;
            background: white;
            padding: 50px;
            border-radius: 15px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
        }

        .header {
            text-align: center;
            margin-bottom: 40px;
            padding-bottom: 30px;
            border-bottom: 3px solid #667eea;
        }

        .header h1 {
            color: #667eea;
            font-size: 2.5em;
            margin-bottom: 15px;
        }

        .header .subtitle {
            font-size: 1.2em;
            color: #666;
            margin-bottom: 20px;
        }

        .badge {
            display: inline-block;
            background: #667eea;
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.9em;
            margin: 5px;
            font-weight: 600;
        }

        .status-box {
            background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
            border-left: 5px solid #28a745;
            padding: 25px;
            border-radius: 10px;
            margin: 30px 0;
        }

        .status-item {
            display: flex;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid rgba(0, 0, 0, 0.1);
        }

        .status-item:last-child {
            border-bottom: none;
        }

        .status-label {
            font-weight: 600;
            color: #155724;
        }

        .status-value {
            color: #155724;
            font-family: 'Courier New', monospace;
        }

        .links-section {
            margin: 30px 0;
        }

        .links-section h2 {
            color: #667eea;
            font-size: 1.8em;
            margin-bottom: 20px;
        }

        .link-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }

        .link-card {
            background: #f8f9fa;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
            transition: all 0.3s;
            border: 2px solid #dee2e6;
        }

        .link-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 20px rgba(0, 0, 0, 0.15);
            border-color: #667eea;
        }

        .link-card a {
            color: #667eea;
            text-decoration: none;
            font-weight: bold;
            font-size: 1.1em;
        }

        .link-card a:hover {
            color: #764ba2;
        }

        .info-section {
            background: #f8f9fa;
            padding: 25px;
            border-radius: 10px;
            margin: 30px 0;
        }

        .info-section h3 {
            color: #764ba2;
            margin-bottom: 15px;
            font-size: 1.4em;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }

        .info-item {
            background: white;
            padding: 15px;
            border-radius: 8px;
            border-left: 4px solid #667eea;
        }

        .info-item strong {
            color: #667eea;
            display: block;
            margin-bottom: 5px;
        }

        .footer {
            text-align: center;
            margin-top: 40px;
            padding-top: 30px;
            border-top: 2px solid #e9ecef;
            color: #666;
        }

        .footer .author {
            font-size: 1.1em;
            color: #667eea;
            font-weight: 600;
            margin: 10px 0;
        }

        .note {
            background: #fff3cd;
            border-left: 5px solid #ffc107;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
        }

        .note p {
            color: #856404;
            margin: 0;
        }

        @media (max-width: 768px) {
            .container {
                padding: 30px 20px;
                margin: 20px auto;
            }

            .header h1 {
                font-size: 2em;
            }

            .link-grid, .info-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <div class=""container"">
        <!-- Header -->
        <div class=""header"">
            <h1>&#128640; Prop MT5 Connection Service</h1>
            <p class=""subtitle"">MetaTrader 5 API Integration &amp; Trading Management Platform</p>
            <div>
                <span class=""badge"">Version " + Version + @"</span>
                <span class=""badge"">&#128197; " + currentDate + @"</span>
                <span class=""badge"">&#128336; " + serverTime + @" UTC</span>
            </div>
        </div>

        <!-- Status Box -->
        <div class=""status-box"">
            <div class=""status-item"">
                <span class=""status-label"">&#128994; Service Status:</span>
                <span class=""status-value"">RUNNING</span>
            </div>
            <div class=""status-item"">
                <span class=""status-label"">&#128205; Base URL:</span>
                <span class=""status-value"">http://localhost:8086</span>
            </div>
            <div class=""status-item"">
                <span class=""status-label"">&#128290; API Version:</span>
                <span class=""status-value"">" + Version + @"</span>
            </div>
            <div class=""status-item"">
                <span class=""status-label"">&#128268; MT5 Connection:</span>
                <span class=""status-value"">ACTIVE</span>
            </div>
        </div>

        <!-- Quick Links Section -->
        <div class=""links-section"">
            <h2>&#128279; Quick Links</h2>
            <div class=""link-grid"">
                <div class=""link-card"">
                    <a href=""/health"">&#127973; Health Check</a>
                </div>
                <div class=""link-card"">
                    <a href=""/api/health"">&#128200; API Health</a>
                </div>
                <div class=""link-card"">
                    <a href=""/api/mt5/accounts"">&#128101; View Accounts</a>
                </div>
                <div class=""link-card"">
                    <a href=""/api/liquidation/MT5Liquidation"">&#9889; Liquidation</a>
                </div>
                <div class=""link-card"">
                    <a href=""/api/livedashboard"">&#128202; Dashboard</a>
                </div>
                <div class=""link-card"">
                    <a href=""/api/leaderboard"">&#127942; Leaderboard</a>
                </div>
            </div>
        </div>

        <!-- System Information -->
        <div class=""info-section"">
            <h3>&#128187; System Information</h3>
            <div class=""info-grid"">
                <div class=""info-item"">
                    <strong>Framework:</strong>
                    .NET Framework 4.8
                </div>
                <div class=""info-item"">
                    <strong>Language:</strong>
                    C# 7.3
                </div>
                <div class=""info-item"">
                    <strong>Web Server:</strong>
                    OWIN + Web API
                </div>
                <div class=""info-item"">
                    <strong>Logging:</strong>
                    Serilog
                </div>
                <div class=""info-item"">
                    <strong>Service Host:</strong>
                    Topshelf
                </div>
                <div class=""info-item"">
                    <strong>MT5 API:</strong>
                    MetaTrader 5 Manager API
                </div>
            </div>
        </div>

        <!-- Note -->
        <div class=""note"">
            <p>&#9888; <strong>Note:</strong> You are viewing the simplified fallback page. The full documentation page with interactive features is the default view.</p>
        </div>

        <!-- Footer -->
        <div class=""footer"">
            <p class=""author"">&#128100; Developed by: " + Author + @"</p>
            <p><strong>Version:</strong> " + Version + @" | <strong>Released:</strong> " + currentDate + @"</p>
            <p style=""margin-top: 15px; color: #999;"">
                Built with &#10084; for Professional Trading
            </p>
            <p style=""margin-top: 10px; font-size: 0.9em;"">
                &copy; 2025 Prop MT5 Connection Service. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>
";
        }
    }
}
