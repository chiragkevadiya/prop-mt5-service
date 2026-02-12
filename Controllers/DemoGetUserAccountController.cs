using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    [RoutePrefix("api/demo-user-accounts")] // Added route prefix for consistency
    public class DemoUserAccountsController : ApiController // Renamed class for consistency
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();

        [HttpPost]
        [Route("get-by-user-id")] // Added explicit route for the action
        public List<MtGetAllLiveAccountVM> GetDemoByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            return MT5AccountOperations.GetAccountsByLoginIds(_managerDemo, LoginId);
        }
    }
}

