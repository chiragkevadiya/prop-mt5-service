using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Configuration;
using PropMT5ConnectionService.ViewModels;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Models;
using PropMT5ConnectionService.ViewModels.ChallengeSettlement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helpers.Constant;

namespace PropMT5ConnectionService.Services
{
    public class LiquidationService : ILiquidationService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly CIMTManagerAPI _manager;

        public LiquidationService(
            IHttpClientService httpClientService,
            IConfiguration configuration,
            CIMTManagerAPI manager)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            _manager = manager;
        }

        public async Task<BaseResponseObject<object>> CheckAndLiquidateAccounts()
        {
            // Database context has been removed from this service.
            // This method is intentionally disabled. Callers should supply active phases and account details
            // to a different implementation that performs DB operations.
            await Task.CompletedTask;
            return new BaseResponseObject<object>
            {
                Success = false,
                Message = "CheckAndLiquidateAccounts is disabled: DB context removed from LiquidationService."
            };
        }
        public async Task<Dictionary<long, AccountDetailsVM>> GetAccountsDetailsBulk(List<long> terminalIds)
        {
            var results = new Dictionary<long, AccountDetailsVM>();
            int batchSize = 100;
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

        public async Task<ChallengeSettlementResult> CloseChallengeAsync(UserChallengePhase ch, decimal currentEquity, ChallengeStatus status, decimal? overrideSplitPercentage = null, string failureReason = null)
        {
            // DB removed from this service. This operation cannot proceed here.
            await Task.CompletedTask;
            return new ChallengeSettlementResult { Success = false, Message = "CloseChallengeAsync is disabled: DB context removed from LiquidationService." };
        }


        public async Task<long?> GetAdminUserIdAsync()
        {
            // DB removed from this service. Returning no admin.
            await Task.CompletedTask;
            return null;
        }

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
                    // 1️ Fetch user
                    var ret = _manager.UserGet(loginId, user);
                    if (ret != MTRetCode.MT_RET_OK)
                    {
                        results[loginId] = ret;
                        continue;
                    }

                    // 2️ Disable trading and login rights
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

                    // 3️ Force-close all open positions using CIMTAdminAPI
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



