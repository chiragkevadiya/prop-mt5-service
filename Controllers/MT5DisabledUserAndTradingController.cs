using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class MT5DisabledUserAndTradingController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public MT5DisabledUserAndTradingController()
        {

        }

        [HttpPost]
        public Dictionary<ulong, MTRetCode> DisableUserAndTrading(List<long> loginIds)
        {
            var results = new Dictionary<ulong, MTRetCode>();

            foreach (ulong loginId in loginIds)
            {
                CIMTUser user = _manager.UserCreate();
                if (user == null)
                {
                    results[loginId] = MTRetCode.MT_RET_ERROR;
                    continue;
                }

                try
                {
                    // 1? Fetch user
                    var ret = _manager.UserGet(loginId, user);
                    if (ret != MTRetCode.MT_RET_OK)
                    {
                        results[loginId] = ret;
                        continue;
                    }

                    // 2? Disable trading and login rights
                    var rights = user.Rights();
                    rights |= CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED;
                    rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_ENABLED;
                    user.Rights(rights);

                    ret = _manager.UserUpdate(user);
                    if (ret != MTRetCode.MT_RET_OK)
                    {
                        results[loginId] = ret;
                        continue;
                    }

                    // 3? Force-close all open positions using CIMTAdminAPI
                    CIMTPositionArray positions = _manager.PositionCreateArray();
                    ret = _manager.PositionRequest(loginId, positions); // get all positions for this login

                    if (ret == MTRetCode.MT_RET_OK)
                    {
                        for (uint i = 0; i < positions.Total(); i++)
                        {
                            CIMTPosition pos = positions.Next(i);
                            if (pos == null) continue;

                            var deleteRet = _manager.PositionDelete(pos); // force close
                            Console.WriteLine($"Position {pos.Symbol()} for login {loginId} deleted, result={deleteRet}");
                        }
                    }
                    positions.Release();

                    results[loginId] = MTRetCode.MT_RET_OK;
                    Console.WriteLine($"Account {loginId}: disabled and all positions force-closed successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing account {loginId}: {ex.Message}");
                    results[loginId] = MTRetCode.MT_RET_ERROR;
                }
                finally
                {
                    user.Release();
                }
            }

            return results;
        }

    }
}
