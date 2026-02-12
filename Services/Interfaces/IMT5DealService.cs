using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services.Interfaces
{
    public interface IMT5DealService
    {
        Task<BaseResponseModel<List<CIMTDeal>>> GetDealHistoryAsync(
            ulong loginId, DateTime from, DateTime to, bool isDemo = false);

        Task<BaseResponseModel<List<DealMasterVM>>> GetDealHistoryByGroupAsync(
            string groupName, DateTime from, DateTime to, bool isDemo = false);

        Task<BaseResponseModel<List<MT5TradingHistoryVM>>> GetTradingHistoryAsync(
            ulong loginId, DateTime from, DateTime to, bool isDemo = false);

        Task<BaseResponseModel<List<ProfitBySymbolDaywiseVM>>> GetProfitBySymbolAsync(
            ulong loginId, DateTime from, DateTime to, bool isDemo = false);
    }
}
