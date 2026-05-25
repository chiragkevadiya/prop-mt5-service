using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/mt5/account")]
    public class LiveAccountDisableController : BaseApiController
    {
        public LiveAccountDisableController(CIMTManagerAPI manager) : base(manager) { }

        [HttpPost]
        [Route("disable")]
        public IHttpActionResult DisableUserAndTrading([FromBody] List<long> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var results = new List<AccountDisableResult>();

                foreach (long id in loginIds)
                {
                    ulong loginId = (ulong)id;
                    var result = new AccountDisableResult { LoginId = loginId };

                    CIMTUser user = _manager.UserCreate();
                    if (user == null)
                    {
                        result.Status = "Failed";
                        result.Message = "Unable to create user object";
                        results.Add(result);
                        continue;
                    }

                    try
                    {
                        var ret = _manager.UserGet(loginId, user);
                        if (ret != MTRetCode.MT_RET_OK)
                        {
                            result.Status = "Failed";
                            result.Message = $"User not found ({ret})";
                            results.Add(result);
                            continue;
                        }

                        var rights = user.Rights();
                        rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_ENABLED;
                        rights |= CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED;
                        user.Rights(rights);

                        ret = _manager.UserUpdate(user);
                        if (ret != MTRetCode.MT_RET_OK)
                        {
                            result.Status = "Failed";
                            result.Message = $"Update rights failed ({ret})";
                            results.Add(result);
                            continue;
                        }

                        // Get live account balance before closing positions
                        CIMTAccount account = _manager.UserCreateAccount();
                        if (_manager.UserAccountGet(loginId, account) == MTRetCode.MT_RET_OK)
                            result.Balance = account.Balance();
                        account.Release();

                        // Close all open positions via DealPerform and collect PnL
                        CIMTPositionArray positions = _manager.PositionCreateArray();
                        ret = _manager.PositionRequest(loginId, positions);

                        if (ret == MTRetCode.MT_RET_OK)
                        {
                            for (uint i = 0; i < positions.Total(); i++)
                            {
                                CIMTPosition pos = positions.Next(i);
                                if (pos == null) continue;

                                CIMTDeal closeDeal = _manager.DealCreate();
                                if (closeDeal == null) continue;

                                try
                                {
                                    closeDeal.Login(pos.Login());
                                    closeDeal.Symbol(pos.Symbol());
                                    closeDeal.Action(pos.Action() == (uint)CIMTPosition.EnPositionAction.POSITION_BUY
                                        ? (uint)CIMTDeal.EnDealAction.DEAL_SELL
                                        : (uint)CIMTDeal.EnDealAction.DEAL_BUY);
                                    closeDeal.VolumeExt(pos.VolumeExt());
                                    closeDeal.Price(pos.PriceCurrent());
                                    closeDeal.PositionID(pos.Position());
                                    closeDeal.Entry((uint)CIMTDeal.EnEntryFlag.ENTRY_OUT);
                                    closeDeal.ReasonSet((uint)CIMTDeal.EnDealReason.DEAL_REASON_CLIENT);

                                    MTRetCode dealRet = _manager.DealPerform(closeDeal);
                                    if (dealRet == MTRetCode.MT_RET_OK)
                                    {
                                        result.ClosedPositions++;
                                        result.TotalProfit += closeDeal.Profit();
                                        result.TotalSwap += closeDeal.Storage();
                                        result.TotalCommission += closeDeal.Commission();
                                    }
                                }
                                finally
                                {
                                    closeDeal.Release();
                                }
                            }
                        }
                        positions.Release();

                        result.TotalPnL = result.TotalProfit + result.TotalSwap + result.TotalCommission;
                        result.ProfitPercentage = Math.Round((result.TotalPnL / 100), 2);
                        result.Status = "Success";
                        result.Message = "Disabled and positions closed";
                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        result.Status = "Failed";
                        result.Message = ex.Message;
                        results.Add(result);
                    }
                    finally
                    {
                        user.Release();
                    }
                }

                return new BaseResponse<List<AccountDisableResult>>().WithSuccess(results, "Disable process completed.");
            });
        }
    }
}
