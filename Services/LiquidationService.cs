using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using PropMT5ConnectionService.APIResponse;
using PropMT5ConnectionService.Data;
using PropMT5ConnectionService.Helper;
using PropMT5ConnectionService.Models;
using PropMT5ConnectionService.ViewModels.ChallengeSettlement;
using PropTradingMT5.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Services
{
    public class LiquidationService : ILiqudationService
    {
        private readonly PropTradingDBContext _dbContext;
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly CIMTManagerAPI _manager;

        public LiquidationService(
            PropTradingDBContext dbContext,
            IHttpClientService httpClientService,
            IConfiguration configuration, CIMTManagerAPI manager)
        {
            _dbContext = dbContext;
            _httpClientService = httpClientService;
            _configuration = configuration;
            _manager = manager;
        }

        public async Task<BaseResponseObject<object>> CheckAndLiquidateAccounts()
        {
            try
            {
                // 1. Get active phases
                var activePhases = await _dbContext.UserChallengePhase
                    .Where(x => x.ChallengePhaseStatus == ChallengePhaseStatus.Active).ToListAsync();

                if (!activePhases.Any())
                {
                    return new BaseResponseObject<object>
                    {
                        Success = true,
                        Message = "No active challenge phases found."
                    };
                }

                // 2. Bulk fetch MT5 details
                var loginIds = activePhases.Select(p => p.UserAccountId).Distinct().ToList();
                var accountsData = await GetAccountsDetailsBulk(loginIds);

                var errors = new List<string>();
                var successCount = 0;

                // 3. Process phases
                foreach (var phase in activePhases)
                {
                    // DB account validation
                    var account = await _dbContext.PropAccountMaster
                        .FirstOrDefaultAsync(a => a.TerminalID == phase.UserAccountId && a.IsActive && !a.IsDeleted);

                    if (account == null)
                    {
                        errors.Add($"Account not found for login {phase.UserAccountId}.");
                        continue;
                    }

                    // MT5 details
                    if (!accountsData.TryGetValue(phase.UserAccountId, out var accountDetails))
                    {
                        errors.Add($"MT5 details not found for login {phase.UserAccountId}.");
                        continue;
                    }

                    decimal accountSize = phase.InitialBalance;
                    decimal currentEquity = (decimal)accountDetails.Equity;

                    // Limits
                    decimal dailyLossLimit = currentEquity * (phase.DailyLossPercent / 100m);
                    decimal maxLossLimit = accountSize * (phase.OverallLossPercent / 100m);
                    decimal liquidationLimit = currentEquity - dailyLossLimit;

                    // Breach check
                    if (currentEquity <= liquidationLimit)
                    {
                        string postURL = $"UserAccount?loginId={phase.UserAccountId}";
                        ResponseHttpMessage responseContent =
                            await _httpClientService.PostAsync(_configuration["MT5ServiceApiUrl"], postURL);

                        if (responseContent.Success)
                        {
                            await CloseChallengeAsync(
                                phase,
                                currentEquity,
                                ChallengeStatus.Failed,
                                phase.ProfitSplitPercentage,
                                $"Challenge closed due to liquidation breach. Equity={currentEquity}, Limit={liquidationLimit}"
                            );
                            DisableUserAndTrading(loginIds);
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Failed to disable account {phase.UserAccountId} in MT5: {responseContent.Message}");
                        }
                    }
                }

                var message = errors.Any()
                    ? $"Processed {successCount} accounts, {errors.Count} errors: {string.Join("; ", errors)}"
                    : $"Successfully liquidated {successCount} accounts.";

                return new BaseResponseObject<object>
                {
                    Success = true,
                    Message = message,
                    Data = new { Processed = successCount, Errors = errors }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseObject<object>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
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
            var adminid = await GetAdminUserIdAsync();
            if (!adminid.HasValue)
                return new ChallengeSettlementResult { Success = false, Message = "Admin user not found." };

            var today = DateTime.UtcNow;
            try
            {
                var isSuccess = status == ChallengeStatus.Completed;
                // 1. Validate account
                var account = await _dbContext.PropAccountMaster
                    .FirstOrDefaultAsync(a => a.TerminalID == ch.UserAccountId && a.UserId == ch.UserId);
                if (account == null)
                    return new ChallengeSettlementResult { Success = false, Message = "Account not found." };

                // 2. Calculate profit and split
                var profit = isSuccess ? Math.Max(0m, currentEquity - ch.StartingEquity) : 0m;
                var splitPct = overrideSplitPercentage ?? (ch.ProfitSplitPercentage > 0 ? ch.ProfitSplitPercentage : 0m);
                var adminShare = isSuccess && profit > 0 ? Math.Round(profit * splitPct / 100m, 2) : 0m;
                var userShare = isSuccess && profit > 0 ? Math.Round(profit - adminShare, 2) : 0m;

                // 3. Lock trading and deactivate account
                ch.TradingLocked = true;
                ch.CompletedAt = today;
                ch.ChallengePhaseStatus = isSuccess ? ChallengePhaseStatus.Success : ChallengePhaseStatus.Drawdown;
                ch.ClosingEquity = currentEquity;

                // Store failure reason in comment
                ch.Comment = failureReason ?? "Phase Completed";
                var challengeHistory = await _dbContext.UserChallengeHistory.FirstOrDefaultAsync(h => h.Id == ch.UserChallengeHistoryId);
                // Also update UserChallengeHistory.Comment for overall challenge status
                if (!isSuccess)
                {
                    if (challengeHistory != null)
                    {
                        challengeHistory.Comment = failureReason ?? "Failed due to violate some condition";
                        challengeHistory.Status = ChallengeStatus.Failed;
                    }
                    var failureVariables = new
                    {
                        account.FullName,
                        AccountNo = account.TerminalID,
                        FailureReason = failureReason ?? "Failed due to violate some condition",
                    };

                    //await _emailService.SendEmailAsync(
                    //    EmailSubjectName.ChallengeClosed,
                    //    account.Email,
                    //    new[] { failureVariables });
                }

                await _dbContext.SaveChangesAsync();
                // await tx.CommitAsync();
                return new ChallengeSettlementResult
                {
                    Success = true,
                    Message = isSuccess ? "Challenge closed successfully." : "Challenge closed as failed.",
                    Profit = profit,
                    UserShare = userShare,
                    AdminShare = adminShare
                };
            }
            catch (Exception ex)
            {
                //await tx.RollbackAsync();
                return new ChallengeSettlementResult { Success = false, Message = ex.Message };
            }
        }


        public async Task<long?> GetAdminUserIdAsync()
        {
            // Find the first admin user (adjust RoleId/IsAdmin as per your schema)
            var adminUser = await _dbContext.UserMasters
                .Where(u => u.IsAdmin && u.RoleId == RoleType.Admin && u.IsActive && !u.IsDelete)
                .OrderBy(u => u.UserId)
                .FirstOrDefaultAsync();
            return adminUser?.UserId;
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
                    MTRetCode ret = _manager.UserGet(loginId, user);
                    if (ret != MTRetCode.MT_RET_OK)
                    {
                        results[loginId] = ret;
                        continue;
                    }

                    // Disable trading and login
                    var rights = user.Rights();
                    rights |= CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED;
                    rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_ENABLED;
                    user.Rights(rights);

                    ret = _manager.UserUpdate(user);
                    results[loginId] = ret;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disabling account {loginId}: {ex.Message}");
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



