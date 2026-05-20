using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropMT5Service.Services
{
    public class LiquidationService : ILiquidationService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly CIMTManagerAPI _manager;

        public LiquidationService(IHttpClientService httpClientService, CIMTManagerAPI manager)
        {
            _httpClientService = httpClientService;
            _manager = manager;
        }

        public async Task<Dictionary<long, AccountDetailsVM>> GetAccountsDetailsBulk(List<long> terminalIds)
        {
            var results = new Dictionary<long, AccountDetailsVM>();
            int batchSize = MT5Constants.AccountOperations.LiquidationBatchSize;
            CIMTAccountArray accountArray = _manager.UserCreateAccountArray();
            for (int i = 0; i < terminalIds.Count; i += batchSize)
            {
                var batch = terminalIds.Skip(i).Take(batchSize).ToArray();
                var accountArrayLogins = batch.Select(id => (ulong)id).ToArray();
                var retCode = _manager.UserAccountRequestByLogins(accountArrayLogins, accountArray);

                if (retCode == MTRetCode.MT_RET_ERROR)
                    continue;

                for (uint j = 0; j < accountArray.Total(); j++)
                {
                    CIMTAccount user = accountArray.Next(j); // MT5 way to get the account
                    if (user == null)
                        continue;

                    results[(long)user.Login()] = new AccountDetailsVM
                    {
                        Login = user.Login(),
                        Equity = user.Equity(),
                        Balance = user.Balance(),
                        MarginFree = user.Margin()
                    };
                }
            }

            return results;
        }
        public Dictionary<ulong, MTRetCode> DisableUserAndTrading(List<long> loginIds)
        {
            var results = new Dictionary<ulong, MTRetCode>();

            foreach (long loginIdLong in loginIds)
            {
                ulong loginId = (ulong)loginIdLong;
                CIMTUser user = _manager.UserCreate();

                if (user == null)
                {
                    results[loginId] = MTRetCode.MT_RET_ERROR;
                    continue;
                }

                try
                {
                    var ret = _manager.UserGet(loginId, user);
                    if (ret != MTRetCode.MT_RET_OK)
                    {
                        results[loginId] = ret;
                        continue;
                    }

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

                    CIMTPositionArray positions = _manager.PositionCreateArray();

                    try
                    {
                        ret = _manager.PositionRequest(loginId, positions);

                        if (ret == MTRetCode.MT_RET_OK)
                        {
                            for (uint i = 0; i < positions.Total(); i++)
                            {
                                CIMTPosition pos = positions.Next(i);
                                if (pos == null) continue;

                                var deleteRet = _manager.PositionDelete(pos);
                            }
                        }
                    }
                    finally
                    {
                        positions.Release();
                    }

                    results[loginId] = MTRetCode.MT_RET_OK;
                }
                catch (Exception ex)
                {
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



