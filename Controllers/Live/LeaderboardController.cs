using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.AspNetCore.Mvc;
using PropMT5Service.Helpers;
using PropMT5Service.Utilities;
using PropMT5Service.ViewModels;
using PropMT5Service.ViewModels.LeaderBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;


namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/leaderboard")]
    public class LeaderboardController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpGet]
        [System.Web.Http.Route("reports")]
        public BaseResponseModel<LeaderboardResponse> MT5LeaderBoard([FromQuery] string loginIds)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginIds))
                    return new BaseResponseModel<LeaderboardResponse> { Success = false, Message = "Please provide at least one LoginId." };

                // Parse comma-separated or single ID into ulong[]
                var loginIdArray = loginIds
                    .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => ulong.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                    .Where(id => id > 0)
                    .Distinct()
                    .ToArray();

                if (loginIdArray.Length == 0)
                    return new BaseResponseModel<LeaderboardResponse> { Success = false, Message = "Invalid LoginIds provided." };

                // --- 1) Collect user + account info ---
                var perUser = new Dictionary<ulong, (CIMTUser user, CIMTAccount acc)>();
                DateTime minRegLocal = DateTime.MaxValue;

                foreach (var login in loginIdArray)
                {
                    CIMTUser cIMTUserc = _manager.UserCreate();
                    var retU = _manager.UserGet(login, cIMTUserc);
                    if (retU != MTRetCode.MT_RET_OK)
                        return new BaseResponseModel<LeaderboardResponse> { Success = false, Message = $"User not found: {login}" };

                    CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                    var retA = _manager.UserAccountGet(login, cIMTAccountInfo);
                    if (retA != MTRetCode.MT_RET_OK)
                        return new BaseResponseModel<LeaderboardResponse> { Success = false, Message = $"Account not found: {login}" };

                    perUser[login] = (cIMTUserc, cIMTAccountInfo);

                    var reg = DateTimeOffset.FromUnixTimeSeconds(cIMTUserc.Registration()).UtcDateTime;
                    if (reg < minRegLocal) minRegLocal = reg;
                }

                if (minRegLocal == DateTime.MaxValue) minRegLocal = DateTime.UtcNow;

                // --- 2) Deals ---
                var from = new DateTimeOffset(minRegLocal.Date.AddDays(-1), TimeSpan.Zero).ToUnixTimeSeconds();
                var to = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(2), TimeSpan.Zero).ToUnixTimeSeconds();

                var dealsArray = _manager.DealCreateArray();
                var retDeals = _manager.DealRequestByLogins(loginIdArray, from, to, dealsArray);
                if (retDeals != MTRetCode.MT_RET_OK)
                {
                    dealsArray.Release();
                    return new BaseResponseModel<LeaderboardResponse> { Success = false, Message = "No deal data found." };
                }

                var allDeals = new List<TradeData>();
                var totalDeals = dealsArray.Total();
                if (totalDeals > 0)
                {
                    allDeals.AddRange(
                        dealsArray.ToArray().Select(d => new TradeData
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
                            Profit = (decimal)d.Profit(),
                            Comment = d.Comment(),
                            Deal = d.Deal(),
                            Login = d.Login(),
                            MarketAsk = (decimal)d.MarketAsk(),
                            MarketBid = (decimal)d.MarketBid(),
                            PositionID = d.PositionID(),
                            RateProfit = (decimal)d.RateProfit(),
                            RateMargin = (decimal)d.RateMargin()
                        })
                    );
                }

                bool IsTradeAction(TradeData t) => (t.Action == 0 || t.Action == 1) && !string.IsNullOrEmpty(t.Symbol);

                // --- 3) Stats per user ---
                var rows = new List<TraderLeaderboardVM>();

                foreach (var kv in perUser)
                {
                    var login = kv.Key;
                    var user = kv.Value.user;
                    var acc = kv.Value.acc;

                    var closedDeals = allDeals.Where(d => d.Login == login && IsTradeAction(d)).OrderBy(d => d.Time1).ToList();

                    var tradesCount = closedDeals.Count;
                    var wins = closedDeals.Count(d => d.Profit > 0);
                    var losses = closedDeals.Count(d => d.Profit < 0);

                    decimal totalProfit = Math.Round(closedDeals.Sum(d => d.Profit), 2);

                    decimal balance = (decimal)Math.Round(acc.Balance(), 2);
                    decimal profitPct = tradesCount > 0 ? Math.Min(100m, Math.Round((totalProfit / tradesCount) * 100m, 2)) : 0m;
                    decimal avgWin = wins > 0 ? Math.Round(closedDeals.Where(d => d.Profit > 0).Average(d => d.Profit), 2) : 0m;
                    decimal avgLoss = losses > 0 ? Math.Round(closedDeals.Where(d => d.Profit < 0).Average(d => d.Profit), 2) : 0m;

                    var pair = closedDeals
                        .GroupBy(d => d.Symbol)
                        .OrderByDescending(g => g.Count())
                        .ThenByDescending(g => Math.Abs(g.Sum(x => x.Profit)))
                        .Select(g => g.Key)
                        .FirstOrDefault() ?? string.Empty;

                    var avgDuration = LeaderboardCalculator.AverageClosedDuration(closedDeals);
                    var (winStreak, lossStreak) = LeaderboardCalculator.CurrentStreaks(closedDeals);
                    var lots = closedDeals.Sum(d => d.Volume) / 10000m;

                    rows.Add(new TraderLeaderboardVM
                    {
                        Login = login,
                        Trader = $"{user.FirstName()} {user.LastName()}".Trim(),
                        Profit = totalProfit,
                        ProfitPercent = profitPct,
                        WinRatio = tradesCount > 0 ? Math.Round((wins * 100m) / tradesCount, 2) : 0m,
                        Pair = pair,
                        AvgWin = avgWin,
                        AvgLoss = avgLoss,
                        AvgDuration = avgDuration,
                        Trades = tradesCount,
                        LosingStreak = lossStreak,
                        WinningStreak = winStreak,
                        TotalLots = lots.ToString("0.00")
                    });
                }

                // --- 4) Rank ---
                var ranked = rows
                    .OrderByDescending(r => r.Profit)
                    .ThenByDescending(r => r.ProfitPercent)
                    .ToList();

                for (int i = 0; i < ranked.Count; i++)
                    ranked[i].Rank = i + 1;

                // --- 5) Cleanup ---
                dealsArray.Release();
                foreach (var kv in perUser)
                {
                    kv.Value.acc.Release();
                    kv.Value.user.Release();
                }

                return new BaseResponseModel<LeaderboardResponse>
                {
                    Success = true,
                    Message = "Leaderboard generated.",
                    Data = new LeaderboardResponse { Rows = ranked }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<LeaderboardResponse>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

    }

}
