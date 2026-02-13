# Auto-Open Browser Feature

## Overview
The application now automatically opens the default web browser to `http://localhost:8086/welcome` when the service starts in both **Development** and **Production** environments.

## Changes Made

### 1. **WebServer.cs**
- Added `using System.Diagnostics;` for Process management
- Added `OpenBrowserToWelcomePage()` method that:
  - Constructs the welcome URL from the base URI
  - Detects the current environment (Development/Production)
  - Checks the `AutoOpenBrowser` configuration setting
  - Opens the default browser using `Process.Start()` with `UseShellExecute = true`
  - Includes error handling to prevent startup failures
- Modified `Start()` method to call `OpenBrowserToWelcomePage()` after successful server startup

### 2. **appsettings.Development.json**
- Added `"AutoOpenBrowser": "true"` to the `WebServer` section
- Browser will automatically open when running in Development mode

### 3. **appsettings.Production.json**
- Added `"AutoOpenBrowser": "true"` to the `WebServer` section
- Browser will automatically open when running in Production mode

## Configuration

### Enable/Disable Auto-Open
To control the auto-open behavior, modify the configuration file:

**Enable (default):**
```json
"WebServer": {
  "BaseUri": "http://localhost:8086",
  "AutoOpenBrowser": "true"
}
```

**Disable:**
```json
"WebServer": {
  "BaseUri": "http://localhost:8086",
  "AutoOpenBrowser": "false"
}
```

If the `AutoOpenBrowser` setting is missing, the default behavior is to **open the browser**.

## How It Works

1. **Service Starts** ? TopShelf initializes the WebServer
2. **OWIN Application Starts** ? Web API becomes available at `http://localhost:8086`
3. **Background Jobs Start** ? Liquidation monitoring begins
4. **Browser Opens** ? Default browser opens to `http://localhost:8086/welcome`

## Environment Detection

The feature automatically detects the environment:
- Reads `DOTNET_ENVIRONMENT` environment variable
- Defaults to "Development" if not set
- Logs the environment in console output

## Error Handling

- If browser opening fails, the service continues running normally
- Errors are logged as warnings (not fatal errors)
- Console displays: `[WARNING] Could not automatically open browser: {error message}`

## Testing

### Development Environment:
```bash
# Run the service
dotnet run

# Or if using Topshelf:
PropMT5ConnectionService.exe

# Browser should automatically open to:
# http://localhost:8086/welcome
```

### Production Environment:
```bash
# Set environment variable
set DOTNET_ENVIRONMENT=Production

# Run the service
PropMT5ConnectionService.exe

# Browser should automatically open to:
# http://localhost:8086/welcome
```

### Disable Auto-Open (if needed):
```json
"WebServer": {
  "AutoOpenBrowser": "false"
}
```

## Console Output

When the browser opens successfully, you'll see:
```
[INFO] WebServer started at http://localhost:8086
[INFO] Opening browser to: http://localhost:8086/welcome
```

## Benefits

? **Developer Experience:** No need to manually open browser and type URL  
? **QA Testing:** Automatically opens to welcome page after deployment  
? **Production Ready:** Works in both Development and Production environments  
? **Configurable:** Can be disabled via configuration  
? **Safe:** Errors don't prevent service startup  

## Browser Compatibility

Works with all major browsers:
- Google Chrome
- Microsoft Edge
- Firefox
- Safari
- Internet Explorer (legacy)

The default browser set in Windows will be used.

## Troubleshooting

### Browser doesn't open:
1. Check the console for warning messages
2. Verify `AutoOpenBrowser` is set to `"true"`
3. Ensure no firewall is blocking the port 8086
4. Try opening `http://localhost:8086/welcome` manually

### Service starts but browser shows error:
1. Verify the WebServer actually started (check console logs)
2. Ensure no other application is using port 8086
3. Check the `/health` endpoint: `http://localhost:8086/health`

---

**Created:** January 2025  
**Version:** 1.0  
**Author:** GitHub Copilot
