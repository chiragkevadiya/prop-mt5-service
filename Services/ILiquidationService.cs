using MetaQuotes.MT5CommonAPI;
using PropMT5Service.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropMT5Service.Services
{
    public interface ILiquidationService
    {
        Task<Dictionary<long, AccountDetailsVM>> GetAccountsDetailsBulk(List<long> terminalIds);
        Dictionary<ulong, MTRetCode> DisableUserAndTrading(List<long> loginIds);
    }
}
