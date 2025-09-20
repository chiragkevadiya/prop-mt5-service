using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using MT5ConnectionService.ViewModels.TradePosition;
using Nancy.Extensions;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class OpenTradeDetailController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public BaseResponseModel<List<TradeOpenClosedVM>> OpenTradeDetail(string loginId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginId))
                {
                    return new BaseResponseModel<List<TradeOpenClosedVM>>
                    {
                        Success = false,
                        Message = "No login IDs provided."
                    };
                }

                // Split and parse the loginIds from string to ulong[]
                var loginIdArray = loginId
                    .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => ulong.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                    .Where(id => id > 0)
                    .ToArray();

                if (loginIdArray.Length == 0)
                {
                    return new BaseResponseModel<List<TradeOpenClosedVM>>
                    {
                        Success = false,
                        Message = "Invalid or empty login ID values."
                    };
                }

                List<TradeOpenClosedVM> tradeOpenClosedVM = new List<TradeOpenClosedVM>();

                CIMTPositionArray positionsArray = _manager.PositionCreateArray();
                MTRetCode mTRetCode = _manager.PositionGetByLogins(loginIdArray, positionsArray);

                if (mTRetCode != MTRetCode.MT_RET_OK)
                {
                    return new BaseResponseModel<List<TradeOpenClosedVM>>
                    {
                        Success = false,
                        Message = "Data Not Found."
                    };
                }

                uint totalPosition = positionsArray.Total();

                if (totalPosition > 0)
                {
                    var dealsPositionTemp = positionsArray.ToArray()
                        .Select(p => new TradeOpenClosedVM
                        {
                            Login = p.Login(),
                            Symbol = p.Symbol(),
                            Time = DateTimeOffset.FromUnixTimeSeconds(p.TimeCreate()).LocalDateTime,
                            Type = p.Action() == 0 ? "BUY" : "SELL",
                            Volume = (decimal)p.Volume() / 10000,
                            PriceOpen = Math.Round(p.PriceOpen(), 5),
                            PriceCurrent = Math.Round(p.PriceCurrent(), 5),
                            StopLoss = Math.Round(p.PriceSL(), 5),
                            TakeProfit = Math.Round(p.PriceTP(), 5),
                            Swap = (decimal)p.Storage(),
                            Profit = Math.Round(p.Profit(), 3),
                            PositionId = p.Position(),
                        }).ToList();

                    tradeOpenClosedVM.AddRange(dealsPositionTemp);
                }

                positionsArray.Release();

                return new BaseResponseModel<List<TradeOpenClosedVM>>
                {
                    Success = true,
                    Message = "Open Trade data retrieved successfully.",
                    Data = tradeOpenClosedVM
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<List<TradeOpenClosedVM>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
