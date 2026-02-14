# PropMT5ConnectionService - ClickOnce Deployment Fix

## Problem
The application was failing to find `appsettings.json` when deployed as a Windows Service via ClickOnce:
```
[FATAL] Application terminated unexpectedly: The configuration file 'appsettings.json' was not found and is not optional. The expected physical path was 'C:\Users\amitl\AppData\Local\Apps\2.0\NDT9OR17.NNE\1C5GQYPG.MQ5\prop..tion_0000000000000000_0001.0000_43aa0614e9184c88\appsettings.json'.
```

## Root Causes
1. **ClickOnce Cache Directory Issue**: The application was looking for configuration files in the ClickOnce temporary cache directory, where they weren't being deployed.
2. **Missing PublishFile Metadata**: The `.csproj` file didn't have `PublishFile` entries to ensure the configuration files were included in the ClickOnce deployment manifest.
3. **Fixed Base Path Detection**: The original code only checked one base path.

## Solution Implemented

### 1. Enhanced Program.cs - Robust Configuration Loading
Updated `Program.cs` to:
- Implement `LoadConfiguration()` method that searches multiple base paths:
  1. Executable location (standard deployment)
  2. ClickOnce data directory (searched in AppData\Local\Apps)
  3. Current working directory
  4. Published location (`C:\MT5WindowsService\prop_mt5\`)
  
- Implement `GetClickOnceDataDirectory()` to detect and return ClickOnce deployment directories
- Provide detailed error messages showing all searched paths if config not found
- Log the found configuration file path for debugging

### 2. Updated PropMT5ConnectionService.csproj
Added `PublishFile` ItemGroup with entries for:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Production.json`

Each entry includes:
- `PublishState: Include` - Ensures file is published
- `IncludeHash: True` - Validates file integrity
- `FileType: File` - Marks as data file
- `Visible: False` - Hidden from file view

## Deployment Instructions

### For ClickOnce Deployment:
1. **Clean and Rebuild**: Clean the solution, then rebuild it completely
2. **Publish Updates**: 
   - In Visual Studio, right-click project ? "Publish"
   - Increment the ApplicationRevision in the project properties
   - Publish to your configured URL (`C:\MT5WindowsService\prop_mt5\`)

3. **Verify Deployment**:
   - The ClickOnce manifest should now include the appsettings.json files
   - Users will receive an update notification
   - Application will start and find the configuration files in the correct location

### For Direct Deployment:
- Place `appsettings.json` and environment-specific config files in the same directory as the executable
- The application will automatically detect and use them

## Testing
After redeployment:
1. Start the service and check for the log message:
   ```
   [INFO] Found appsettings.json at: <path>
   ```
2. Verify service starts without the FileNotFoundException
3. Check that the service is running successfully:
   ```
   Get-Service PropMT5ConnectionService
   ```

## Files Modified
- `Program.cs` - Added multi-path configuration loading logic
- `PropMT5ConnectionService.csproj` - Added PublishFile metadata for ClickOnce deployment
