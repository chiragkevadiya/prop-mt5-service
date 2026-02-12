using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for disabling live accounts and force-closing positions
    /// </summary>
    [RoutePrefix("api/mt5/account")]
    public class LiveAccountDisableController : BaseApiController
    {
        public LiveAccountDisableController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Disable user accounts and force-close all open positions
        /// </summary>
        [HttpPost]
        [Route("disable")]
        public IHttpActionResult DisableUserAndTrading([FromBody] List<long> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var results = new Dictionary<ulong, string>();

                foreach (long id in loginIds)
                {
                    ulong loginId = (ulong)id;
                    CIMTUser user = _manager.UserCreate();
                    if (user == null)
                    {
                        results[loginId] = "Failed: Unable to create user object";
                        continue;
                    }

                    try
                    {
                        var ret = _manager.UserGet(loginId, user);
                        if (ret != MTRetCode.MT_RET_OK)
                        {
                            results[loginId] = $"Failed: User not found ({ret})";
                            continue;
                        }

                        // Disable trading and login rights
                        var rights = user.Rights();
                        rights |= CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED;
                        rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_ENABLED;
                        user.Rights(rights);

                        ret = _manager.UserUpdate(user);
                        if (ret != MTRetCode.MT_RET_OK)
                        {
                            results[loginId] = $"Failed: Update rights failed ({ret})";
                            continue;
                        }

                        // Force-close all open positions
                        CIMTPositionArray positions = _manager.PositionCreateArray();
                        ret = _manager.PositionRequest(loginId, positions);

                        if (ret == MTRetCode.MT_RET_OK)
                        {
                            for (uint i = 0; i < positions.Total(); i++)
                            {
                                CIMTPosition pos = positions.Next(i);
                                if (pos == null) continue;
                                _manager.PositionDelete(pos);
                            }
                        }
                        positions.Release();

                        results[loginId] = "Success: Disabled and positions force-closed";
                    }
                    catch (Exception ex)
                    {
                        results[loginId] = $"Error: {ex.Message}";
                    }
                    finally
                    {
                        user.Release();
                    }
                }

                return new BaseResponse<Dictionary<ulong, string>>().WithSuccess(results, "Disable process completed.");
            });
        }
    }
}
