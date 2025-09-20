using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using MT5ConnectionService.ViewModels.TradePosition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService
{
    public class TradeAccountOverviewController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public TradeAccountOverviewController()
        {   

        }

        [HttpGet]
        public BaseResponseModel<GetUserAccountSummary> TradeAccountOverview(ulong LoginId)
        {
            try
            {
                List<TradeOpenClosedVM> tradeOpenClosedVM = new List<TradeOpenClosedVM>();
                List<TradeData> tradeallData = new List<TradeData>();

                // history get
                ulong[] LoginIds = { LoginId };

                CIMTPositionArray positionsArray = _manager.PositionCreateArray();
                MTRetCode mTRetCode = _manager.PositionGetByLogins(LoginIds, positionsArray);
                if (MTRetCode.MT_RET_OK != mTRetCode)
                    return new BaseResponseModel<GetUserAccountSummary> { Success = false, Message = "Data Not Found." };
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
                        Volume = Math.Round((decimal)p.Volume() / 10000, 2),
                        PriceOpen = Math.Round(p.PriceOpen(), 2),
                        PriceCurrent = Math.Round(p.PriceCurrent(), 2),
                        StopLoss = Math.Round(p.PriceSL(), 2),
                        TakeProfit = 0,
                        Swap = 0,
                        Profit = Math.Round(p.Profit(), 2),
                    }).ToList();
                    tradeOpenClosedVM.AddRange(dealsPositionTemp);
                }

                CIMTUser userobj = _manager.UserCreate();
                MTRetCode mTRetCode1 = _manager.UserGet(LoginId, userobj);
                if (MTRetCode.MT_RET_OK != mTRetCode1)
                    return new BaseResponseModel<GetUserAccountSummary> { Success = false, Message = "User Not Found." };

                CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                MTRetCode mTRetCode2 = _manager.UserAccountGet(LoginId, cIMTAccountInfo);

                DateTimeOffset dateFromString = DateFormatCovert.FormatDate(DateTimeOffset.FromUnixTimeSeconds(userobj.Registration()).LocalDateTime.ToString("dd-MM-yyyy"));
                DateTimeOffset dateToString = DateFormatCovert.FormatDate(DateTime.UtcNow.ToString("dd-MM-yyyy"));
                DateTimeOffset startDate = dateFromString.AddDays(-1);
                DateTimeOffset endDate = dateToString.AddDays(2);
                long fromDateAss = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                long toDateAss = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;


                CIMTDealArray ciMTDealArray = _manager.DealCreateArray();
                MTRetCode mTRetCode12 = _manager.DealRequestByLogins(LoginIds, fromDateAss, toDateAss, ciMTDealArray);
                uint totalDeal = ciMTDealArray.Total();

                if (totalDeal > 0)
                {
                    var dealsPositionTemp = ciMTDealArray.ToArray()
                    .Select(d => new TradeData
                    {
                        Action = d.Action(),
                        Entry = d.Entry(),
                        Time1 = DateTimeOffset.FromUnixTimeSeconds(d.Time()).UtcDateTime,
                        Order = d.Order(),
                        Symbol = d.Symbol(),
                        Volume = d.Volume(),
                        Price = (decimal)d.Price(),
                        PriceSL = (decimal)d.PriceSL(),
                        PriceTP = (decimal)d.PriceTP(),
                        Time2 = DateTimeOffset.FromUnixTimeSeconds(d.Time()).UtcDateTime,
                        Profit = (decimal)d.Profit(),
                        Comment = d.Comment(),
                        Deal = d.Deal(),
                        Login = d.Login(),
                        MarketAsk = (decimal)d.MarketAsk(),
                        MarketBid = (decimal)d.MarketBid(),
                        PositionID = d.PositionID(),
                        RateProfit = (decimal)d.RateProfit(),
                        RateMargin = (decimal)d.RateMargin()
                    }).ToList();
                    tradeallData.AddRange(dealsPositionTemp);
                }

                // Step 1: Open Trade Profit (unrealized)
                decimal openTradeProfit = (decimal)Math.Round(tradeOpenClosedVM.Sum(x => x.Profit), 2);

                // Step 2: Realized Profit (closed trades)
                decimal realizedTradeProfit = Math.Round(
                    tradeallData.Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null)
                                .Sum(x => x.Profit), 2);

                // Step 3: Calculate RMS loss (assuming it's the absolute loss from open trades)
                decimal rmsLoss = Math.Abs(openTradeProfit); // Use absolute value if openTradeProfit is negative

                // Step 4: Account Capital (must be provided or calculated)
                decimal accountCapital = (decimal)Math.Round(cIMTAccountInfo.Balance(), 2); // actual capital

                // Step 5: Calculate RMS Percentage (avoid division by zero)
                decimal RMSPercentage = accountCapital != 0
                    ? Math.Round((rmsLoss / accountCapital) * 100, 2)
                    : 0;


                // dealLots 
                decimal dealLots = tradeallData
                                        .Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null)
                                        .Sum(x => x.Volume);
                // total lots
                decimal dealLotsTemp = Math.Round(dealLots / 10000, 2);  // Example: outputs 0.10

                // Average Trade Length //Pending Task


                // final data
                var GetUserAccountSummary = new GetUserAccountSummary
                {
                    AccountNumber = userobj.Login(),
                    Name = userobj.FirstName() + ' ' + userobj.LastName(),
                    JoinedDate = DateTimeOffset.FromUnixTimeSeconds(userobj.Registration()).LocalDateTime,
                    CurrentEquity = (decimal)Math.Round(cIMTAccountInfo.Equity(), 2),
                    Balance = (decimal)Math.Round(cIMTAccountInfo.Balance(), 2),
                    Profit = (decimal)Math.Round(cIMTAccountInfo.Profit(), 2),
                    Deposit = (decimal)Math.Round(tradeallData.Where(y => y.Action == 2 && y.Profit > 0).Sum(x => x.Profit), 2),
                    Withdraw = (decimal)Math.Round(tradeallData.Where(y => y.Action == 2 && y.Profit < 0).Sum(x => x.Profit), 2),
                    InOut = (decimal)Math.Round(tradeallData.Where(y => y.Action == 2 && y.Profit > 0).Sum(x => x.Profit) + tradeallData.Where(y => y.Action == 2 && y.Profit < 0).Sum(x => x.Profit), 2),
                    OpenTradeProfit = (decimal)Math.Round(tradeOpenClosedVM.Sum(x => x.Profit), 2),
                    CloseTradeProfit = (decimal)Math.Round(tradeallData.Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null).Sum(x => x.Profit), 2),
                    RMSAmount = (decimal)Math.Round(tradeOpenClosedVM.Sum(x => x.Profit), 2) + (decimal)Math.Round(tradeallData.Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null).Sum(x => x.Profit), 2),
                    RMSPercentage = RMSPercentage,
                    OpenTrades = totalPosition,
                    CloseTrades = (uint)tradeallData.Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null).Count(),
                    LastLoginTime = DateTimeOffset.FromUnixTimeSeconds(userobj.LastAccess()).LocalDateTime,
                    TotalTrades = totalPosition + (uint)tradeallData.Where(x => (x.Action == 1 || x.Action == 0) && x.Symbol != null).Count(),
                    TotalLots = dealLotsTemp.ToString("0.00"),
                    AverageTradeLength = "0 weeks, 0 days, 0 hours, 0 minutes",
                    //Pending Task
                    HighestEquity = 0,
                    HighestEquityDate = null,
                    DrawDown = 0,
                    DrawDownDate = null,
                };

                ciMTDealArray.Release();
                cIMTAccountInfo.Release();
                userobj.Release();
                positionsArray.Release();

                return new BaseResponseModel<GetUserAccountSummary>
                {
                    Success = true,
                    Message = "Open Trade data retrieved successfully.",
                    Data = GetUserAccountSummary
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<GetUserAccountSummary>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        [HttpPost]
        public BaseResponseModel<List<GetUserAccountSummary>> TradeAccountOverview([FromBody] List<ulong> loginIds)
        {
            List<GetUserAccountSummary> summaryList = new List<GetUserAccountSummary>();

            try
            {
                foreach (ulong LoginId in loginIds)
                {
                    // Prepare containers
                    List<TradeOpenClosedVM> tradeOpenClosedVM = new List<TradeOpenClosedVM>();
                    ulong[] LoginIds = { LoginId };

                    // Get open positions
                    CIMTPositionArray positionsArray = _manager.PositionCreateArray();
                    if (_manager.PositionGetByLogins(LoginIds, positionsArray) == MTRetCode.MT_RET_OK)
                    {
                        var openPositions = positionsArray.ToArray()
                            .Select(p => new TradeOpenClosedVM
                            {
                                Login = p.Login(),
                                Symbol = p.Symbol(),
                                Time = DateTimeOffset.FromUnixTimeSeconds(p.TimeCreate()).LocalDateTime,
                                Type = p.Action() == 0 ? "BUY" : "SELL",
                                Volume = Math.Round((decimal)p.Volume() / 10000, 2),
                                PriceOpen = Math.Round(p.PriceOpen(), 2),
                                PriceCurrent = Math.Round(p.PriceCurrent(), 2),
                                StopLoss = Math.Round(p.PriceSL(), 2),
                                TakeProfit = 0,
                                Swap = 0,
                                Profit = Math.Round(p.Profit(), 2),
                            }).ToList();

                        tradeOpenClosedVM.AddRange(openPositions);
                    }

                    // Get user info
                    CIMTUser userobj = _manager.UserCreate();
                    if (_manager.UserGet(LoginId, userobj) != MTRetCode.MT_RET_OK)
                        continue;

                    CIMTAccount accountInfo = _manager.UserCreateAccount();
                    _manager.UserAccountGet(LoginId, accountInfo);

                    // Get deals
                    DateTimeOffset fromDate = DateFormatCovert.FormatDate(DateTimeOffset.FromUnixTimeSeconds(userobj.Registration()).LocalDateTime.ToString("dd-MM-yyyy"));
                    DateTimeOffset toDate = DateFormatCovert.FormatDate(DateTime.UtcNow.ToString("dd-MM-yyyy"));
                    long fromSeconds = (long)(fromDate.AddDays(-1) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                    long toSeconds = (long)(toDate.AddDays(2) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

                    CIMTDealArray dealsArray = _manager.DealCreateArray();
                    _manager.DealRequestByLogins(LoginIds, fromSeconds, toSeconds, dealsArray);

                    var deals = dealsArray.ToArray();

                    // Optimized loop variables
                    decimal realizedTradeProfit = 0;
                    decimal depositTotal = 0;
                    decimal withdrawTotal = 0;
                    decimal inOutTotal = 0;
                    decimal dealLots = 0;
                    uint closeTradesCount = 0;
                    string avgTradeLength = "0 weeks, 0 days, 0 hours, 0 minutes";

                    long totalSecondsForAvg = 0;
                    Dictionary<DateTime, decimal> equityByDate = new Dictionary<DateTime, decimal>();

                    foreach (var d in deals)
                    {
                        if (d.Symbol() != null && (d.Action() == 0 || d.Action() == 1)) // BUY/SELL
                        {
                            // Build equity history
                            var tradeDate = DateTimeOffset.FromUnixTimeSeconds(d.Time()).UtcDateTime.Date;
                            if (!equityByDate.ContainsKey(tradeDate))
                                equityByDate[tradeDate] = 0;
                            equityByDate[tradeDate] += (decimal)d.Profit();
                        }

                        if (d.Action() == 2) // Deposit/Withdraw
                        {
                            inOutTotal += (decimal)d.Profit();
                            if (d.Profit() > 0) depositTotal += (decimal)d.Profit();
                            else withdrawTotal += (decimal)d.Profit();
                        }

                        if (d.Entry() == 1 && d.Symbol() != null) // DEAL_ENTRY_OUT - closed trades
                        {
                            realizedTradeProfit += (decimal)d.Profit();
                            dealLots += d.Volume();
                            closeTradesCount++;

                            // Avg trade length
                            long secondsDiff = DateTimeOffset.FromUnixTimeSeconds(d.Time()).ToUnixTimeSeconds()
                                              - DateTimeOffset.FromUnixTimeSeconds(d.Time()).ToUnixTimeSeconds(); // Adjust if you have open time
                            totalSecondsForAvg += secondsDiff;
                        }
                    }

                    // Calculate Average Trade Length
                    if (closeTradesCount > 0)
                    {
                        var avgSeconds = totalSecondsForAvg / closeTradesCount;
                        var avgSpan = TimeSpan.FromSeconds(avgSeconds);
                        avgTradeLength = $"{avgSpan.Days / 7} weeks, {avgSpan.Days % 7} days, {avgSpan.Hours} hours, {avgSpan.Minutes} minutes";
                    }

                    // Highest Equity & Drawdown
                    decimal highestEquity = 0;
                    DateTime? highestEquityDate = null;
                    decimal peakEquity = decimal.MinValue;
                    decimal maxDrawdown = 0;
                    DateTime? drawdownDate = null;

                    if (equityByDate.Any())
                    {
                        var maxEntry = equityByDate.OrderByDescending(x => x.Value).First();
                        highestEquity = maxEntry.Value;
                        highestEquityDate = maxEntry.Key;

                        foreach (var kv in equityByDate.OrderBy(x => x.Key))
                        {
                            if (kv.Value > peakEquity)
                                peakEquity = kv.Value;

                            var drawdown = peakEquity - kv.Value;
                            if (drawdown > maxDrawdown)
                            {
                                maxDrawdown = drawdown;
                                drawdownDate = kv.Key;
                            }
                        }
                    }

                    // Final summary
                    decimal openTradeProfit = (decimal)Math.Round(tradeOpenClosedVM.Sum(x => x.Profit), 2);
                    decimal accountCapital = (decimal)Math.Round(accountInfo.Balance(), 2);
                    decimal RMSPercentage = accountCapital != 0 ? Math.Round((Math.Abs(openTradeProfit) / accountCapital) * 100, 2) : 0;

                    summaryList.Add(new GetUserAccountSummary
                    {
                        AccountNumber = userobj.Login(),
                        Name = userobj.FirstName() + ' ' + userobj.LastName(),
                        JoinedDate = DateTimeOffset.FromUnixTimeSeconds(userobj.Registration()).LocalDateTime,
                        CurrentEquity = (decimal)Math.Round(accountInfo.Equity(), 2),
                        Balance = accountCapital,
                        Profit = (decimal)Math.Round(accountInfo.Profit(), 2),
                        Deposit = Math.Round(depositTotal, 2),
                        Withdraw = Math.Round(withdrawTotal, 2),
                        InOut = Math.Round(inOutTotal, 2),
                        OpenTradeProfit = openTradeProfit,
                        CloseTradeProfit = Math.Round(realizedTradeProfit, 2),
                        RMSAmount = openTradeProfit + realizedTradeProfit,
                        RMSPercentage = RMSPercentage,
                        OpenTrades = positionsArray.Total(),
                        CloseTrades = closeTradesCount,
                        LastLoginTime = DateTimeOffset.FromUnixTimeSeconds(userobj.LastAccess()).LocalDateTime,
                        TotalTrades = positionsArray.Total() + closeTradesCount,
                        TotalLots = Math.Round(dealLots / 10000, 2).ToString("0.00"),
                        AverageTradeLength = avgTradeLength,
                        HighestEquity = highestEquity,
                        HighestEquityDate = highestEquityDate,
                        DrawDown = maxDrawdown,
                        DrawDownDate = drawdownDate
                    });

                    // Release resources
                    dealsArray.Release();
                    accountInfo.Release();
                    userobj.Release();
                    positionsArray.Release();
                }

                return new BaseResponseModel<List<GetUserAccountSummary>>
                {
                    Success = true,
                    Message = "Account summary retrieved.",
                    Data = summaryList
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<List<GetUserAccountSummary>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
