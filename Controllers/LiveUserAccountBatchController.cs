using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class LiveUserAccountBatchController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpPost]
        public List<Mt5LiveAccountVM> GetLiveByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            return MT5AccountOperations.GetAccountsByLoginIds(_manager, LoginId);
        }
    }
}
