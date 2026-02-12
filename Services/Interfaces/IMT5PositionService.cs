using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services.Interfaces
{
    public interface IMT5PositionService
    {
        Task<BaseResponseModel<List<CIMTPosition>>> GetOpenPositionsAsync(ulong loginId, bool isDemo = false);

        Task<BaseResponseModel<bool>> ClosePositionAsync(ClosePositionRequest request, bool isDemo = false);

        Task<BaseResponseModel<List<MT5TradingDataVM>>> GetOpenTradesAsync(ulong loginId, bool isDemo = false);

        Task<BaseResponseModel<bool>> HasOpenPositionsAsync(ulong loginId, bool isDemo = false);
    }
}
