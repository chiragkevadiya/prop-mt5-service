using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoGetUserAccountController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();

        [HttpPost]
        public List<MtGetAllLiveAccountVM> GetDemoByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            return MT5AccountOperations.GetAccountsByLoginIds(_managerDemo, LoginId);
        }
    }
}
