using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class GetUserAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpPost]
        public List<MtGetAllLiveAccountVM> GetLiveByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            return MT5AccountOperations.GetAccountsByLoginIds(_manager, LoginId);
        }
    }
}
