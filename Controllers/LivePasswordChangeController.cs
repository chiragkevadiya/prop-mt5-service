using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Controllers;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Services;
using PropMT5ConnectionService.ViewModels.Password;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 password changes
    /// </summary>
    [RoutePrefix("api/password")]
    public class LivePasswordChangeController : BaseApiController
    {
        private readonly IMT5AccountService _accountService;

        public LivePasswordChangeController(CIMTManagerAPI manager) : base(manager)
        {
            _accountService = new MT5AccountService(manager);
        }

        /// <summary>
        /// Change MT5 master or investor password
        /// </summary>
        /// <param name="loginId">User login ID</param>
        /// <param name="newPassword">New password</param>
        /// <param name="passwordType">0 = Master Password, 1 = Investor Password</param>
        [HttpPost]
        [Route("change")]
        public IHttpActionResult ChangePassword(ulong loginId, string newPassword, int passwordType)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return BadRequest("Password cannot be empty");

            if (passwordType != 0 && passwordType != 1)
                return BadRequest("Password type must be 0 (Master) or 1 (Investor)");

            return ExecuteSafe(() =>
            {
                var mt5PasswordType = passwordType == 0
                    ? CIMTUser.EnUsersPasswords.USER_PASS_MAIN
                    : CIMTUser.EnUsersPasswords.USER_PASS_INVESTOR;

                var result = _accountService.ChangePassword(loginId, newPassword, mt5PasswordType);
                
                return new BaseResponse<UserMasterInvestorPasswordVM>
                {
                    Success = result.Success,
                    Message = result.Message,
                    StatusCode = result.StatusCode,
                    Data = result.Success ? new UserMasterInvestorPasswordVM
                    {
                        Login = loginId,
                        MasterPassword = passwordType == 0 ? newPassword : null,
                        InvestorPassword = passwordType == 1 ? newPassword : null
                    } : null
                };
            });
        }

        /// <summary>
        /// Legacy endpoint for backward compatibility
        /// </summary>
        [HttpGet]
        [Obsolete("Use POST /api/password/change instead")]
        public IHttpActionResult MT5LiveMasterInvestorPasswordChange(ulong loginId, string newPassword, int pType)
        {
            return ChangePassword(loginId, newPassword, pType);
        }
    }
}
