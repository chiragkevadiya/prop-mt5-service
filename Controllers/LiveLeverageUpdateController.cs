using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class LiveLeverageUpdateController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpGet]
        public BaseResponseModel<int> MT5LiveLeverageUpdate(ulong LoginId, uint Leverage)
        {
            try
            {
                //Update Leverage

                CIMTUser cIMTUser = _manager.UserCreate();
                MTRetCode mTRetCode = _manager.UserGet(LoginId, cIMTUser);

                if (mTRetCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                {
                    return new BaseResponseModel<int>
                    {
                        Data = 0,
                        Message = "Login ID could not be found or does not exist within the system.",
                        Success = false,
                        MTRetErrorCode = mTRetCode
                    };
                }

                cIMTUser.Leverage(Leverage);
                MTRetCode mTRetCode1 = _manager.UserUpdate(cIMTUser);


                return new BaseResponseModel<int>
                {
                    Data = 0,
                    Message = "Leverage update successfully.",
                    Success = true,
                    MTRetErrorCode = MTRetCode.MT_RET_OK
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
