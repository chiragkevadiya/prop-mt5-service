using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Services;
using System;

namespace PropMT5ConnectionService.Helpers
{
    public static class Mt5ManagerFactory
    {
        private static CIMTManagerAPI _managerInstance { get; set; }

        public static void InitializeManager(CIMTManagerAPI cIMT)
        {
            _managerInstance = cIMT;
        }

        public static CIMTManagerAPI GetManager()
        {
            return _managerInstance;
        }
    }

    public class Mt5LiquidationFactory
    {
        // FIX: Corrected typo from ILiquidationService to ILiquidationService
        private ILiquidationService _liquidationInstance;

        // Renamed method for clarity
        public Mt5LiquidationFactory(ILiquidationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "ILiquidationService cannot be null during initialization.");
            }
            _liquidationInstance = service;
        }

        // Renamed method for clarity
        public ILiquidationService GetLiquidationService()
        {
            if (_liquidationInstance == null)
            {
                throw new InvalidOperationException("ILiquidationService instance has not been initialized.");
            }
            return _liquidationInstance;
        }
    }
}
