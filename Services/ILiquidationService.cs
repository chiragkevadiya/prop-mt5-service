using MetaQuotes.MT5CommonAPI;
using PropMT5ConnectionService.ViewModels;
using PropPropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Models;
using PropPropMT5ConnectionService.ViewModels.ChallengeSettlement;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PropPropMT5ConnectionService.Helpers.Constant;

namespace PropMT5ConnectionService.Services
{
    public interface ILiquidationService
    {
        Task<BaseResponseObject<object>> CheckAndLiquidateAccounts();
        Task<Dictionary<long, AccountDetailsVM>> GetAccountsDetailsBulk(List<long> terminalIds);
        Task<ChallengeSettlementResult> CloseChallengeAsync(UserChallengePhase ch, decimal currentEquity, ChallengeStatus status, decimal? overrideSplitPercentage = null, string failureReason = null);
        Task<long?> GetAdminUserIdAsync();
        Dictionary<ulong, MTRetCode> DisableUserAndTrading(List<long> loginIds);
    }
}
