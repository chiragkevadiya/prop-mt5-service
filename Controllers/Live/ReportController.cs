using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for MT5 reports and analytics
    /// </summary>
    [RoutePrefix("api/reports")]
    public class ReportController : BaseApiController
    {
        public ReportController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get account summary report
        /// </summary>
        [HttpGet]
        [Route("account/{loginId:long}/summary")]
        public IHttpActionResult GetAccountSummary(long loginId, DateTime fromDate, DateTime toDate)
        {
            return ExecuteSafe(() =>
            {
                // Get user
                var user = _manager.UserCreate();
                var userResult = _manager.UserGet((ulong)loginId, user);

                if (userResult != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(userResult), 400);
                }

                double balance = user.Balance();
                double credit = user.Credit();

                // Get deals
                long from = new DateTimeOffset(fromDate).ToUnixTimeSeconds();
                long to = new DateTimeOffset(toDate).ToUnixTimeSeconds();

                var dealsArray = _manager.DealCreateArray();
                var dealsResult = _manager.DealRequest((ulong)loginId, from, to, dealsArray);

                double totalProfit = 0;
                double totalCommission = 0;
                double totalSwap = 0;
                int totalTrades = 0;
                int winningTrades = 0;
                int losingTrades = 0;

                if (dealsResult == MTRetCode.MT_RET_OK)
                {
                    uint total = dealsArray.Total();
                    for (uint i = 0; i < total; i++)
                    {
                        var deal = dealsArray.Next(i);
                        if (deal != null && (deal.Action() == 0 || deal.Action() == 1)) // Buy or Sell
                        {
                            totalTrades++;
                            double profit = deal.Profit();
                            totalProfit += profit;
                            totalCommission += deal.Commission();
                            totalSwap += deal.Storage();

                            if (profit > 0)
                                winningTrades++;
                            else if (profit < 0)
                                losingTrades++;
                        }
                    }
                }

                dealsArray.Release();

                // Get positions
                var positionsArray = _manager.PositionCreateArray();
                var positionsResult = _manager.PositionGet((ulong)loginId, positionsArray);
                int openPositions = 0;
                double unrealizedProfit = 0;

                if (positionsResult == MTRetCode.MT_RET_OK)
                {
                    openPositions = (int)positionsArray.Total();
                    for (uint i = 0; i < positionsArray.Total(); i++)
                    {
                        var position = positionsArray.Next(i);
                        if (position != null)
                        {
                            unrealizedProfit += position.Profit();
                        }
                    }
                }

                positionsArray.Release();

                double equity = balance + credit + unrealizedProfit;

                var summary = new
                {
                    Login = loginId,
                    Balance = Math.Round(balance, 2),
                    Credit = Math.Round(credit, 2),
                    Equity = Math.Round(equity, 2),
                    TotalTrades = totalTrades,
                    WinningTrades = winningTrades,
                    LosingTrades = losingTrades,
                    WinRate = totalTrades > 0 ? Math.Round((double)winningTrades / totalTrades * 100, 2) : 0,
                    TotalProfit = Math.Round(totalProfit, 2),
                    TotalCommission = Math.Round(totalCommission, 2),
                    TotalSwap = Math.Round(totalSwap, 2),
                    NetProfit = Math.Round(totalProfit + totalCommission + totalSwap, 2),
                    OpenPositions = openPositions,
                    UnrealizedProfit = Math.Round(unrealizedProfit, 2),
                    FromDate = fromDate,
                    ToDate = toDate
                };

                user.Release();
                return new BaseResponse<object>().WithSuccess(summary, "Account summary retrieved successfully");
            });
        }

        /// <summary>
        /// Get trading statistics for an account
        /// </summary>
        [HttpGet]
        [Route("account/{loginId:long}/statistics")]
        public IHttpActionResult GetTradingStatistics(long loginId, DateTime fromDate, DateTime toDate)
        {
            return ExecuteSafe(() =>
            {
                long from = new DateTimeOffset(fromDate).ToUnixTimeSeconds();
                long to = new DateTimeOffset(toDate).ToUnixTimeSeconds();

                var dealsArray = _manager.DealCreateArray();
                var result = _manager.DealRequest((ulong)loginId, from, to, dealsArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    dealsArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var symbolStats = new Dictionary<string, SymbolStatistics>();
                double totalProfit = 0;
                int totalTrades = 0;
                double maxProfit = double.MinValue;
                double maxLoss = double.MaxValue;
                int consecutiveWins = 0;
                int consecutiveLosses = 0;
                int maxConsecutiveWins = 0;
                int maxConsecutiveLosses = 0;

                uint total = dealsArray.Total();
                for (uint i = 0; i < total; i++)
                {
                    var deal = dealsArray.Next(i);
                    if (deal != null && (deal.Action() == 0 || deal.Action() == 1))
                    {
                        string symbol = deal.Symbol();
                        double profit = deal.Profit();

                        totalTrades++;
                        totalProfit += profit;

                        if (profit > maxProfit) maxProfit = profit;
                        if (profit < maxLoss) maxLoss = profit;

                        // Track consecutive wins/losses
                        if (profit > 0)
                        {
                            consecutiveWins++;
                            consecutiveLosses = 0;
                            if (consecutiveWins > maxConsecutiveWins)
                                maxConsecutiveWins = consecutiveWins;
                        }
                        else if (profit < 0)
                        {
                            consecutiveLosses++;
                            consecutiveWins = 0;
                            if (consecutiveLosses > maxConsecutiveLosses)
                                maxConsecutiveLosses = consecutiveLosses;
                        }

                        // Symbol statistics
                        if (!symbolStats.ContainsKey(symbol))
                        {
                            symbolStats[symbol] = new SymbolStatistics { Symbol = symbol };
                        }

                        symbolStats[symbol].TotalTrades++;
                        symbolStats[symbol].TotalProfit += profit;
                        if (profit > 0)
                            symbolStats[symbol].WinningTrades++;
                        else if (profit < 0)
                            symbolStats[symbol].LosingTrades++;
                    }
                }

                dealsArray.Release();

                var statistics = new
                {
                    TotalTrades = totalTrades,
                    TotalProfit = Math.Round(totalProfit, 2),
                    AverageProfit = totalTrades > 0 ? Math.Round(totalProfit / totalTrades, 2) : 0,
                    MaxProfit = maxProfit != double.MinValue ? Math.Round(maxProfit, 2) : 0,
                    MaxLoss = maxLoss != double.MaxValue ? Math.Round(maxLoss, 2) : 0,
                    MaxConsecutiveWins = maxConsecutiveWins,
                    MaxConsecutiveLosses = maxConsecutiveLosses,
                    SymbolStatistics = symbolStats.Values,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                return new BaseResponse<object>().WithSuccess(statistics, "Trading statistics retrieved successfully");
            });
        }

        /// <summary>
        /// Get daily profit/loss report
        /// </summary>
        [HttpGet]
        [Route("account/{loginId:long}/daily")]
        public IHttpActionResult GetDailyReport(long loginId, DateTime fromDate, DateTime toDate)
        {
            return ExecuteSafe(() =>
            {
                long from = new DateTimeOffset(fromDate).ToUnixTimeSeconds();
                long to = new DateTimeOffset(toDate).ToUnixTimeSeconds();

                var dealsArray = _manager.DealCreateArray();
                var result = _manager.DealRequest((ulong)loginId, from, to, dealsArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    dealsArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var dailyStats = new Dictionary<string, DailyStatistics>();
                uint total = dealsArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var deal = dealsArray.Next(i);
                    if (deal != null && (deal.Action() == 0 || deal.Action() == 1))
                    {
                        var dealDate = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).Date;
                        string dateKey = dealDate.ToString("yyyy-MM-dd");

                        if (!dailyStats.ContainsKey(dateKey))
                        {
                            dailyStats[dateKey] = new DailyStatistics { Date = dealDate };
                        }

                        dailyStats[dateKey].TotalTrades++;
                        dailyStats[dateKey].Profit += deal.Profit();
                        dailyStats[dateKey].Commission += deal.Commission();
                        dailyStats[dateKey].Swap += deal.Storage();

                        if (deal.Profit() > 0)
                            dailyStats[dateKey].WinningTrades++;
                        else if (deal.Profit() < 0)
                            dailyStats[dateKey].LosingTrades++;
                    }
                }

                dealsArray.Release();

                var dailyReport = new List<object>();
                foreach (var stat in dailyStats.Values)
                {
                    dailyReport.Add(new
                    {
                        Date = stat.Date,
                        TotalTrades = stat.TotalTrades,
                        WinningTrades = stat.WinningTrades,
                        LosingTrades = stat.LosingTrades,
                        Profit = Math.Round(stat.Profit, 2),
                        Commission = Math.Round(stat.Commission, 2),
                        Swap = Math.Round(stat.Swap, 2),
                        NetProfit = Math.Round(stat.Profit + stat.Commission + stat.Swap, 2)
                    });
                }

                return new BaseResponse<object>().WithSuccess(dailyReport, "Daily report retrieved successfully");
            });
        }

        /// <summary>
        /// Get group report (all accounts in a group)
        /// </summary>
        [HttpGet]
        [Route("group/{groupName}/summary")]
        public IHttpActionResult GetGroupReport(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var result = _manager.UserAccountRequestArray(groupName, accountArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                int totalAccounts = 0;
                double totalBalance = 0;
                double totalEquity = 0;
                double totalCredit = 0;

                uint total = accountArray.Total();
                for (uint i = 0; i < total; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        totalAccounts++;
                        totalBalance += account.Balance();
                        totalCredit += account.Credit();

                        // Calculate equity with open positions
                        var positionsArray = _manager.PositionCreateArray();
                        if (_manager.PositionGet(account.Login(), positionsArray) == MTRetCode.MT_RET_OK)
                        {
                            double unrealizedProfit = 0;
                            for (uint j = 0; j < positionsArray.Total(); j++)
                            {
                                var position = positionsArray.Next(j);
                                if (position != null)
                                {
                                    unrealizedProfit += position.Profit();
                                }
                            }
                            totalEquity += account.Balance() + account.Credit() + unrealizedProfit;
                        }
                        positionsArray.Release();
                    }
                }

                accountArray.Release();

                var groupReport = new
                {
                    GroupName = groupName,
                    TotalAccounts = totalAccounts,
                    TotalBalance = Math.Round(totalBalance, 2),
                    TotalEquity = Math.Round(totalEquity, 2),
                    TotalCredit = Math.Round(totalCredit, 2),
                    AverageBalance = totalAccounts > 0 ? Math.Round(totalBalance / totalAccounts, 2) : 0,
                    AverageEquity = totalAccounts > 0 ? Math.Round(totalEquity / totalAccounts, 2) : 0
                };

                return new BaseResponse<object>().WithSuccess(groupReport, "Group report retrieved successfully");
            });
        }
    }

    internal class SymbolStatistics
    {
        public string Symbol { get; set; }
        public int TotalTrades { get; set; }
        public double TotalProfit { get; set; }
        public int WinningTrades { get; set; }
        public int LosingTrades { get; set; }
    }

    internal class DailyStatistics
    {
        public DateTime Date { get; set; }
        public int TotalTrades { get; set; }
        public int WinningTrades { get; set; }
        public int LosingTrades { get; set; }
        public double Profit { get; set; }
        public double Commission { get; set; }
        public double Swap { get; set; }
    }
}
