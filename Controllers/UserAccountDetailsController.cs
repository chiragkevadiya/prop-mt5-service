using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class UserAccountDetailsController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public UserAccountDetailsController()
        {
            
        }

        [HttpPost]
        public List<AccountDetailsVM> GetAccountDetails([FromBody] List<ulong> LoginId)
        {
            try
            {
                List<AccountDetailsVM> liveAccounts = new List<AccountDetailsVM>();

                foreach (ulong loginId in LoginId)
                {
                    CIMTUser cIMTUserc = _manager.UserCreate();

                    MTRetCode mTRetCode1 = _manager.UserGet(loginId, cIMTUserc);

                    CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();

                    MTRetCode mTRetCode2 = _manager.UserAccountGet(loginId, cIMTAccountInfo);

                    if (MTRetCode.MT_RET_OK == mTRetCode1)
                    {
                        AccountDetailsVM liveAccountVM = new AccountDetailsVM()
                        {
                            Login = cIMTUserc.Login(),
                            Credit = cIMTUserc.Credit(),
                            Balance = cIMTUserc.Balance(),
                            MarginFree = cIMTAccountInfo.MarginFree(),
                            Equity = cIMTAccountInfo.Equity(),
                            Profit = cIMTAccountInfo.Profit(),
                        };

                        liveAccounts.Add(liveAccountVM);

                        cIMTUserc.Clear();
                        cIMTUserc.Release();
                    }
                }

                return liveAccounts;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
