using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static MetaQuotes.MT5CommonAPI.CIMTDeal;

namespace MT5ConnectionService.Controllers
{
    public class CloseTradeController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public CloseTradeController()
        {

        }

        [HttpPost]
        public BaseResponseModel<List<ClosedTradeResponse>> TradesOrdersClose([FromBody] ClosePositionRequest entity)
        {
            try
            {
                if (entity == null || entity.PositionId == null || entity.PositionId.Count == 0)
                    return new BaseResponseModel<List<ClosedTradeResponse>> { Success = false, Message = "Invalid input data." };

                if (_manager == null)
                    return new BaseResponseModel<List<ClosedTradeResponse>> { Success = false, Message = "Manager is not initialized." };

                CIMTPositionArray positions = _manager.PositionCreateArray();
                if (positions == null)
                    return new BaseResponseModel<List<ClosedTradeResponse>> { Success = false, Message = "Failed to create position array." };

                MTRetCode res = _manager.PositionGet(entity.LoginId, positions);
                if (res != MTRetCode.MT_RET_OK || positions.Total() == 0)
                    return new BaseResponseModel<List<ClosedTradeResponse>> { Success = false, Message = "No positions found for user." , MTRetErrorCode = res};

                var closedTrades = new List<ClosedTradeResponse>();
                for (uint i = 0; i < positions.Total(); i++)
                {
                    CIMTPosition position = positions.Next(i);
                    if (position == null || !entity.PositionId.Contains(position.Position()))
                        continue;

                    CIMTDeal deal = _manager.DealCreate();
                    if (deal == null)
                        continue;

                    deal.Login(position.Login());
                    deal.Symbol(position.Symbol());
                    deal.Action(position.Action() == (uint)CIMTPosition.EnPositionAction.POSITION_BUY
                        ? (uint)CIMTDeal.EnDealAction.DEAL_SELL
                        : (uint)CIMTDeal.EnDealAction.DEAL_BUY);
                    deal.Volume(position.Volume());
                    deal.Price(position.PriceCurrent());
                    deal.PositionID(position.Position());
                    deal.Entry((uint)EnEntryFlag.ENTRY_OUT);
                    deal.ReasonSet((uint)CIMTDeal.EnDealReason.DEAL_REASON_CLIENT); // or DEAL_REASON_MANAGER

                    // Add deal
                    MTRetCode dealResult = _manager.DealPerform(deal);


                    if (dealResult != MTRetCode.MT_RET_OK)
                    {
                        return new BaseResponseModel<List<ClosedTradeResponse>>
                        {
                            Success = false,
                            Message = $"Failed to perform deal for position {position.Position()}, code: {dealResult}"
                        };
                    }
                    else if(dealResult == MTRetCode.MT_RET_OK)
                    {
                        closedTrades.Add(new ClosedTradeResponse
                        {
                            DealId = deal.Deal(),
                            PositionId = position.Position(),
                            Symbol = deal.Symbol(),
                            Volume = deal.Volume(),
                            Price = deal.Price(),
                            PriceOpen = position.PriceOpen(),
                            Profit = deal.Profit(),
                            Commission = deal.Commission(),
                            Swap = deal.Storage(), // sometimes called Swap or Storage
                            Action = deal.Action(),
                            Entry = deal.Entry(),
                            Reason = deal.Reason(),
                            Time = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).UtcDateTime,
                            Login = deal.Login(),
                            Order = deal.Order()
                        });
                    }
                    deal.Release();
                }

                positions.Release();

                return new BaseResponseModel<List<ClosedTradeResponse>>
                {
                    Success = true,
                    Message = "Trades closed successfully.",
                    Data = closedTrades
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<List<ClosedTradeResponse>>
                {
                    Success = false,
                    Message = $"An exception occurred: {ex.Message}"
                };
            }
        }
    
    }
}
