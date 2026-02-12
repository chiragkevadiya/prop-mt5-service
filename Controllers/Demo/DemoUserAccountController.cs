using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for demo user account details and balance operations
    /// </summary>
    [RoutePrefix("api/demo/user-account")]
    public class DemoUserAccountController : BaseApiController
    {
        public DemoUserAccountController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get demo user account details by login ID
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetUserAccount(long loginId)
        {
            return ExecuteSafe(() =>
            {
                ulong login = (ulong)loginId;

                CIMTAccount account = _manager.UserCreateAccount();
                if (account == null)
                    throw new System.InvalidOperationException("Failed to create account object.");

                MTRetCode retCode = _manager.UserAccountGet(login, account);

                if (retCode != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return new BaseResponse<UserDetailsAccountVM>().WithError("Account not found", 404);
                }

                var userDetails = new UserDetailsAccountVM
                {
                    Balance = account.Balance(),
                    Credit = account.Credit(),
                    Equity = account.Equity(),
                    Margin = account.Margin(),
                    MarginFree = account.MarginFree(),
                    Profit = account.Profit()
                };

                account.Release();

                return new BaseResponse<UserDetailsAccountVM>().WithSuccess(userDetails, "Demo account details retrieved successfully");
            });
        }

        /// <summary>
        /// Deposit or withdraw balance for a demo account
        /// </summary>
        [HttpPost]
        [Route("deposit-balance")]
        public IHttpActionResult DepositBalance([FromBody] Mt5DepositBalanceVM entity)
        {
            if (entity == null)
                return BadRequest("Request body cannot be null");

            return ExecuteSafe(() =>
            {
                MTRetCode result = MT5AccountOperations.DepositOrWithdrawBalance(_manager, entity);

                if (result == MTRetCode.MT_RET_REQUEST_DONE)
                    return new BaseResponse<MTRetCode>().WithSuccess(result, "Balance operation completed successfully");

                return new BaseResponse<MTRetCode>().WithError($"Balance operation failed: {result}", 400);
            });
        }
    }
}
