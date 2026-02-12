using MetaQuotes.MT5CommonAPI;
using PropMT5ConnectionService.Mt5Client;
using Serilog;
using System;

namespace PropMT5ConnectionService.Diagnostics
{
    /// <summary>
    /// MT5 Connection Diagnostic Tool
    /// Tests and verifies MT5 Live and Demo client connectivity
    /// </summary>
    public class Mt5ConnectionDiagnostics
    {
        private readonly ILogger _logger;

        public Mt5ConnectionDiagnostics()
        {
            // Create a basic logger for diagnostics
            // Note: Console sink requires Serilog.Sinks.Console package
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        /// <summary>
        /// Run complete diagnostic test
        /// </summary>
        public void RunDiagnostics(string libraryPath, string liveServer, ulong liveLogin, string livePassword,
            string demoServer = null, ulong demoLogin = 0, string demoPassword = null)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("  MT5 CONNECTION DIAGNOSTICS");
            Console.WriteLine("======================================");
            Console.WriteLine();

            // Test 1: Check library files
            TestLibraryFiles(libraryPath);

            // Test 2: Test Live Connection
            TestLiveConnection(libraryPath, liveServer, liveLogin, livePassword);

            // Test 3: Test Demo Connection (if provided)
            if (!string.IsNullOrEmpty(demoServer))
            {
                TestDemoConnection(libraryPath, demoServer, demoLogin, demoPassword);
            }
            else
            {
                Console.WriteLine("[SKIP] Demo connection test skipped (no configuration provided)");
            }

            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine("  DIAGNOSTICS COMPLETED");
            Console.WriteLine("======================================");
        }

        /// <summary>
        /// Test if MT5 library files exist
        /// </summary>
        private void TestLibraryFiles(string libraryPath)
        {
            Console.WriteLine("\n[TEST 1] Checking MT5 Library Files");
            Console.WriteLine($"Library Path: {libraryPath}");

            string[] requiredFiles = {
                "mtmanapi64.dll",
                "MT5CommonAPI.dll",
                "MT5ManagerAPI.dll"
            };

            bool allFilesExist = true;

            foreach (var file in requiredFiles)
            {
                string fullPath = System.IO.Path.Combine(libraryPath, file);
                bool exists = System.IO.File.Exists(fullPath);

                Console.WriteLine($"  {(exists ? "[OK]" : "[MISSING]")} {file}");

                if (!exists)
                {
                    allFilesExist = false;
                }
            }

            if (!allFilesExist)
            {
                Console.WriteLine("\n[ERROR] Some required MT5 library files are missing!");
                Console.WriteLine("  Solution:");
                Console.WriteLine($"  1. Ensure MT5 Manager API is installed");
                Console.WriteLine($"  2. Copy required DLLs to: {libraryPath}");
                Console.WriteLine($"  3. Verify file permissions");
            }
            else
            {
                Console.WriteLine("\n[SUCCESS] All required library files found!");
            }
        }

        /// <summary>
        /// Test Live MT5 connection
        /// </summary>
        private void TestLiveConnection(string libraryPath, string server, ulong login, string password)
        {
            Console.WriteLine("\n[TEST 2] Testing MT5 Live Connection");
            Console.WriteLine($"  Server: {server}");
            Console.WriteLine($"  Login: {login}");
            Console.WriteLine($"  Library: {libraryPath}");

            Mt5LiveClient client = null;

            try
            {
                // Create client
                Console.WriteLine("\n  Step 1: Creating Mt5LiveClient...");
                client = new Mt5LiveClient(_logger, libraryPath);
                Console.WriteLine("  [OK] Client created");

                // Initialize
                Console.WriteLine("\n  Step 2: Initializing MT5 Manager API...");
                var initResult = client.Initialize();

                if (initResult != MTRetCode.MT_RET_OK)
                {
                    Console.WriteLine($"  [FAILED] Initialization failed: {initResult}");
                    DiagnoseInitializationError(initResult, libraryPath);
                    return;
                }

                Console.WriteLine("  [OK] Initialization successful");

                // Connect
                Console.WriteLine("\n  Step 3: Connecting to server...");
                var connectResult = client.Connect(server, login, password, 30000);

                if (connectResult != MTRetCode.MT_RET_OK)
                {
                    Console.WriteLine($"  [FAILED] Connection failed: {connectResult}");
                    DiagnoseConnectionError(connectResult, server);
                    return;
                }

                Console.WriteLine("  [OK] Connection successful!");
                Console.WriteLine("\n[SUCCESS] MT5 Live client is fully functional!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[EXCEPTION] Unexpected error: {ex.Message}");
                Console.WriteLine($"  Type: {ex.GetType().Name}");
                Console.WriteLine($"  Stack: {ex.StackTrace}");
            }
            finally
            {
                if (client != null)
                {
                    Console.WriteLine("\n  Cleaning up connection...");
                    client.Dispose();
                    Console.WriteLine("  [OK] Cleanup complete");
                }
            }
        }

