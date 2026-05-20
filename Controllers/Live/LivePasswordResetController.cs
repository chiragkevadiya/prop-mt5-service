using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.Utilities;
using PropMT5Service.ViewModels.Password;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/livepasswordreset")]
    public class LivePasswordResetController : BaseApiController
    {
        public LivePasswordResetController(CIMTManagerAPI manager) : base(manager) { }

        [HttpPost]
        [Route("")]
        public IHttpActionResult ResetPassword(ulong loginId)
        {
            return ExecuteSafe(() =>
            {
                string masterPass = PasswordGenerator.GenerateMasterPassword(MT5Constants.PasswordConfig.DefaultMasterPasswordLength);
                string investorPass = PasswordGenerator.GenerateInvestorPassword(MT5Constants.PasswordConfig.DefaultInvestorPasswordLength);

                MTRetCode masterRet = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_MAIN, loginId, masterPass);
                MTRetCode investorRet = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_INVESTOR, loginId, investorPass);

                if (!IsSuccessful(masterRet))
                    return new BaseResponse<UserPasswordChangeVM>().WithError(
                        $"Master password reset failed: {GetMT5ErrorMessage(masterRet)}", 400);

                if (!IsSuccessful(investorRet))
                    return new BaseResponse<UserPasswordChangeVM>().WithError(
                        $"Investor password reset failed: {GetMT5ErrorMessage(investorRet)}", 400);

                return new BaseResponse<UserPasswordChangeVM>().WithSuccess(
                    new UserPasswordChangeVM
                    {
                        Login = loginId,
                        MasterPassword = masterPass,
                        InvestorPassword = investorPass
                    },
                    "Passwords reset successfully.");
            });
        }
    }
}
