using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class DemoUserAccountBatchController : ApiController
    {
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();

        [HttpPost]
        public List<Mt5LiveAccountVM> GetDemoByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            return MT5AccountOperations.GetAccountsByLoginIds(_managerDemo, LoginId);
        }
    }
}
