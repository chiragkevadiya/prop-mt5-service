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
    public class CreditInOutController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public CreditInOutController()
        {

        }

        [HttpPost]
        public MTRetCode CreadiInOutBalance([FromBody] MTFiveDepositBalanceVM entity)
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
