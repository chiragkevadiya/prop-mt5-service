using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels.SymbolName;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class SymbolController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public SymbolNameListVM GetSymbolName()
        {
            return MT5SymbolOperations.GetAllSymbols(_manager);
        }
    }
}
