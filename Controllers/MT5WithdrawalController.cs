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
    public class MT5WithdrawalController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpPost]
        public MTRetCode MT5Withdrawal([FromBody] MTFiveDepositBalanceVM entity)
        {
            try
            {
                MTRetCode mTRetCode;
                ulong variable = 0;

                CIMTUser cIMTUser = _manager.UserCreate();
                mTRetCode = _manager.UserGet(entity.Login, cIMTUser);

                if (mTRetCode == MTRetCode.MT_RET_OK)
                {
                    var balance = GetBalanceForLogin(entity.Login);

                    if (balance < 0)
                    {
                        return MTRetCode.MT_RET_REQUEST_NO_MONEY;
                    }
                    if (entity.Amount <= 0)
                    {
                        return MTRetCode.MT_RET_REQUEST_NO_MONEY;
                    }

                    if (balance == 0)
                    {
                        return MTRetCode.MT_RET_REQUEST_NO_MONEY;
                    }

                    if (balance < entity.Amount)
                    {
                        return MTRetCode.MT_RET_REQUEST_NO_MONEY;
                    }

                    mTRetCode = _manager.DealerBalance(entity.Login, -entity.Amount, 2, entity.Comment, out variable);

                    if (MTRetCode.MT_RET_REQUEST_DONE == mTRetCode)
                    {
                        return mTRetCode;
                    }
                    else
                    {
                        // If the value of this parameter is true, the free margin is checked before conducting the balance operation.
                        // If the amount withdraw is greater than the free margin value, error MT_RET_REQUEST_NO_MONEY is returned.
                        // If the parameter is set to false, the margin is not checked and the requested amount will be withdrawn even
                        // if it's greater then the free margin. If the parameter is not passed, the check is considered enabled.
                        return MTRetCode.MT_RET_REQUEST_NO_MONEY;
                    }
                }
                else
                {
                    // User Not Found (trading account)
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
            CIMTUser cIMTUserc = _manager.UserCreate();
            MTRetCode mTRetCode1 = _manager.UserGet(login, cIMTUserc);

            if (MTRetCode.MT_RET_OK == mTRetCode1)
            {
                return cIMTUserc.Balance();
            }

            return 0;
        }
    }
}
