using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MT5ConnectionService.ClientMT5;
using PropMT5ConnectionService.Configuration;
using PropMT5ConnectionService.Services.Interfaces;
using System;

namespace PropMT5ConnectionService.Services.Implementations
{
    public class MT5ConnectionService : IMT5ConnectionService
    {
        private readonly MT5ConnectionSettings _settings;
        private CIMTManagerAPI _liveManager;
        private CIMTManagerAPI _demoManager;
        private readonly object _liveLock = new object();
        private readonly object _demoLock = new object();

        public MT5ConnectionService(IOptions<MT5ConnectionSettings> settings)
        {
            _settings = settings.Value;
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            // Initialize Live Manager
            var liveConnector = new ClientConnect();
            var initResult = liveConnector.Initialize();
            if (initResult != MTRetCode.MT_RET_OK)
            {
                throw new InvalidOperationException($"FATAL: MT5 Live API Factory initialization failed: {initResult}");
            }

            var connectResult = liveConnector.Connect(
                _settings.Live.Server,
                _settings.Live.Login,
                _settings.Live.Password,
                _settings.Live.Timeout);

            if (connectResult != MTRetCode.MT_RET_OK)
            {
                throw new InvalidOperationException($"FATAL: MT5 Live Manager Login failed: {connectResult}");
            }

            _liveManager = liveConnector.m_manager;
            Console.WriteLine("[INFO] Live MT5 Manager connected successfully");

            // Initialize Demo Manager
            var demoConnector = new ClientConnectDemo();
            var demoInitResult = demoConnector.Initialize_demo();
            if (demoInitResult != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine($"[WARNING] MT5 Demo API Factory initialization failed: {demoInitResult}");
                return;
            }

            var demoConnectResult = demoConnector.Connect_demo(
                _settings.Demo.Server,
                _settings.Demo.Login,
                _settings.Demo.Password,
                _settings.Demo.Timeout);

            if (demoConnectResult != MTRetCode.MT_RET_OK)
            {
                Console.WriteLine($"[WARNING] MT5 Demo Manager Login failed: {demoConnectResult}");
                return;
            }

            _demoManager = demoConnector.m_manager;
            Console.WriteLine("[INFO] Demo MT5 Manager connected successfully");
        }

        public CIMTManagerAPI GetLiveManager()
        {
            lock (_liveLock)
            {
                if (_liveManager == null)
                {
                    throw new InvalidOperationException("Live Manager is not initialized");
                }
                return _liveManager;
            }
        }

        public CIMTManagerAPI GetDemoManager()
        {
            lock (_demoLock)
            {
                if (_demoManager == null)
                {
                    throw new InvalidOperationException("Demo Manager is not initialized");
                }
                return _demoManager;
            }
        }

        public bool IsConnected(bool isDemo = false)
        {
            var manager = isDemo ? _demoManager : _liveManager;
            return manager != null;
        }

        public MTRetCode Reconnect(bool isDemo = false)
        {
            // TODO: Implement reconnection logic
            throw new NotImplementedException("Reconnection logic not yet implemented");
        }
    }
}
