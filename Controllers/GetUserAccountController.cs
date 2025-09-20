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
    public class GetUserAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public GetUserAccountController()
        {

        }

        // Get by Terminal Id Login array

        [HttpPost]
        public List<MtGetAllLiveAccountVM> GetLiveByUserIdAccounts([FromBody] List<ulong> LoginId)
        {
            List<MtGetAllLiveAccountVM> liveAccounts = new List<MtGetAllLiveAccountVM>();

            try
            {
                foreach (ulong loginId in LoginId)
                {
                    CIMTUser cIMTUserc = _manager.UserCreate();

                    MTRetCode mTRetCode1 = _manager.UserGet(loginId, cIMTUserc);

                    CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                    MTRetCode mTRetCode2 = _manager.UserAccountGet(loginId, cIMTAccountInfo);

                    if (MTRetCode.MT_RET_OK == mTRetCode1)
                    {
                        MtGetAllLiveAccountVM liveAccountVM = new MtGetAllLiveAccountVM()
                        {
                            Login = cIMTUserc.Login(),
                            FirstName = cIMTUserc.FirstName(),
                            LastName = cIMTUserc.LastName(),
                            Group = cIMTUserc.Group(),
                            Country = cIMTUserc.Country(),
                            Credit = cIMTUserc.Credit(),
                            Balance = cIMTUserc.Balance(),
                            Leverage = cIMTUserc.Leverage(),
                            Status = cIMTUserc.Status(),
                            Margin = cIMTAccountInfo.Margin(),
                            MarginFree = cIMTAccountInfo.MarginFree(),
                            Profit = cIMTAccountInfo.Profit(),
                            Equity = cIMTAccountInfo.Equity()
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
                // Handle exceptions appropriately
                throw;
            }
        }


    }
}
