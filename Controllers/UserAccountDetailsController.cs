using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for retrieving account details for multiple users
    /// </summary>
    [RoutePrefix("api/mt5/account-details")]
    public class UserAccountDetailsController : BaseApiController
    {
        public UserAccountDetailsController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get account details for multiple login IDs
        /// </summary>
        [HttpPost]
        [Route("batch")]
        public IHttpActionResult GetAccountDetails([FromBody] List<ulong> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var accounts = new List<AccountDetailsVM>();

                foreach (ulong loginId in loginIds)
                {
                    CIMTUser user = _manager.UserCreate();
                    MTRetCode userCode = _manager.UserGet(loginId, user);

                    CIMTAccount accountInfo = _manager.UserCreateAccount();
                    MTRetCode accountCode = _manager.UserAccountGet(loginId, accountInfo);

                    if (userCode == MTRetCode.MT_RET_OK)
                    {
                        accounts.Add(new AccountDetailsVM
                        {
                            Login = user.Login(),
                            Credit = user.Credit(),
                            Balance = user.Balance(),
                            MarginFree = accountInfo.MarginFree(),
                            Equity = accountInfo.Equity(),
                            Profit = accountInfo.Profit()
                        });
                    }

                    user.Clear();
                    user.Release();
                    accountInfo.Release();
                }

                return new BaseResponse<List<AccountDetailsVM>>().WithSuccess(accounts, "Account details retrieved successfully");
            });
        }
    }
}
