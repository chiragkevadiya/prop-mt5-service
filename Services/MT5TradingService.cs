using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Utilities;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services
{
    /// <summary>
    /// Interface for trading operations
    /// </summary>
    public interface IMT5TradingService
    {
        Task<BaseResponse<List<Mt5TradingHistoryVM>>> GetTradingHistoryAsync(ulong loginId, string fromDate, string toDate);
        Task<BaseResponse<List<Mt5TradingDataVM>>> GetTradingDataAsync(ulong loginId, uint entryType, string fromDate, string toDate);
        Task<BaseResponse<List<ClosedTradeResponse>>> ClosePositionsAsync(ClosePositionRequest request);
        Task<BaseResponse<List<Mt5TradingDataVM>>> GetOpenTradesAsync(ulong loginId);
        Task<BaseResponse<List<Mt5TradingDataVM>>> GetClosedTradesAsync(ulong loginId, string fromDate, string toDate);
    }

    /// <summary>
    /// Service for handling MT5 trading operations
    /// </summary>
    public class MT5TradingService : IMT5TradingService
    {
        private readonly CIMTManagerAPI _manager;

        public MT5TradingService(CIMTManagerAPI manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <summary>
        /// Get trading history for a login between dates
        /// </summary>
        public async Task<BaseResponse<List<Mt5TradingHistoryVM>>> GetTradingHistoryAsync(ulong loginId, string fromDate, string toDate)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                    {
                        return new BaseResponse<List<Mt5TradingHistoryVM>>
                        {
                            Success = false,
                            Message = "Date parameters cannot be null or empty",
                            StatusCode = 400
                        };
                    }

                    var (fromTimestamp, toTimestamp) = ConvertDateRange(fromDate, toDate);
                    ulong[] loginIds = { loginId };

                    CIMTDealArray dealArray = _manager.DealCreateArray();
                    if (dealArray == null)
                    {
                        return new BaseResponse<List<Mt5TradingHistoryVM>>
                        {
                            Success = false,
                            Message = "Failed to create deal array",
                            StatusCode = 500
                        };
                    }

                    try
                    {
                        MTRetCode retCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);

                        if (retCode == MTRetCode.MT_RET_OK)
                        {
                            var deals = MapToTradingHistory(dealArray);

                            return new BaseResponse<List<Mt5TradingHistoryVM>>
                            {
                                Success = true,
                                Message = "Trading history retrieved successfully",
                                Data = deals,
                                StatusCode = 200
                            };
                        }
                        else
                        {
                            return new BaseResponse<List<Mt5TradingHistoryVM>>
                            {
                                Success = false,
                                Message = GetErrorMessage(retCode),
                                StatusCode = 400
                            };
                        }
                    }
                    finally
                    {
                        dealArray.Clear();
                        dealArray.Release();
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<Mt5TradingHistoryVM>>
                    {
                        Success = false,
                        Message = $"Error retrieving trading history: {ex.Message}",
                        StatusCode = 500
                    };
                }
            });
        }

        /// <summary>
        /// Get trading data filtered by entry type
        /// </summary>
        public async Task<BaseResponse<List<Mt5TradingDataVM>>> GetTradingDataAsync(ulong loginId, uint entryType, string fromDate, string toDate)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                    {
                        return new BaseResponse<List<Mt5TradingDataVM>>
                        {
                            Success = false,
                            Message = "Date parameters cannot be null or empty",
                            StatusCode = 400
                        };
                    }

                    var (fromTimestamp, toTimestamp) = ConvertDateRange(fromDate, toDate);
                    ulong[] loginIds = { loginId };

                    CIMTDealArray dealArray = _manager.DealCreateArray();
                    if (dealArray == null)
                    {
                        return new BaseResponse<List<Mt5TradingDataVM>>
                        {
                            Success = false,
                            Message = "Failed to create deal array",
                            StatusCode = 500
                        };
                    }

                    try
                    {
                        MTRetCode retCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);

                        if (retCode == MTRetCode.MT_RET_OK)
                        {
                            var deals = MapToTradingData(dealArray, entryType);

                            return new BaseResponse<List<Mt5TradingDataVM>>
                            {
                                Success = true,
                                Message = "Trading data retrieved successfully",
                                Data = deals,
                                StatusCode = 200
                            };
                        }
                        else
                        {
                            return new BaseResponse<List<Mt5TradingDataVM>>
                            {
                                Success = false,
                                Message = GetErrorMessage(retCode),
                                StatusCode = 400
                            };
                        }
                    }
                    finally
                    {
                        dealArray.Clear();
                        dealArray.Release();
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<Mt5TradingDataVM>>
                    {
                        Success = false,
                        Message = $"Error retrieving trading data: {ex.Message}",
                        StatusCode = 500
                    };
                }
            });
        }

        /// <summary>
        /// Get open trades for a login
        /// </summary>
        public async Task<BaseResponse<List<Mt5TradingDataVM>>> GetOpenTradesAsync(ulong loginId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    CIMTPositionArray positions = _manager.PositionCreateArray();
                    if (positions == null)
                    {
                        return new BaseResponse<List<Mt5TradingDataVM>>
                        {
                            Success = false,
                            Message = "Failed to create position array",
                            StatusCode = 500
                        };
                    }

                    try
                    {
                        MTRetCode retCode = _manager.PositionGet(loginId, positions);

                        if (retCode == MTRetCode.MT_RET_OK)
                        {
                            var openTrades = MapPositionsToTradingData(positions);

                            return new BaseResponse<List<Mt5TradingDataVM>>
                            {
                                Success = true,
                                Message = "Open trades retrieved successfully",
                                Data = openTrades,
                                StatusCode = 200
                            };
                        }
                        else
                        {
                            return new BaseResponse<List<Mt5TradingDataVM>>
                            {
                                Success = false,
                                Message = GetErrorMessage(retCode),
                                StatusCode = 400
                            };
                        }
                    }
                    finally
                    {
                        positions.Release();
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<Mt5TradingDataVM>>
                    {
                        Success = false,
                        Message = $"Error retrieving open trades: {ex.Message}",
                        StatusCode = 500
                    };
                }
            });
        }

        /// <summary>
        /// Get closed trades for a login
        /// </summary>
        public async Task<BaseResponse<List<Mt5TradingDataVM>>> GetClosedTradesAsync(ulong loginId, string fromDate, string toDate)
        {
            return await GetTradingDataAsync(loginId, 1, fromDate, toDate); // Entry type 1 = closed
        }

        /// <summary>
        /// Close multiple positions
        /// </summary>
        public async Task<BaseResponse<List<ClosedTradeResponse>>> ClosePositionsAsync(ClosePositionRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (request == null || request.PositionId == null || request.PositionId.Count == 0)
                    {
                        return new BaseResponse<List<ClosedTradeResponse>>
                        {
                            Success = false,
                            Message = "Invalid request: Position IDs are required",
                            StatusCode = 400
                        };
                    }

                    CIMTPositionArray positions = _manager.PositionCreateArray();
                    if (positions == null)
                    {
                        return new BaseResponse<List<ClosedTradeResponse>>
                        {
                            Success = false,
                            Message = "Failed to create position array",
                            StatusCode = 500
                        };
                    }

                    try
                    {
                        MTRetCode retCode = _manager.PositionGet(request.LoginId, positions);
                        if (retCode != MTRetCode.MT_RET_OK || positions.Total() == 0)
                        {
                            return new BaseResponse<List<ClosedTradeResponse>>
                            {
                                Success = false,
                                Message = "No positions found for the specified login",
                                StatusCode = 404
                            };
                        }

                        var closedTrades = new List<ClosedTradeResponse>();
                        var errors = new List<string>();

                        for (uint i = 0; i < positions.Total(); i++)
                        {
                            CIMTPosition position = positions.Next(i);
                            if (position == null || !request.PositionId.Contains(position.Position()))
                                continue;

                            var closeResult = ClosePosition(position);
                            if (closeResult.Success)
                            {
                                closedTrades.Add(closeResult.Trade);
                            }
                            else
                            {
                                errors.Add($"Position {position.Position()}: {closeResult.Error}");
                            }
                        }

                        if (closedTrades.Count == 0)
                        {
                            return new BaseResponse<List<ClosedTradeResponse>>
                            {
                                Success = false,
                                Message = errors.Count > 0 
                                    ? $"Failed to close positions: {string.Join("; ", errors)}" 
                                    : "No matching positions found to close",
                                StatusCode = 400
                            };
                        }

                        return new BaseResponse<List<ClosedTradeResponse>>
                        {
                            Success = true,
                            Message = $"Successfully closed {closedTrades.Count} position(s)" + 
                                     (errors.Count > 0 ? $". Errors: {string.Join("; ", errors)}" : ""),
                            Data = closedTrades,
                            StatusCode = 200
                        };
                    }
                    finally
                    {
                        positions.Release();
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<List<ClosedTradeResponse>>
                    {
                        Success = false,
                        Message = $"Error closing positions: {ex.Message}",
                        StatusCode = 500
                    };
                }
            });
        }

        #region Private Helper Methods

        private (long fromTimestamp, long toTimestamp) ConvertDateRange(string fromDate, string toDate)
        {
            DateTimeOffset startDate = DateFormatConverter.FormatDate(fromDate);
            DateTimeOffset endDate = DateFormatConverter.FormatDate(toDate).AddDays(1);

            long fromTimestamp = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
            long toTimestamp = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

            return (fromTimestamp, toTimestamp);
        }

        private List<Mt5TradingHistoryVM> MapToTradingHistory(CIMTDealArray dealArray)
        {
            return dealArray.ToArray()
                .Select(deal => new Mt5TradingHistoryVM
                {
                    Deal = deal.Deal(),
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(deal.TimeMsc()).ToUnixTimeMilliseconds(),
                    ExternalID = deal.ExternalID(),
                    Login = deal.Login(),
                    Dealer = deal.Dealer(),
                    Order = deal.Order(),
                    Action = deal.Action(),
                    Entry = deal.Entry(),
                    Reason = deal.Reason(),
                    Digits = deal.Digits(),
                    DigitsCurrency = deal.DigitsCurrency(),
                    ContractSize = deal.ContractSize(),
                    Time = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).DateTime,
                    TimeMsc = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).DateTime,
                    Symbol = deal.Symbol(),
                    Price = deal.Price(),
                    VolumeExt = deal.VolumeExt(),
                    Profit = deal.Profit(),
                    Storage = deal.Storage(),
                    Commission = deal.Commission(),
                    Fee = deal.Fee(),
                    RateProfit = deal.RateProfit(),
                    RateMargin = deal.RateMargin(),
                    ExpertID = deal.ExpertID(),
                    PositionID = deal.PositionID(),
                    Comment = deal.Comment(),
                    ProfitRaw = deal.ProfitRaw(),
                    PricePosition = deal.PricePosition(),
                    PriceSL = deal.PriceSL(),
                    PriceTP = deal.PriceTP(),
                    VolumeClosedExt = deal.VolumeClosedExt(),
                    TickValue = deal.TickValue(),
                    TickSize = deal.TickSize(),
                    Flags = deal.Flags(),
                    Value = deal.Value(),
                    Gateway = deal.Gateway(),
                    PriceGateway = deal.PriceGateway(),
                    ModifyFlags = deal.ModificationFlags(),
                    MarketBid = deal.MarketBid(),
                    MarketAsk = deal.MarketAsk(),
                    MarketLast = deal.MarketLast(),
                    Volume = deal.Volume(),
                    VolumeClosed = deal.VolumeClosed()
                })
                .OrderByDescending(x => x.Time)
                .ToList();
        }

        private List<Mt5TradingDataVM> MapToTradingData(CIMTDealArray dealArray, uint? entryTypeFilter = null)
        {
            var deals = dealArray.ToArray().AsEnumerable();

            if (entryTypeFilter.HasValue)
            {
                deals = deals.Where(deal => deal.Entry() == entryTypeFilter.Value);
            }

            return deals.Select(deal => new Mt5TradingDataVM
                {
                    Deal = deal.Deal(),
                    Symbol = deal.Symbol(),
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(deal.TimeMsc()).ToUnixTimeMilliseconds(),
                    Time = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).DateTime,
                    TimeMsc = DateTimeOffset.FromUnixTimeMilliseconds(deal.TimeMsc()).DateTime,
                    Login = deal.Login(),
                    PositionID = deal.PositionID(),
                    Action = deal.Action() == 0 ? "Buy" : "Sell",
                    Entry = deal.Entry() == 0 ? "Open" : "Close",
                    Volume = deal.Volume(),
                    Swap = deal.Storage(),
                    Price = deal.Price(),
                    PriceSL = deal.PriceSL(),
                    PriceTP = deal.PriceTP(),
                    Profit = deal.Profit()
                })
                .OrderByDescending(deal => deal.Time)
                .ToList();
        }

        private List<Mt5TradingDataVM> MapPositionsToTradingData(CIMTPositionArray positions)
        {
            var result = new List<Mt5TradingDataVM>();

            for (uint i = 0; i < positions.Total(); i++)
            {
                CIMTPosition position = positions.Next(i);
                if (position != null)
                {
                    result.Add(new Mt5TradingDataVM
                    {
                        PositionID = position.Position(),
                        Symbol = position.Symbol(),
                        Login = position.Login(),
                        Action = position.Action() == (uint)CIMTPosition.EnPositionAction.POSITION_BUY ? "Buy" : "Sell",
                        Entry = "Open",
                        Volume = position.Volume(),
                        Price = position.PriceOpen(),
                        PriceSL = position.PriceSL(),
                        PriceTP = position.PriceTP(),
                        Profit = position.Profit(),
                        Swap = position.Storage(),
                        Time = DateTimeOffset.FromUnixTimeSeconds(position.TimeCreate()).DateTime
                    });
                }
            }

            return result.OrderByDescending(x => x.Time).ToList();
        }

        private (bool Success, ClosedTradeResponse Trade, string Error) ClosePosition(CIMTPosition position)
        {
            CIMTDeal deal = _manager.DealCreate();
            if (deal == null)
            {
                return (false, null, "Failed to create deal");
            }

            try
            {
                deal.Login(position.Login());
                deal.Symbol(position.Symbol());
                deal.Action(position.Action() == (uint)CIMTPosition.EnPositionAction.POSITION_BUY
                    ? (uint)CIMTDeal.EnDealAction.DEAL_SELL
                    : (uint)CIMTDeal.EnDealAction.DEAL_BUY);
                deal.Volume(position.Volume());
                deal.Price(position.PriceCurrent());
                deal.PositionID(position.Position());
                deal.Entry((uint)CIMTDeal.EnEntryFlag.ENTRY_OUT);
                deal.ReasonSet((uint)CIMTDeal.EnDealReason.DEAL_REASON_CLIENT);

                MTRetCode dealResult = _manager.DealPerform(deal);

                if (dealResult == MTRetCode.MT_RET_OK)
                {
                    var closedTrade = new ClosedTradeResponse
                    {
                        DealId = deal.Deal(),
                        PositionId = position.Position(),
                        Symbol = deal.Symbol(),
                        Volume = deal.Volume(),
                        Price = deal.Price(),
                        PriceOpen = position.PriceOpen(),
                        Profit = deal.Profit(),
                        Commission = deal.Commission(),
                        Swap = deal.Storage(),
                        Action = deal.Action(),
                        Entry = deal.Entry(),
                        Reason = deal.Reason(),
                        Time = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).UtcDateTime,
                        Login = deal.Login(),
                        Order = deal.Order()
                    };

                    return (true, closedTrade, null);
                }
                else
                {
                    return (false, null, $"Deal perform failed: {dealResult}");
                }
            }
            finally
            {
                deal.Release();
            }
        }

        private string GetErrorMessage(MTRetCode retCode)
        {
            if (retCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                return "No data found for the specified criteria";
            if (retCode == MTRetCode.MT_RET_ERR_PARAMS)
                return "Invalid parameters provided";
            if (retCode == MTRetCode.MT_RET_ERR_CONNECTION)
                return "Connection error";
            
            return $"Operation failed with code: {retCode}";
        }

        #endregion
    }
}
