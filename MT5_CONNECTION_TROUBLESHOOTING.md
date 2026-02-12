# MT5 Connection Troubleshooting Guide

## Problem
Mt5DemoClient and Mt5LiveClient files missing or not connecting to MT5 server.

---

## ? Status Check

### Files Verification

| File | Status | Location |
|------|--------|----------|
| `Mt5ClientBase.cs` | ? EXISTS | `Mt5Client\Mt5ClientBase.cs` |
| `Mt5LiveClient.cs` | ? EXISTS | `Mt5Client\Mt5LiveClient.cs` |
| `Mt5DemoClient.cs` | ? EXISTS | `Mt5Client\Mt5DemoClient.cs` |
| **Compilation** | ? **NO ERRORS** | All files compile successfully |

**Conclusion:** Files are NOT missing. They exist and compile correctly.

---

## Connection Issues - Root Causes

### 1. **Missing MT5 Library Files**

**Symptoms:**
- Initialization fails
- "DLL not found" errors
- MTRetCode.MT_RET_ERR_NOTFOUND

**Check:**
```powershell
# Verify MT5 libraries exist:
dir C:\dll_dot\MT5Libs\*.dll

# Required files:
# - mtmanapi64.dll
# - MT5CommonAPI.dll
# - MT5ManagerAPI.dll
```

**Solution:**
1. Install MT5 Manager API
2. Copy DLLs to `C:\dll_dot\MT5Libs`
3. Or update `appsettings.json` with correct path:
```json
{
  "MT5": {
    "LibraryPath": "C:\\your\\actual\\path\\to\\MT5Libs"
  }
}
```

---

### 2. **Wrong Server Configuration**

**Symptoms:**
- Connection timeout
- "Cannot reach server" errors
- MTRetCode.MT_RET_ERR_CONNECTION

**Check appsettings.json:**
```json
{
  "MT5": {
    "Live": {
      "Server": "your-server.com:443",  ? Must be correct
      "Login": "12345678",               ? Manager account
      "Password": "your-password",       ? Correct password
      "Timeout": "30000"
    }
  }
}
```

**Common Issues:**
- ? Wrong port (should be 443 for HTTPS, or specific MT5 port)
- ? Missing port number (e.g., `server.com` instead of `server.com:443`)
- ? Wrong server address
- ? Firewall blocking connection

**Test Connection:**
```powershell
# Test if server is reachable:
Test-NetConnection -ComputerName your-server.com -Port 443

# Should show: TcpTestSucceeded : True
```

---

### 3. **Authentication Failure**

**Symptoms:**
- MTRetCode.MT_RET_AUTH_FAILED
- "Invalid credentials"
- Connection closes immediately

**Check:**
- ? Using **Manager** account (not trader account)
- ? Password is correct
- ? Account has proper permissions
- ? Account is not disabled/locked

**Test Credentials:**
```powershell
# Use the diagnostic tool:
.\TestMT5Connection.exe
# Enter your credentials to test
```

---

### 4. **Configuration File Missing**

**Symptoms:**
- FileNotFoundException: appsettings.json not found
- Service crashes on startup

**Solution:**
```powershell
# Copy configuration to output directory:
copy appsettings.json bin\Debug\appsettings.json /Y

# Or run:
.\copy-config.bat
```

---

### 5. **Serilog Logger Not Configured**

**Symptoms:**
- NullReferenceException when creating clients
- "Logger cannot be null"

**Solution:**
Ensure logger is initialized in `Program.cs`:
```csharp
// In ConfigureSerilog():
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateLogger();

// In ConfigureServices():
services.AddSingleton(Log.Logger);
```

---

## ?? Diagnostic Tools

### Tool 1: Connection Test Utility

Run the test utility to diagnose connection issues:

```powershell
# Build the diagnostic tool (if not exists):
# Add TestConnection\TestMT5Connection.cs to project

# Run interactive test:
.\bin\Debug\PropMT5ConnectionService.exe

# Or use diagnostics class directly
```

### Tool 2: Manual Verification

```powershell
# Check files:
dir Mt5Client\*.cs

# Should show:
# Mt5ClientBase.cs
# Mt5DemoClient.cs
# Mt5LiveClient.cs

# Check compilation:
dotnet build

# Should show: Build succeeded
```

### Tool 3: Check Dependencies

```powershell
# Verify MT5 DLLs are loaded:
dumpbin /dependents bin\Debug\PropMT5ConnectionService.exe

# Or check at runtime:
[System.AppDomain]::CurrentDomain.GetAssemblies() | 
  Where-Object {$_.FullName -like "*MT5*"} | 
  Select-Object FullName
```

