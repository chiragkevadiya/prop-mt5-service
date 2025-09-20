using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels.SymbolName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoSymbolController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();
        public DemoSymbolController()
        {

        }

        [HttpGet]
        public SymbolNameListVM GetDemoSymbolName()
        {
            try
            {
                List<SymbolNameVM> symbolNameVMs = new List<SymbolNameVM>();

                uint totalSymbols = _managerDemo.SymbolTotal();


                for (uint i = 0; i < totalSymbols; i++)
                {
                    CIMTConSymbol cIMTConSymbols = _managerDemo.SymbolCreate();
                    MTRetCode mTRetCode = _managerDemo.SymbolNext(i, cIMTConSymbols);

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

                SymbolNameListVM symbolListVM = new SymbolNameListVM
                {
                    SymbolTotal = totalSymbols,
                    SymbolList = symbolNameVMs,
                };

                return symbolListVM;

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
