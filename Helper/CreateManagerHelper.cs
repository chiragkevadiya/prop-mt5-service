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
        private ILiquidationService _liquidationInstance;

        public CreateLiquidationHelper(ILiquidationService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service), "ILiquidationService cannot be null during initialization.");
            }
            _liquidationInstance = service;
        }

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
