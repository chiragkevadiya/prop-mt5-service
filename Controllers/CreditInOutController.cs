using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    [RoutePrefix("api/credit-in-out")] // Updated route prefix for consistency
    public class CreditInOutController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();
        public CreditInOutController()
        {

        }

        [HttpPost]
        [Route("balance")] // Explicit route for the action
        public MTRetCode CreditInOutBalance([FromBody] Mt5DepositBalanceVM entity)
        {
            try
            {
                ulong variable;
                MTRetCode result;

                switch (entity.Comment)
                {
                    case "CreditIn":
                        result = _manager.DealerBalanceRaw(entity.Login, entity.Amount, 3, entity.Comment, out variable);
                        return result == MTRetCode.MT_RET_REQUEST_DONE ? MTRetCode.MT_RET_REQUEST_DONE : MTRetCode.MT_RET_ERR_NOTFOUND;

                    case "CreditOut":
                        var user = _manager.UserCreate();
                        if (_manager.UserGet(entity.Login, user) != MTRetCode.MT_RET_OK)
                            return MTRetCode.MT_RET_ERR_NOTFOUND;

                        double balance = GetBalanceForLogin(entity.Login);
                        if (entity.Amount <= 0 || balance <= 0 || balance < entity.Amount)
                            return MTRetCode.MT_RET_REQUEST_NO_MONEY;

                        result = _manager.DealerBalance(entity.Login, -entity.Amount, 3, entity.Comment, out variable);
                        return result == MTRetCode.MT_RET_REQUEST_DONE ? MTRetCode.MT_RET_REQUEST_DONE : MTRetCode.MT_RET_REQUEST_NO_MONEY;

                    default:
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
                return cIMTUserc.Credit();
            }

            return 0;
        }
    }
}