---

## ?? Step-by-Step Connection Test

### Step 1: Verify Files Exist
```powershell
dir Mt5Client\Mt5*.cs
# Should show 3 files
```

### Step 2: Check MT5 Libraries
```powershell
dir C:\dll_dot\MT5Libs\*.dll
# Should show mtmanapi64.dll and others
```

### Step 3: Verify Configuration
```powershell
# Check appsettings.json exists in output:
type bin\Debug\appsettings.json

# Verify MT5 settings are correct
```

### Step 4: Test Initialization
```powershell
# Run the service with verbose logging:
$env:DOTNET_ENVIRONMENT="Development"
.\bin\Debug\PropMT5ConnectionService.exe

# Watch for initialization messages
```

### Step 5: Check Connection Logs
Look for these messages in console output:
```
[INFO] Connecting to MT5 Live server your-server.com:443 with login 12345678
[INFO] Successfully connected to MT5 Live server  ? Success!
```

Or error messages:
```
[FATAL] MT5 Live Client initialization failed: MT_RET_ERR_NOTFOUND
[FATAL] MT5 Live Client connection failed: MT_RET_ERR_CONNECTION
```

---

## ?? Quick Fixes

### Fix 1: Copy Config Files
```batch
copy-config.bat
```

### Fix 2: Update appsettings.json
```json
{
  "MT5": {
    "LibraryPath": "C:\\dll_dot\\MT5Libs",
    "Live": {
      "Server": "CORRECT-SERVER:443",
      "Login": "CORRECT-LOGIN",
      "Password": "CORRECT-PASSWORD",
      "Timeout": "30000"
    }
  }
}
```

### Fix 3: Verify MT5 Libraries
```powershell
# Check if libraries exist:
Test-Path "C:\dll_dot\MT5Libs\mtmanapi64.dll"

# If False, install MT5 Manager API or update path
```

### Fix 4: Test Network Connection
```powershell
# Test server reachability:
Test-NetConnection -ComputerName your-mt5-server.com -Port 443

# Check firewall:
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*MT5*"}
```

---

## ?? Common Error Codes

| Error Code | Meaning | Solution |
|------------|---------|----------|
| `MT_RET_ERR_NOTFOUND` | DLL not found | Install MT5 Manager API |
| `MT_RET_ERR_CONNECTION` | Cannot reach server | Check server address, firewall |
| `MT_RET_AUTH_FAILED` | Invalid credentials | Verify login/password |
| `MT_RET_ERR_TIMEOUT` | Connection timeout | Increase timeout, check network |
| `MT_RET_ERR_MEM` | Memory error | Reinstall MT5 API, check DLL versions |
| `MT_RET_ERROR` | General error | Check logs for details |

---

## ? Verification Checklist

Before reporting issues, verify:

- [ ] All 3 MT5 client files exist and compile
- [ ] MT5 Manager API DLLs are installed
- [ ] `appsettings.json` exists in `bin\Debug`
- [ ] MT5 server address is correct
- [ ] MT5 login is a **manager** account
- [ ] Password is correct
- [ ] Firewall allows connection
- [ ] Network can reach MT5 server
- [ ] Serilog is configured
- [ ] Configuration file is copied to output

---

## ?? Recommended Workflow

1. **Run diagnostics:**
```powershell
.\bin\Debug\PropMT5ConnectionService.exe
```

2. **Check console output for errors**

3. **If initialization fails:**
   - Verify MT5 DLLs exist
   - Check library path in config

4. **If connection fails:**
   - Verify server address
   - Test network connectivity
   - Check credentials

5. **If still failing:**
   - Use diagnostic tool
   - Check firewall logs
   - Contact MT5 server administrator

---

## ?? Getting Help

If issues persist, provide this information:

1. **Error message** (exact text)
2. **MTRetCode** returned
3. **Configuration** (hide password):
```json
{
  "MT5": {
    "LibraryPath": "C:\\dll_dot\\MT5Libs",
    "Live": {
      "Server": "your-server.com:443",
      "Login": "12345678"
    }
  }
}
```
4. **File verification:**
```powershell
dir Mt5Client\*.cs
dir C:\dll_dot\MT5Libs\*.dll
```

---

## Summary

**Files Status:** ? All files exist and compile  
**Connection Status:** ? Depends on configuration  
**Action Required:** Configure appsettings.json with correct MT5 credentials

**The files are NOT missing. Connection issues are due to configuration or MT5 library setup.**
