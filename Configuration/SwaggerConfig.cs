using System.Web.Http;

namespace PropMT5ConnectionService.Configuration
{
    /// <summary>
    /// Swagger configuration for API documentation
    /// Note: Requires Swashbuckle NuGet package to be installed
    /// Install with: Install-Package Swashbuckle -Version 5.6.0
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Register Swagger configuration
        /// </summary>
        public static void Register(HttpConfiguration config)
        {
            // NOTE: Swagger/Swashbuckle configuration is commented out because the package is not installed
            // To enable Swagger:
            // 1. Install package: Install-Package Swashbuckle -Version 5.6.0
            // 2. Uncomment the code below
            // 3. Rebuild the project
            
            /*
            config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Prop MT5 Connection Service API")
                        .Description("REST API for managing MT5 trading accounts, operations, and analytics")
                        .Contact(cc => cc
                            .Name("PropTrading Team")
                            .Email("support@proptrading.com"))
                        .License(lc => lc
                            .Name("Proprietary")
                            .Url("https://proptrading.com/license"));

                    // Include XML comments for documentation
                    var xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\PropMT5ConnectionService.xml";
                    if (System.IO.File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }

                    // Use fully qualified schema names to avoid conflicts
                    c.SchemaId(type => type.FullName);

                    // Hide obsolete endpoints by default
                    c.IgnoreObsoleteActions();

                    // Add authorization header
                    c.ApiKey("Authorization")
                        .Description("API Key Authentication")
                        .Name("Authorization")
                        .In("header");
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Prop MT5 API Documentation");
                    c.DocExpansion(DocExpansion.List);
                    c.EnableDiscoveryUrlSelector();
                    c.EnableApiKeySupport("Authorization", "header");
                });
            */
            
            System.Console.WriteLine("[INFO] Swagger configuration is available but not active. Install Swashbuckle package to enable.");
        }
    }
}

