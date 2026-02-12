# Quick Reference: XML Documentation for Controllers

## Basic Controller Documentation

```csharp
/// <summary>
/// [Brief description of what this controller does]
/// </summary>
/// <remarks>
/// [Additional details, usage notes, or examples]
/// </remarks>
[RoutePrefix("api/[controller-route]")]
public class YourController : BaseApiController
{
    // Actions...
}
```

## Action/Endpoint Documentation

### GET Endpoint
```csharp
/// <summary>
/// Retrieves [resource description]
/// </summary>
/// <param name="id">Unique identifier of the [resource]</param>
/// <returns>Returns [what is returned]</returns>
/// <response code="200">Success - [resource] found</response>
/// <response code="404">[Resource] not found</response>
/// <response code="401">Unauthorized - authentication required</response>
[HttpGet]
[Route("{id}")]
public IHttpActionResult GetById(int id)
{
    // Implementation
}
```

### POST Endpoint
```csharp
/// <summary>
/// Creates a new [resource]
/// </summary>
/// <param name="model">The [resource] data to create</param>
/// <returns>Created [resource] with generated ID</returns>
/// <response code="200">Success - [resource] created</response>
/// <response code="400">Bad Request - Invalid input data</response>
/// <response code="401">Unauthorized - authentication required</response>
/// <response code="409">Conflict - [resource] already exists</response>
[HttpPost]
[Route("create")]
public IHttpActionResult Create([FromBody] YourModel model)
{
    // Implementation
}
```

### PUT Endpoint
```csharp
/// <summary>
/// Updates an existing [resource]
/// </summary>
/// <param name="id">Unique identifier of the [resource] to update</param>
/// <param name="model">Updated [resource] data</param>
/// <returns>Updated [resource] information</returns>
/// <response code="200">Success - [resource] updated</response>
/// <response code="400">Bad Request - Invalid input data</response>
/// <response code="404">[Resource] not found</response>
/// <response code="401">Unauthorized - authentication required</response>
[HttpPut]
[Route("{id}")]
public IHttpActionResult Update(int id, [FromBody] YourModel model)
{
    // Implementation
}
```

### DELETE Endpoint
```csharp
/// <summary>
/// Deletes a [resource]
/// </summary>
/// <param name="id">Unique identifier of the [resource] to delete</param>
/// <returns>Confirmation of deletion</returns>
/// <response code="200">Success - [resource] deleted</response>
/// <response code="404">[Resource] not found</response>
/// <response code="401">Unauthorized - authentication required</response>
/// <response code="409">Conflict - [resource] cannot be deleted due to dependencies</response>
[HttpDelete]
[Route("{id}")]
public IHttpActionResult Delete(int id)
{
    // Implementation
}
```

## Model/ViewModel Documentation

```csharp
/// <summary>
/// Represents [model description]
/// </summary>
public class YourModel
{
    /// <summary>
    /// Gets or sets the [property description]
    /// </summary>
    /// <example>12345</example>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the [property description]
    /// </summary>
    /// <example>John Doe</example>
    [Required]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the [property description]
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the [enum description]
    /// </summary>
    /// <example>Active</example>
    public AccountStatus Status { get; set; }
}
```

## Enum Documentation

```csharp
/// <summary>
/// Represents [enum description]
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Account is active and can be used for trading
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Account is temporarily suspended
    /// </summary>
    Suspended = 1,
    
    /// <summary>
    /// Account is permanently closed
    /// </summary>
    Closed = 2
}
```

## Common Response Codes

| Code | Description | Use When |
|------|-------------|----------|
| `200` | OK | Successful GET, PUT, DELETE |
| `201` | Created | Successful POST (resource created) |
| `204` | No Content | Successful DELETE (no content returned) |
| `400` | Bad Request | Invalid input/validation errors |
| `401` | Unauthorized | Authentication required/failed |
| `403` | Forbidden | Authenticated but not authorized |
| `404` | Not Found | Resource doesn't exist |
| `409` | Conflict | Resource already exists or conflicting state |
| `500` | Internal Server Error | Unexpected server error |

