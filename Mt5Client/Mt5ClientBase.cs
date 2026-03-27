using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Serilog;
using System;

namespace PropMT5ConnectionService.Mt5Client
{
    /// <summary>
    /// Base class for MT5 client connections providing common initialization and connection logic
    /// </summary>
    public abstract class Mt5ClientBase : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _libraryPath;
        private bool _disposed;

        protected CIMTManagerAPI Manager { get; private set; }
        protected abstract string ClientType { get; }

        protected Mt5ClientBase(ILogger logger, string libraryPath)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _libraryPath = libraryPath ?? throw new ArgumentNullException(nameof(libraryPath));
        }

        /// <summary>
        /// Initialize the MT5 Manager API factory and create manager instance
        /// </summary>
        public MTRetCode Initialize()
        {
            try
            {
                _logger.Information("{ClientType}: Initializing MT5 Manager API from {LibraryPath}", ClientType, _libraryPath);

                // Initialize the factory
                var result = SMTManagerAPIFactory.Initialize(_libraryPath);
                if (result != MTRetCode.MT_RET_OK)
                {
                    _logger.Error("{ClientType}: SMTManagerAPIFactory.Initialize failed - {Result}", ClientType, result);
                    return result;
                }

                // Get and validate version
                result = SMTManagerAPIFactory.GetVersion(out uint version);
                if (result != MTRetCode.MT_RET_OK)
                {
                    _logger.Error("{ClientType}: SMTManagerAPIFactory.GetVersion failed - {Result}", ClientType, result);
                    return result;
                }

                if (version != SMTManagerAPIFactory.ManagerAPIVersion)
                {
                    _logger.Error("{ClientType}: Manager API version mismatch - {ActualVersion} != {ExpectedVersion}",
                        ClientType, version, SMTManagerAPIFactory.ManagerAPIVersion);
                    return MTRetCode.MT_RET_ERROR;
                }

                // Create manager instance
                Manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out result);
                if (result != MTRetCode.MT_RET_OK)
                {
                    _logger.Error("{ClientType}: SMTManagerAPIFactory.CreateManager failed - {Result}", ClientType, result);
                    return result;
                }

                if (Manager == null)
                {
                    _logger.Error("{ClientType}: SMTManagerAPIFactory.CreateManager returned OK but ManagerAPI is null", ClientType);
                    return MTRetCode.MT_RET_ERR_MEM;
                }

                _logger.Information("{ClientType}: Using ManagerAPI v.{Version}", ClientType, version);
                return MTRetCode.MT_RET_OK;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClientType}: Exception during initialization", ClientType);
                return MTRetCode.MT_RET_ERROR;
            }
        }

        /// <summary>
        /// Connect to MT5 server with specified credentials
        /// </summary>
        public MTRetCode Connect(string server, ulong login, string password, uint timeout)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentException("Server cannot be null or empty", nameof(server));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            try
            {
                if (Manager == null)
                {
                    _logger.Error("{ClientType}: Connection to {Server} failed - Manager API is NULL", ClientType, server);
                    return MTRetCode.MT_RET_ERROR;
                }

                _logger.Information("{ClientType}: Connecting to {Server} with login {Login}", ClientType, server, login);

                var result = Manager.Connect(server, login, password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, timeout);

                if (result != MTRetCode.MT_RET_OK)
                {
                    _logger.Error("{ClientType}: Connection to {Server} failed - {Result}", ClientType, server, result);
                    return result;
                }

                // Initialize factory-specific manager only after successful connection
                InitializeManagerFactory();

                _logger.Information("{ClientType}: Successfully connected to {Server}", ClientType, server);
                return MTRetCode.MT_RET_OK;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClientType}: Exception during connection to {Server}", ClientType, server);
                return MTRetCode.MT_RET_ERROR;
            }
        }

        /// <summary>
        /// Request server logs with specified parameters
        /// </summary>
        protected void RequestServerLogs(EnMTLogRequestMode requestMode, EnMTLogType logType, long from, long to, string filter = null)
        {
            if (Manager == null)
            {
                _logger.Error("{ClientType}: Cannot request logs - Manager was not created", ClientType);
                return;
            }

            try
            {
                _logger.Debug("{ClientType}: Requesting server logs - Mode: {Mode}, Type: {Type}", ClientType, requestMode, logType);

                var records = Manager.LoggerServerRequest(requestMode, logType, from, to, filter, out var result);

                if (result == MTRetCode.MT_RET_OK && records != null)
                {
                    _logger.Information("{ClientType}: LoggerServerRequest succeeded - returned {Count} record(s)",
                        ClientType, records.Length);

                    foreach (var record in records)
                    {
                        _logger.Debug("{ClientType}: Log record - {Record}", ClientType, record);
                    }
                }
                else
                {
                    _logger.Warning("{ClientType}: LoggerServerRequest failed - {Result}, returned {Count} record(s)",
                        ClientType, result, records?.Length ?? 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClientType}: Exception during server log request", ClientType);
            }
        }

        /// <summary>
        /// Initialize the specific manager factory (Live or Demo)
        /// </summary>
        protected abstract void InitializeManagerFactory();

        /// <summary>
        /// Get the MT5 Manager API instance
        /// </summary>
        /// <returns>CIMTManagerAPI instance or null if not initialized</returns>
        public CIMTManagerAPI GetManager()
        {
            return Manager;
        }

        /// <summary>
        /// Check if the client is connected to the server
        /// </summary>
        public bool IsConnected()
        {
            return Manager != null;
        }

        /// <summary>
        /// Disconnect from server if connected
        /// </summary>
        public void Disconnect()
        {
            if (Manager != null)
            {
                try
                {
                    _logger.Information("{ClientType}: Disconnecting from server", ClientType);
                    Manager.Disconnect();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "{ClientType}: Exception during disconnect", ClientType);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Disconnect();

                if (Manager != null)
                {
                    try
                    {
                        Manager.Release();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "{ClientType}: Exception during manager release", ClientType);
                    }
                    Manager = null;
                }
            }

            _disposed = true;
        }
    }
}
