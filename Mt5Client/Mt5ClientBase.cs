using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using System;

namespace PropMT5Service.Mt5Client
{
    /// <summary>
    /// Base class for MT5 client connections providing common initialization and connection logic
    /// </summary>
    public abstract class Mt5ClientBase : IDisposable
    {
        private readonly string _libraryPath;
        private bool _disposed;

        // Stored so Reconnect() can re-use the same credentials
        private string _server;
        private ulong _login;
        private string _password;
        private uint _timeout;

        protected CIMTManagerAPI Manager { get; private set; }
        protected abstract string ClientType { get; }

        protected Mt5ClientBase(string libraryPath)
        {
            _libraryPath = libraryPath ?? throw new ArgumentNullException(nameof(libraryPath));
        }

        /// <summary>
        /// Initialize the MT5 Manager API factory and create manager instance
        /// </summary>
        public MTRetCode Initialize()
        {
            try
            {

                // Initialize the factory
                var result = SMTManagerAPIFactory.Initialize(_libraryPath);
                if (result != MTRetCode.MT_RET_OK)
                {
                    return result;
                }

                // Get and validate version
                result = SMTManagerAPIFactory.GetVersion(out uint version);
                if (result != MTRetCode.MT_RET_OK)
                {
                    return result;
                }

                if (version != SMTManagerAPIFactory.ManagerAPIVersion)
                {
                    return MTRetCode.MT_RET_ERROR;
                }

                // Create manager instance
                Manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out result);
                if (result != MTRetCode.MT_RET_OK)
                {
                    return result;
                }

                if (Manager == null)
                {
                    return MTRetCode.MT_RET_ERR_MEM;
                }

                return MTRetCode.MT_RET_OK;
            }
            catch (Exception ex)
            {
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
                    return MTRetCode.MT_RET_ERROR;
                }


                var result = Manager.Connect(server, login, password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, timeout);

                if (result != MTRetCode.MT_RET_OK)
                {
                    return result;
                }

                // Initialize factory-specific manager only after successful connection
                InitializeManagerFactory();

                // Store credentials for future reconnects
                _server = server;
                _login = login;
                _password = password;
                _timeout = timeout;

                return MTRetCode.MT_RET_OK;
            }
            catch (Exception ex)
            {
                return MTRetCode.MT_RET_ERROR;
            }
        }

        /// <summary>
        /// Disconnect then reconnect using the credentials from the last successful Connect() call.
        /// </summary>
        public MTRetCode Reconnect()
        {
            if (string.IsNullOrEmpty(_server))
            {
                return MTRetCode.MT_RET_ERROR;
            }

            Disconnect();
            return Connect(_server, _login, _password, _timeout);
        }

        /// <summary>
        /// Request server logs with specified parameters
        /// </summary>
        protected void RequestServerLogs(EnMTLogRequestMode requestMode, EnMTLogType logType, long from, long to, string filter = null)
        {
            if (Manager == null)
            {
                return;
            }

            try
            {

                var records = Manager.LoggerServerRequest(requestMode, logType, from, to, filter, out var result);

                if (result == MTRetCode.MT_RET_OK && records != null)
                {

                    foreach (var record in records)
                    {
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
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
                    Manager.Disconnect();
                }
                catch (Exception ex)
                {
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
                    }
                    Manager = null;
                }
            }

            _disposed = true;
        }
    }
}