        /// <summary>
        /// Test Demo MT5 connection
        /// </summary>
        private void TestDemoConnection(string libraryPath, string server, ulong login, string password)
        {
            Console.WriteLine("\n[TEST 3] Testing MT5 Demo Connection");
            Console.WriteLine($"  Server: {server}");
            Console.WriteLine($"  Login: {login}");

            Mt5DemoClient client = null;

            try
            {
                client = new Mt5DemoClient(_logger, libraryPath);
                var initResult = client.Initialize();

                if (initResult != MTRetCode.MT_RET_OK)
                {
                    Console.WriteLine($"  [FAILED] Initialization failed: {initResult}");
                    return;
                }

                var connectResult = client.Connect(server, login, password, 30000);

                if (connectResult != MTRetCode.MT_RET_OK)
                {
                    Console.WriteLine($"  [FAILED] Connection failed: {connectResult}");
                    return;
                }

                Console.WriteLine("[SUCCESS] MT5 Demo client is fully functional!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Error: {ex.Message}");
            }
            finally
            {
                client?.Dispose();
            }
        }

        /// <summary>
        /// Diagnose initialization errors
        /// </summary>
        private void DiagnoseInitializationError(MTRetCode error, string libraryPath)
        {
            Console.WriteLine("\n  DIAGNOSIS:");

            switch (error)
            {
                case MTRetCode.MT_RET_ERR_NOTFOUND:
                    Console.WriteLine("  - MT5 Manager API library not found");
                    Console.WriteLine($"  - Check if DLLs exist in: {libraryPath}");
                    Console.WriteLine("  - Required: mtmanapi64.dll, MT5CommonAPI.dll, MT5ManagerAPI.dll");
                    break;

                case MTRetCode.MT_RET_ERR_MEM:
                    Console.WriteLine("  - Memory allocation error");
                    Console.WriteLine("  - Possible: DLL version mismatch");
                    Console.WriteLine("  - Try: Reinstall MT5 Manager API");
                    break;

                case MTRetCode.MT_RET_ERROR:
                    Console.WriteLine("  - General initialization error");
                    Console.WriteLine("  - Possible causes:");
                    Console.WriteLine("    * Wrong platform (x64 vs x86)");
                    Console.WriteLine("    * Missing dependencies");
                    Console.WriteLine("    * Corrupted DLL files");
                    break;

                default:
                    Console.WriteLine($"  - Unknown error code: {error}");
                    Console.WriteLine("  - Contact MT5 support for assistance");
                    break;
            }
        }

        /// <summary>
        /// Diagnose connection errors
        /// </summary>
        private void DiagnoseConnectionError(MTRetCode error, string server)
        {
            Console.WriteLine("\n  DIAGNOSIS:");

            switch (error)
            {
                case MTRetCode.MT_RET_ERR_CONNECTION:
                    Console.WriteLine("  - Network connection failed");
                    Console.WriteLine($"  - Cannot reach server: {server}");
                    Console.WriteLine("  - Check:");
                    Console.WriteLine("    * Server address and port are correct");
                    Console.WriteLine("    * Firewall allows connection");
                    Console.WriteLine("    * Internet connection is working");
                    break;

                case MTRetCode.MT_RET_ERR_TIMEOUT:
                    Console.WriteLine("  - Connection timeout");
                    Console.WriteLine("  - Server is too slow or unreachable");
                    Console.WriteLine("  - Try increasing timeout value");
                    break;

                case MTRetCode.MT_RET_ERROR:
                    Console.WriteLine("  - Authentication or connection error");
                    Console.WriteLine("  - Possible causes:");
                    Console.WriteLine("    * Invalid login credentials");
                    Console.WriteLine("    * Account doesn't have manager permissions");
                    Console.WriteLine("    * Account is disabled or locked");
                    Console.WriteLine("    * Server rejected connection");
                    break;

                default:
                    Console.WriteLine($"  - Error code: {error}");
                    Console.WriteLine("  - Review MT5 server logs for details");
                    Console.WriteLine("  - Check credentials and account permissions");
                    break;
            }
        }

        /// <summary>
        /// Quick test with default settings
        /// </summary>
        public static void QuickTest()
        {
            Console.WriteLine("Enter MT5 connection details:\n");

            Console.Write("Library Path [C:\\dll_dot\\MT5Libs]: ");
            string libraryPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(libraryPath))
                libraryPath = @"C:\dll_dot\MT5Libs";

            Console.Write("Live Server (host:port): ");
            string liveServer = Console.ReadLine();

            Console.Write("Live Login: ");
            ulong liveLogin = ulong.Parse(Console.ReadLine());

            Console.Write("Live Password: ");
            string livePassword = Console.ReadLine();

            var diagnostics = new Mt5ConnectionDiagnostics();
            diagnostics.RunDiagnostics(libraryPath, liveServer, liveLogin, livePassword);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
