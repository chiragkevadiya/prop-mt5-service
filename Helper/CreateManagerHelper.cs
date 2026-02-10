using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Services;
using System;

namespace MT5ConnectionService.Helper
{
    public static class CreateManagerHelper
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

    public class CreateLiquidationHelper
    {
        // FIX: Corrected typo from ILiqudationService to ILiquidationService
        private ILiqudationService _liquidationInstance;

        // Renamed method for clarity
        public CreateLiquidationHelper(ILiqudationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "ILiquidationService cannot be null during initialization.");
            }
            _liquidationInstance = service;
        }

        // Renamed method for clarity
        public ILiqudationService GetLiquidationService()
        {
            if (_liquidationInstance == null)
            {
                throw new InvalidOperationException("ILiquidationService instance has not been initialized.");
            }
            return _liquidationInstance;
        }
    }
}
