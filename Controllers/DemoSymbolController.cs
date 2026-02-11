using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels.SymbolName;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class DemoSymbolController : ApiController
    {
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();

        [HttpGet]
        public SymbolNameListVM GetDemoSymbolName()
        {
            return MT5SymbolOperations.GetAllSymbols(_managerDemo);
        }
    }
}
