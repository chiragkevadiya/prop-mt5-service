using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    [RoutePrefix("api/accountperformance")]
    public class AccountPerformanceDataController : BaseApiController
    {
        public AccountPerformanceDataController(CIMTManagerAPI manager) : base(manager)
        {
        }

        [HttpGet]
        [Route("performance")]
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