## Examples for MT5 Controllers

### LiveAccountController Example
```csharp
/// <summary>
/// Manages MT5 live trading accounts
/// </summary>
/// <remarks>
/// Provides endpoints for creating, updating, and managing live MT5 accounts.
/// All endpoints require authentication.
/// </remarks>
[RoutePrefix("api/liveaccount")]
public class LiveAccountController : BaseApiController
{
    /// <summary>
    /// Creates a new MT5 live trading account
    /// </summary>
    /// <param name="model">Account creation details including login, group, and leverage</param>
    /// <returns>Created account information with credentials</returns>
    /// <response code="200">Account created successfully</response>
    /// <response code="400">Invalid account parameters (e.g., invalid group, leverage)</response>
    /// <response code="401">Unauthorized - valid JWT token required</response>
    /// <response code="409">Account with specified login already exists</response>
    [HttpPost]
    [Route("create")]
    [Authorize]
    public IHttpActionResult CreateAccount([FromBody] Mt5LiveAccountVM model)
    {
        // Implementation
    }
}
```

### TradingHistoryController Example
```csharp
/// <summary>
/// Retrieves MT5 trading history and deal information
/// </summary>
[RoutePrefix("api/tradinghistory")]
public class LiveTradingHistoryController : BaseApiController
{
    /// <summary>
    /// Gets trading history for a specific MT5 account
    /// </summary>
    /// <param name="login">MT5 account login number</param>
    /// <param name="fromDate">Start date for history query (format: yyyy-MM-dd)</param>
    /// <param name="toDate">End date for history query (format: yyyy-MM-dd)</param>
    /// <returns>List of deals and orders for the specified period</returns>
    /// <response code="200">Trading history retrieved successfully</response>
    /// <response code="400">Invalid date range or login</response>
    /// <response code="404">Account not found</response>
    /// <response code="401">Unauthorized - authentication required</response>
    [HttpGet]
    [Route("account/{login}")]
    public IHttpActionResult GetHistory(long login, 
        [FromUri] string fromDate, 
        [FromUri] string toDate)
    {
        // Implementation
    }
}
```

### GroupController Example
```csharp
/// <summary>
/// Manages MT5 trading groups and their configurations
/// </summary>
[RoutePrefix("api/group")]
public class GroupController : BaseApiController
{
    /// <summary>
    /// Retrieves all available MT5 trading groups
    /// </summary>
    /// <returns>List of group configurations including symbols, leverage, and margin settings</returns>
    /// <response code="200">Groups retrieved successfully</response>
    /// <response code="401">Unauthorized - authentication required</response>
    /// <response code="500">Failed to connect to MT5 server</response>
    [HttpGet]
    [Route("list")]
    public IHttpActionResult GetAllGroups()
    {
        // Implementation
    }
}
```

## Tips for Good Documentation

### DO:
? Use clear, concise descriptions
? Include examples in `<example>` tags
? Document all parameters
? List all possible response codes
? Explain error conditions
? Use consistent terminology
? Document validation rules

### DON'T:
? Leave obvious things undocumented (every public API member needs docs)
? Copy-paste generic descriptions
? Forget to update docs when code changes
? Use internal/technical jargon in user-facing docs
? Skip error response documentation

## Adding Documentation to Existing Controllers

### Step 1: Identify Controllers
List all controllers that expose API endpoints:
- LiveAccountController
- DemoAccountController
- TradingHistoryController
- GroupController
- etc.

### Step 2: Template for Each Controller
1. Add controller-level `<summary>`
2. Add action-level `<summary>` for each public method
3. Document all parameters with `<param>`
4. Add `<response>` tags for all status codes

### Step 3: Verify in Swagger
1. Build project
2. Open Swagger UI
3. Check that descriptions appear
4. Test examples work

## Automated Documentation Tools

Consider these tools to help:
- **GhostDoc** - Auto-generates XML comments
- **Sandcastle** - Generates help files from XML
- **DocFX** - Microsoft's documentation generator

---

**Remember:** Good API documentation is essential for developer experience and reduces support overhead!
