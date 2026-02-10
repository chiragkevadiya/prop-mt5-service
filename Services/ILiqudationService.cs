using MetaQuotes.MT5CommonAPI;
using MT5ConnectionService.ViewModels;
using PropMT5ConnectionService.Helper;
using PropMT5ConnectionService.Models;
using PropMT5ConnectionService.ViewModels.ChallengeSettlement;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Services
{
    public interface ILiqudationService
    {
        Task<BaseResponseObject<object>> CheckAndLiquidateAccounts();
        Task<Dictionary<long, AccountDetailsVM>> GetAccountsDetailsBulk(List<long> terminalIds);
        Task<ChallengeSettlementResult> CloseChallengeAsync(UserChallengePhase ch, decimal currentEquity, ChallengeStatus status, decimal? overrideSplitPercentage = null, string failureReason = null);
        Task<long?> GetAdminUserIdAsync();
        Dictionary<ulong, MTRetCode> DisableUserAndTrading(List<long> loginIds);
    }
}
