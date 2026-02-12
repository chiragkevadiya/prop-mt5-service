using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class DemoUserAccountController : ApiController
    {
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();

        [HttpGet]
        public UserDetailsAccountVM UsersAccountGet(ulong LoginId)
        {
            try
            {
                CIMTAccount cIMT = _managerDemo.UserCreateAccount();
                MTRetCode mTRetCode = _managerDemo.UserAccountGet(LoginId, cIMT);

                if (mTRetCode == MTRetCode.MT_RET_OK)
                {
                    UserDetailsAccountVM userDetailsAccountVM = new UserDetailsAccountVM()
                    {
                        Balance = cIMT.Balance(),
                        Credit = cIMT.Credit(),
                        Equity = cIMT.Equity(),
                        Margin = cIMT.Margin(),
                        MarginFree = cIMT.MarginFree(),
                        Profit = cIMT.Profit(),
                    };
                    cIMT.Release();
                    return userDetailsAccountVM;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public MTRetCode UserDepositBalance([FromBody] Mt5DepositBalanceVM entity)
        {
            return MT5AccountOperations.DepositOrWithdrawBalance(_managerDemo, entity);
        }
    }
}
