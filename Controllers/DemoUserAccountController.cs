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
    public class DemoUserAccountController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();
        public DemoUserAccountController()
        {

        }

        [HttpGet]
        public UserDetailsAccountVM UsersAccountGet(ulong LoginId)
        {
            try
            {
                //List<UserDetailsAccountVM> userDetailsAccountVMs = new List<UserDetailsAccountVM>();

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
                else
                {
                    return null;
                }
            }

            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public MTRetCode UserDepositBalance([FromBody] MTFiveDepositBalanceVM entity)
        {
            try
            {
                // (DEAL_CREDIT 3) =>  Use This Id 3 Credit operation.
                // (DEAL_BALANCE 2) =>  Use This Id 2 A balance operation.

                MTRetCode mTRetCode;
                ulong variable = 0;

                if (entity.Comment == "Withdraw")
                {
                    var balance = GetBalanceForLogin(entity.Login);

                    if (balance < 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }
                    if (entity.Amount <= 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    if (balance == 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    if (balance < entity.Amount)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    mTRetCode = _managerDemo.DealerBalanceRaw(entity.Login, -entity.Amount, 2, entity.Comment, out variable);
                }
                else
                {
                    mTRetCode = _managerDemo.DealerBalanceRaw(entity.Login, entity.Amount, 2, entity.Comment, out variable);
                }

                if (MTRetCode.MT_RET_REQUEST_DONE == mTRetCode)
                {
                    return mTRetCode;
                }
                else
                {
                    return MTRetCode.MT_RET_ERR_NOTFOUND;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private double GetBalanceForLogin(ulong login)
        {
            CIMTUser cIMTUserc = _managerDemo.UserCreate();
            MTRetCode mTRetCode1 = _managerDemo.UserGet(login, cIMTUserc);

            if (MTRetCode.MT_RET_OK == mTRetCode1)
            {
                return cIMTUserc.Balance();
            }

            return 0;
        }

    }
}
