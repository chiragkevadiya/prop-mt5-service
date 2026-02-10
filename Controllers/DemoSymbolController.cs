using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels.SymbolName;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoSymbolController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();

        [HttpGet]
        public SymbolNameListVM GetDemoSymbolName()
        {
            return MT5SymbolOperations.GetAllSymbols(_managerDemo);
        }
    }
}
