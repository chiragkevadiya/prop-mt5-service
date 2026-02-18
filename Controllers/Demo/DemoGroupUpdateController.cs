using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class DemoGroupUpdateController : ApiController
    {
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();

        [HttpGet]
        [Route("")]
        public BaseResponseModel<int> DemoMT5LiveGroupNameChanges(ulong LoginId, string GroupName)
        {
            try
            {
                //Update Group Name

                CIMTUser cIMTUser = _managerDemo.UserCreate();
                MTRetCode mTRetCode = _managerDemo.UserGet(LoginId, cIMTUser);

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

                cIMTUser.Group(GroupName);
                MTRetCode mTRetCode1 = _managerDemo.UserUpdate(cIMTUser);


                return new BaseResponseModel<int>
                {
                    Data = 0,
                    Message = "Group name update successfully.",
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
