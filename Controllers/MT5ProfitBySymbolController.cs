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

namespace MT5ConnectionService.Controllers
{
    public class MT5ProfitBySymbolController : ApiController
    {
        private readonly CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public BaseResponseModel<ProfitLossBySymbolDaywiseVM> GetProfitBySymbolPast5Days(string groupMask = "*")
        {
            try
            {
                // Step 1: Calculate time range (last 5 days)
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-5);

                long fromTimestamp = new DateTimeOffset(startDate).ToUnixTimeSeconds();
                long toTimestamp = new DateTimeOffset(endDate.AddDays(1)).ToUnixTimeSeconds();

                // Step 2: Get deals by group and all symbols
                CIMTDealArray dealArray = _manager.DealCreateArray();
                var resultCode = _manager.DealRequestByGroupSymbol(groupMask, "", fromTimestamp, toTimestamp, dealArray);

                if (resultCode != MTRetCode.MT_RET_OK)
                    return new BaseResponseModel<ProfitLossBySymbolDaywiseVM>
                    {
                        Success = false,
                        Message = "Failed to retrieve deal data",
                        MTRetErrorCode = resultCode,
                        Data = null
                    };

                // Step 3: Parse and group data
                var groupedResult = dealArray.ToArray()
                    .Where(x => !string.IsNullOrEmpty(x.Symbol())) // filter valid symbols
                    .GroupBy(x => new
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds(x.Time()).UtcDateTime.Date,
                        Symbol = x.Symbol()
                    })
                    .Select(g => new
                    {
                        g.Key.Date,
                        g.Key.Symbol,
                        TotalProfit = g.Sum(d => d.Profit()),
                        TradeCount = g.Count()
                    })
                    .ToList();

                // For Profitable Pairs
                var profitGroups = groupedResult
                    .Where(x => x.TotalProfit >= 0)
                    .GroupBy(x => x.Date)
                    .Select(dayGroup => new ProfitBySymbolDaywiseVM
                    {
                        Date = dayGroup.Key.ToString("yyyy-MM-dd"),
                        Pairs = dayGroup
                            .Select(pair => new SymbolProfitVM
                            {
                                Symbol = pair.Symbol,
                                TotalProfit = Math.Round(pair.TotalProfit,3),
                                TradeCount = pair.TradeCount
                            })
                            .OrderByDescending(p => p.TotalProfit)
                            .ToList()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                // For Losing Pairs
                var lossGroups = groupedResult
                    .Where(x => x.TotalProfit < 0)
                    .GroupBy(x => x.Date)
                    .Select(dayGroup => new ProfitBySymbolDaywiseVM
                    {
                        Date = dayGroup.Key.ToString("yyyy-MM-dd"),
                        Pairs = dayGroup
                            .Select(pair => new SymbolProfitVM
                            {
                                Symbol = pair.Symbol,
                                TotalProfit = Math.Round(pair.TotalProfit, 3),
                                TradeCount = pair.TradeCount
                            })
                            .OrderBy(p => p.TotalProfit) // More negative first
                            .ToList()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                // Structure for chart — list of dates with symbol-profit grouping
                var chartData = groupedResult
                    .GroupBy(x => x.Date)
                    .Select(dayGroup => new ProfitBySymbolDaywiseVM
                    {
                        Date = dayGroup.Key.ToString("yyyy-MM-dd"),
                        Pairs = dayGroup.Select(pair => new SymbolProfitVM
                        {
                            Symbol = pair.Symbol,
                            TotalProfit = Math.Round(pair.TotalProfit,3),
                            TradeCount = pair.TradeCount
                        }).OrderByDescending(x => x.TotalProfit).ToList()
                    }).OrderBy(x => x.Date).ToList();

                dealArray.Clear();
                dealArray.Release();

                return new BaseResponseModel<ProfitLossBySymbolDaywiseVM>
                {
                    Success = true,
                    Message = "Profit data by symbol retrieved successfully",
                    Data = new ProfitLossBySymbolDaywiseVM
                    {
                        ProfitablePairsChartData = profitGroups,
                        LosingPairsChartData = lossGroups
                    },
                    MTRetErrorCode = resultCode
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<ProfitLossBySymbolDaywiseVM>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Data = null,
                    MTRetErrorCode = MTRetCode.MT_RET_AUTH_SERVER_INTERNAL
                };
            }
        }
    }

}
