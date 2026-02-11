using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropPropMT5ConnectionService.ViewModels.SymbolName;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class SymbolController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpGet]
        public SymbolNameListVM GetSymbolName()
        {
            return MT5SymbolOperations.GetAllSymbols(_manager);
        }
    }
}
