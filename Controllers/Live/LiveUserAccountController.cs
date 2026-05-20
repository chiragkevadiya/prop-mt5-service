using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System;
using System.Linq;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for live user account details and balance operations
    /// </summary>
    [RoutePrefix("api/mt5/user-account")]
    public class LiveUserAccountController : BaseApiController
    {
        public LiveUserAccountController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get live user account details including closed profit
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetUserAccount(long loginId)
        {
            return ExecuteSafe(() =>
            {
                ulong login = (ulong)loginId;
                ulong[] loginIds = { login };

                CIMTUser user = _manager.UserCreate();
                if (user == null)
                    throw new InvalidOperationException("Failed to create user object.");

                MTRetCode userCode = _manager.UserGet(login, user);
                if (userCode != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    throw new InvalidOperationException($"Failed to get user info: {userCode}");
                }

                DateTimeOffset endDate = DateTimeOffset.UtcNow.AddDays(1);
                long fromTimestamp = 0;
                long toTimestamp = endDate.ToUnixTimeSeconds();

                CIMTDealArray dealArray = _manager.DealCreateArray();
                if (dealArray == null)
                    throw new InvalidOperationException("Failed to create deal array.");

                MTRetCode dealRequestCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);
                if (dealRequestCode != MTRetCode.MT_RET_OK)
                    throw new InvalidOperationException($"Deal request failed: {dealRequestCode}");

                var closedDeals = dealArray.ToArray()
                    .Where(deal => deal.Entry() == 1)
                    .ToList();

                double closedProfit = Math.Round(closedDeals.Sum(deal => deal.Profit()), 2);
                dealArray.Release();

                CIMTAccount account = _manager.UserCreateAccount();
                if (account == null)
                    throw new InvalidOperationException("Failed to create account object.");

                MTRetCode accountCode = _manager.UserAccountGet(login, account);
                if (accountCode != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return new BaseResponse<UserDetailsAccountVM>().WithError("Account not found", 404);
                }

                double totalProfit = account.Profit() + closedProfit;

                var userDetails = new UserDetailsAccountVM
                {
                    Balance = account.Balance(),
                    Credit = account.Credit(),
                    Equity = account.Equity(),
                    Margin = account.Margin(),
                    MarginFree = account.MarginFree(),
                    Profit = Math.Round(totalProfit, 2),
                    TotalCloseProfit = closedProfit
                };

                account.Release();
                user.Release();

                return new BaseResponse<UserDetailsAccountVM>().WithSuccess(userDetails, "Account details retrieved successfully");
            });
        }

        /// <summary>
        /// Deposit or withdraw balance for a live account
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
