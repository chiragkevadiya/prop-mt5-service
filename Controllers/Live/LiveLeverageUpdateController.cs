using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/liveleverageupdate")]
    public class LiveLeverageUpdateController : BaseApiController
    {
        public LiveLeverageUpdateController(CIMTManagerAPI manager) : base(manager) { }

        [HttpGet]
        [Route("")]
        public IHttpActionResult MT5LiveLeverageUpdate(ulong loginId, uint leverage)
        {
            return ExecuteSafe(() =>
            {
                return WithUser(loginId, (user, ret) =>
                {
                    if (ret == MTRetCode.MT_RET_ERR_NOTFOUND)
                        return new BaseResponse<int>().WithError(
                            "Login ID could not be found or does not exist within the system.", 404);

                    if (!IsSuccessful(ret))
                        return new BaseResponse<int>().WithError(GetMT5ErrorMessage(ret), 400);

                    user.Leverage(leverage);
                    MTRetCode updateRet = _manager.UserUpdate(user);

                    if (!IsSuccessful(updateRet))
                        return new BaseResponse<int>().WithError(GetMT5ErrorMessage(updateRet), 400);

                    return new BaseResponse<int>().WithSuccess(0, "Leverage updated successfully.");
                });
            });
        }
    }
}
