using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels.GroupName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5LiveLeverageUpdateController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

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
