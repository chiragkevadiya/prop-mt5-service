# MT5 Client Refactoring - Migration Guide

## Overview
The `Mt5LiveClient` and `Mt5DemoClient` classes have been refactored to follow modern C# practices with improved error handling, logging, and resource management.

## Key Changes

### 1. Constructor Changes
**Before:**
```csharp
var liveClient = new Mt5LiveClient();
var demoClient = new Mt5DemoClient();
```

**After:**
```csharp
// ILogger is now required (inject or create)
var logger = Log.Logger; // or inject ILogger
var liveClient = new Mt5LiveClient(logger);
var demoClient = new Mt5DemoClient(logger, @"C:\dll_dot\MT5Libs"); // optional path
```

### 2. Method Name Changes

#### Demo Client Methods
**Before:**
```csharp
demoClient.Initialize_demo();
demoClient.Connect_demo(server, login, password, timeout);
```

**After:**
```csharp
demoClient.Initialize();
demoClient.Connect(server, login, password, timeout);
```

#### Live Client Methods
**Before:**
```csharp
liveClient.Initialize();
liveClient.Connect(server, login, password, timeout);
```

**After:**
```csharp
// No change - same method names
liveClient.Initialize();
liveClient.Connect(server, login, password, timeout);
```

### 3. Field/Property Changes

**Before:**
```csharp
// Public fields with Hungarian notation
liveClient.m_manager
demoClient.m_manager_demo
```

**After:**
```csharp
// Protected property (access via inheritance only)
// For external code, inject/use the factory classes that wrap these clients
```

### 4. IDisposable Implementation

**Before:**
```csharp
var client = new Mt5LiveClient();
client.Initialize();
client.Connect(...);
// No cleanup mechanism
```

**After:**
```csharp
// Recommended: use 'using' statement
using (var client = new Mt5LiveClient(logger))
{
    client.Initialize();
    client.Connect(...);
    // Automatic cleanup
}

// Or manual disposal
var client = new Mt5LiveClient(logger);
try
{
    client.Initialize();
    client.Connect(...);
}
finally
{
    client.Dispose(); // or client.Disconnect()
}
```

### 5. Logging Changes

**Before:**
```csharp
Console.WriteLine("SMTManagerAPIFactory.Initialize failed - {0}", res);
```

**After:**
```csharp
// Automatic structured logging via Serilog
logger.Error("Live: SMTManagerAPIFactory.Initialize failed - {Result}", res);
```

## Migration Steps

### Step 1: Setup Logger in Your Application
```csharp
// In Startup.cs or Program.cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/mt5-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Step 2: Update Client Creation
Replace all instances of:
```csharp
var mt5Demo = new Mt5DemoClient();
var mt5Live = new Mt5LiveClient();
```

With:
```csharp
var mt5Demo = new Mt5DemoClient(Log.Logger);
var mt5Live = new Mt5LiveClient(Log.Logger);
```

### Step 3: Update Method Calls
Replace:
```csharp
// Demo client
client.Initialize_demo();
client.Connect_demo(server, login, password, timeout);
```

With:
```csharp
// Demo client
client.Initialize();
client.Connect(server, login, password, timeout);
```

### Step 4: Add Disposal Pattern
Wrap client usage in `using` statements:
```csharp
using (var client = new Mt5LiveClient(logger))
{
    var initResult = client.Initialize();
    if (initResult != MTRetCode.MT_RET_OK)
        return;
        
    var connectResult = client.Connect(server, login, password, timeout);
    if (connectResult != MTRetCode.MT_RET_OK)
        return;
        
    // Use client...
}
```

### Step 5: Update Direct Manager Access
If you were accessing `m_manager` or `m_manager_demo` directly:

**Before:**
```csharp
var manager = client.m_manager;
manager.SomeOperation();
```

**After:**
```csharp
// Option 1: Extend the client class with needed methods
public class ExtendedMt5LiveClient : Mt5LiveClient
{
    public ExtendedMt5LiveClient(ILogger logger) : base(logger) { }
    
    public void CustomOperation()
    {
        if (Manager != null)
        {
            Manager.SomeOperation();
        }
    }
}

// Option 2: Use factory classes that already wrap manager operations
// (Mt5ManagerFactory, Mt5DemoManagerFactory, etc.)
```

## Breaking Changes Summary

| Old Code | New Code | Notes |
|----------|----------|-------|
| `new Mt5LiveClient()` | `new Mt5LiveClient(logger)` | Logger required |
| `new Mt5DemoClient()` | `new Mt5DemoClient(logger)` | Logger required |
| `client.Initialize_demo()` | `client.Initialize()` | Method renamed |
| `client.Connect_demo(...)` | `client.Connect(...)` | Method renamed |
| `client.m_manager` | N/A | Use protected `Manager` property via inheritance |
| `client.m_manager_demo` | N/A | Use protected `Manager` property via inheritance |
| No disposal | `client.Dispose()` or `using` | IDisposable implemented |

## Benefits of Refactoring

1. **DRY Principle**: Common code moved to `Mt5ClientBase` base class
2. **Proper Logging**: Structured logging via Serilog instead of Console.WriteLine
3. **Resource Management**: IDisposable pattern ensures proper cleanup
4. **Better Error Handling**: Comprehensive exception handling and logging
5. **Testability**: Constructor injection enables better unit testing
6. **Modern C# Standards**: Removed Hungarian notation, added XML documentation
7. **Configuration**: Library path can be injected instead of hardcoded

## Testing Checklist

- [ ] All client creations updated with logger parameter
- [ ] All `Initialize_demo()` calls changed to `Initialize()`
- [ ] All `Connect_demo()` calls changed to `Connect()`
- [ ] `using` statements or manual `Dispose()` calls added
- [ ] Direct `m_manager`/`m_manager_demo` access refactored
- [ ] Logger properly configured in application startup
- [ ] Build succeeds without errors
- [ ] Runtime testing confirms connectivity works
- [ ] Log files are generated correctly

## Need Help?
If you encounter issues during migration, refer to `Examples\Mt5ClientUsageExample.cs` for complete usage examples.
