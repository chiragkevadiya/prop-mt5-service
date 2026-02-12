# Swagger/OpenAPI Configuration Guide

## Overview
Swagger documentation has been configured for the Prop MT5 Connection Service API. This provides interactive API documentation accessible via browser.

## Installation Steps

### 1. Install Swashbuckle Package
The required package has been added to `packages.config`. Restore NuGet packages:

```powershell
# In Visual Studio: Right-click solution > Restore NuGet Packages
# Or via Package Manager Console:
Update-Package -reinstall
```

Packages added:
- `Swashbuckle.Core` v5.6.0
- `WebActivatorEx` v2.2.0 (dependency)

### 2. Enable XML Documentation

**Option A: Via Visual Studio UI**
1. Right-click project → Properties
2. Go to **Build** tab
3. Check **XML documentation file**
4. Set path: `bin\Debug\PropMT5ConnectionService.xml` (for Debug)
5. Repeat for Release configuration: `bin\Release\PropMT5ConnectionService.xml`
6. Add to Suppress warnings: `1591` (missing XML comments)

**Option B: Manual Edit (Close Solution First)**
Add these lines to `PropMT5ConnectionService.csproj`:

```xml
<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
  <!-- Existing properties... -->
  <DocumentationFile>bin\Debug\PropMT5ConnectionService.xml</DocumentationFile>
  <NoWarn>1591</NoWarn>
</PropertyGroup>

<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
  <!-- Existing properties... -->
  <DocumentationFile>bin\Release\PropMT5ConnectionService.xml</DocumentationFile>
  <NoWarn>1591</NoWarn>
</PropertyGroup>
```

### 3. Rebuild Solution
```powershell
# Clean and rebuild
dotnet clean
dotnet build
```

## Accessing Swagger Documentation

Once the application is running, access Swagger UI at:

**Default URL:**
```
http://localhost:<port>/swagger
```

**Swagger JSON Specification:**
```
http://localhost:<port>/swagger/docs/v1
```

## Features Implemented

### 1. **API Information**
- Title: "Prop MT5 Connection Service API"
- Version: v1
- Description: REST API for MT5 trading operations
- Contact: dev@proptrading.com
- License: Proprietary

### 2. **Authentication Support**
- JWT Bearer token authentication
- Authorization header in Swagger UI
- Format: `Bearer {your-token-here}`

### 3. **Custom Filters**

#### `AddResponseHeadersFilter`
Automatically adds standard HTTP response codes:
- `400` - Bad Request
- `401` - Unauthorized
- `500` - Internal Server Error

#### `AddRequiredHeaderParameter`
Auto-detects `[Authorize]` attributes and adds required Authorization header to documentation

#### `HideInSwaggerFilter`
Document filter for hiding specific endpoints (customize as needed)

### 4. **XML Documentation**
- Automatically includes XML comments from code
- Provides detailed parameter descriptions
- Shows return types and examples

### 5. **Configuration Options**
- Schema names use full type names to avoid conflicts
- Enums displayed as strings
- Obsolete actions/properties are hidden
- Discovery URL selector enabled
- API key support in UI

## Swagger Configuration Options

### Customizing API Info
Edit `SwaggerConfig.cs`:

```csharp
c.SingleApiVersion("v1", "Your API Title")
    .Description("Your API description")
    .Contact(cc => cc
        .Name("Your Name")
        .Email("your@email.com")
        .Url("https://yoursite.com"))
    .License(lc => lc
        .Name("Your License")
        .Url("https://yoursite.com/license"));
```

### Adding Custom Styles
Uncomment in `SwaggerConfig.cs`:

```csharp
c.InjectStylesheet(typeof(SwaggerConfig).Assembly, 
    "PropMT5ConnectionService.Swagger.custom.css");
```

Then add `Swagger\custom.css` as embedded resource.

### OAuth2 Support (Future)
Uncomment in `SwaggerConfig.cs`:

```csharp
c.EnableOAuth2Support("clientId", "clientSecret", "realm", "Prop MT5 API");
```

## Documenting Your API

### Controller Documentation
Add XML comments to your controllers and actions:

```csharp
/// <summary>
/// Manages live MT5 trading accounts
/// </summary>
[RoutePrefix("api/liveaccount")]
public class LiveAccountController : BaseApiController
{
    /// <summary>
    /// Creates a new live trading account
    /// </summary>
    /// <param name="model">Account creation details</param>
    /// <returns>Created account information</returns>
    /// <response code="200">Account created successfully</response>
    /// <response code="400">Invalid input parameters</response>
    /// <response code="401">Unauthorized - authentication required</response>
    [HttpPost]
    [Route("create")]
    public IHttpActionResult CreateAccount([FromBody] AccountCreateModel model)
    {
        // Implementation
    }
}
```

### Model Documentation

```csharp
/// <summary>
/// MT5 account creation request
/// </summary>
public class AccountCreateModel
{
    /// <summary>
    /// Account login number (must be unique)
    /// </summary>
    /// <example>12345678</example>
    public long Login { get; set; }
    
    /// <summary>
    /// Trading group name
    /// </summary>
    /// <example>demo\\standard</example>
    public string Group { get; set; }
}
```

### Hiding Endpoints from Swagger

```csharp
[ApiExplorerSettings(IgnoreApi = true)]
public IHttpActionResult InternalEndpoint()
{
    // This won't appear in Swagger
}
```

## Troubleshooting

### Issue: Swagger page returns 404
**Solution:** Ensure `SwaggerConfig.Register(config)` is called in `Startup.cs`:

```csharp
PropMT5ConnectionService.Configuration.SwaggerConfig.Register(config);
```

### Issue: XML comments not appearing
**Solutions:**
1. Verify XML documentation file is generated in output directory
2. Check build output path matches `SwaggerConfig.cs` path
3. Ensure XML comments use triple-slash `///` syntax
4. Rebuild project after adding comments

### Issue: Swashbuckle not found error
**Solution:** Restore NuGet packages:
```powershell
Update-Package -reinstall
```

### Issue: Schema conflicts
**Solution:** Already configured with full type names:
```csharp
c.SchemaId(type => type.FullName);
```

## Security Considerations

### 1. Disable in Production (Optional)
Add environment check in `Startup.cs`:

```csharp
#if DEBUG
PropMT5ConnectionService.Configuration.SwaggerConfig.Register(config);
#endif
```

### 2. Add Authentication to Swagger UI
Require authentication to access `/swagger`:

```csharp
app.Map("/swagger", builder =>
{
    builder.Use<AuthenticationMiddleware>();
    builder.UseWebApi(config);
});
```

### 3. Disable External Validator
Already configured:
```csharp
c.DisableValidator();
```

## Additional Resources

- [Swashbuckle GitHub](https://github.com/domaindrivendev/Swashbuckle)
- [Swagger Specification](https://swagger.io/specification/)
- [OpenAPI Initiative](https://www.openapis.org/)

## Testing Swagger

1. **Start the application**
2. **Navigate to:** `http://localhost:<port>/swagger`
3. **Click "Authorize"** and enter JWT token
4. **Expand endpoint** and click "Try it out"
5. **Fill parameters** and click "Execute"
6. **View response** directly in the browser

## Next Steps

1. ✅ Install Swashbuckle package
2. ✅ Enable XML documentation
3. ✅ Rebuild solution
4. 🔲 Add XML comments to your controllers
5. 🔲 Add XML comments to your models
6. 🔲 Test Swagger UI
7. 🔲 Customize branding (optional)
8. 🔲 Add authentication to Swagger UI (optional)

---

**Status:** Configuration complete, ready to use after NuGet restore and XML documentation enabled.
