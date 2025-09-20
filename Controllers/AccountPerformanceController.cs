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
    public class AccountPerformanceController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public AccountPerformanceController()
        {

        }

        [HttpGet]
        public BaseResponseModel<List<AccountPerformanceVM>> AccountPerformance()
        {
            try
            {

                List<AccountPerformanceVM> accountPerformanceVMs = new List<AccountPerformanceVM>();

                CIMTUserArray cIMTUserArray = _manager.UserCreateArray();
                MTRetCode result = _manager.UserGetByGroup("*", cIMTUserArray);

                // Check result
                if (result != MTRetCode.MT_RET_OK)
                {
                    return new BaseResponseModel<List<AccountPerformanceVM>>
                    {
                        Success = false,
                        Message = $"User Not Found"
                    };
                }


                for (uint i = 0; i < cIMTUserArray.Total(); i++)
                {
                    CIMTUser cIMTUser1 = cIMTUserArray.Next(i);

                    CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                    MTRetCode mTRetCode1 = _manager.UserAccountGet(cIMTUser1.Login(), cIMTAccountInfo);

                    accountPerformanceVMs.Add(new AccountPerformanceVM
                    {
                        Login = cIMTUser1.Login(),
                        Balance = cIMTUser1.Balance(),
                        Credit = cIMTUser1.Credit(),
                        Equity = cIMTAccountInfo.Equity(),
                        Profit = cIMTAccountInfo.Profit()
                    });
                    cIMTAccountInfo.Release();
                }

                cIMTUserArray.Release();

                return new BaseResponseModel<List<AccountPerformanceVM>>
                {
                    Success = true,
                    Message = $"Data retrieved successfully.",
                    Data = accountPerformanceVMs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<List<AccountPerformanceVM>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
