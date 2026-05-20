using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using System;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for managing MT5 account margin calculations
    /// </summary>
    [RoutePrefix("api/margin")]
    public class MarginController : BaseApiController
    {
        public MarginController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get current margin information for an account
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetMarginInfo(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var account = _manager.UserCreate();
                var result = _manager.UserGet((ulong)loginId, account);

                if (result != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                // Get account details
                var user = _manager.UserCreate();
                _manager.UserGet((ulong)loginId, user);

                // Get positions to calculate margin
                var positionsArray = _manager.PositionCreateArray();
                _manager.PositionGet((ulong)loginId, positionsArray);
                double totalProfit = 0;
                uint totalPositions = positionsArray.Total();

                for (uint i = 0; i < totalPositions; i++)
                {
                    var position = positionsArray.Next(i);
                    if (position != null)
                    {
                        totalProfit += position.Profit();
                    }
                }

                double balance = account.Balance();
                double credit = account.Credit();
                double equity = balance + credit + totalProfit;

                positionsArray.Release();
                account.Release();
                user.Release();

                var marginInfo = new
                {
                    Login = loginId,
                    Balance = Math.Round(balance, 2),
                    Credit = Math.Round(credit, 2),
                    Equity = Math.Round(equity, 2),
                    Profit = Math.Round(totalProfit, 2),
                    OpenPositions = totalPositions
                };

                return new BaseResponse<object>().WithSuccess(marginInfo, "Margin information retrieved successfully");
            });
        }

        /// <summary>
        /// Get account summary with margin details
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}/summary")]
        public IHttpActionResult GetAccountSummary(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var account = _manager.UserCreate();
                var result = _manager.UserGet((ulong)loginId, account);

                if (result != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var positionsArray = _manager.PositionCreateArray();
                _manager.PositionGet((ulong)loginId, positionsArray);

                double totalProfit = 0;
                uint totalPositions = positionsArray.Total();

                for (uint i = 0; i < totalPositions; i++)
                {
                    var position = positionsArray.Next(i);
                    if (position != null)
                    {
                        totalProfit += position.Profit();
                    }
                }

                double balance = account.Balance();
                double credit = account.Credit();
                double equity = balance + credit + totalProfit;
                double freeMargin = equity; // Simplified calculation

                positionsArray.Release();
                account.Release();

                var summary = new
                {
                    Login = loginId,
                    Balance = Math.Round(balance, 2),
                    Credit = Math.Round(credit, 2),
                    Equity = Math.Round(equity, 2),
                    FreeMargin = Math.Round(freeMargin, 2),
                    UnrealizedProfit = Math.Round(totalProfit, 2),
                    OpenPositions = totalPositions
                };

                return new BaseResponse<object>().WithSuccess(summary, "Account summary retrieved successfully");
            });
        }
    }
}

