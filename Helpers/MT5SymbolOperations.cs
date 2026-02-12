using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.ViewModels.SymbolName;
using System.Collections.Generic;

namespace PropMT5ConnectionService.Helpers
{
    public static class MT5SymbolOperations
    {
        public static SymbolNameListVM GetAllSymbols(CIMTManagerAPI manager)
        {
            List<SymbolNameVM> symbolNameVMs = new List<SymbolNameVM>();

            uint totalSymbols = manager.SymbolTotal();

            for (uint i = 0; i < totalSymbols; i++)
            {
                CIMTConSymbol cIMTConSymbols = manager.SymbolCreate();
                MTRetCode mTRetCode = manager.SymbolNext(i, cIMTConSymbols);

                if (mTRetCode == MTRetCode.MT_RET_OK)
                {
                    SymbolNameVM symbolNameVM = new SymbolNameVM()
                    {
                        SymbolName = cIMTConSymbols.Symbol(),
                        TickSize = cIMTConSymbols.TickSize(),
                        TickValue = cIMTConSymbols.TickValue(),
                        VolumeLimit = cIMTConSymbols.VolumeLimit(),
                        Country = cIMTConSymbols.Country(),
                        CurrencyMargin = cIMTConSymbols.CurrencyMargin(),
                        Description = cIMTConSymbols.Description(),
                        Spread = cIMTConSymbols.Spread(),
                        StopsLevel = cIMTConSymbols.StopsLevel(),
                        SymbolPath = cIMTConSymbols.Path(),
                        SymbolPoint = cIMTConSymbols.Point(),
                        Category = cIMTConSymbols.Category(),
                        Source = cIMTConSymbols.Source(),
                    };
                    symbolNameVMs.Add(symbolNameVM);
                }
            }

            return new SymbolNameListVM
            {
                SymbolTotal = totalSymbols,
                SymbolList = symbolNameVMs,
            };
        }
    }
}
