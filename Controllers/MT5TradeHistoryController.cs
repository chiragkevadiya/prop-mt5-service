using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using NaptunePropTrading_Service.Helper;
using NaptunePropTrading_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NaptunePropTrading_Service.Controllers
{
    public class MT5TradeHistoryController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public MT5TradeHistoryController()
        {

        }


        [HttpGet]
        public BaseResponse GetMT5TradeHistory(ulong LoginId, string fromDate, string toDate, double challengeDailyLossLimit, double challengeTotalLossLimit, double equity)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                    throw new ArgumentException("Date parameters cannot be null or empty.");

                ulong[] loginIds = { LoginId };

                // Get timestamps for today and total history
                long fromDateToday = GetUnixTimestamp(DateTimeOffset.UtcNow.Date);
                long toDateToday = GetUnixTimestamp(DateTimeOffset.UtcNow.Date.AddDays(1));

                long fromDateTotal = GetUnixTimestamp(DateFormatCovert.FormatDate(fromDate));
                long toDateTotal = GetUnixTimestamp(DateFormatCovert.FormatDate(toDate).AddDays(1));

                // Fetch deal history
                List<MT5TradingHistoryVM> todayDeals = FetchDealHistory(loginIds, fromDateToday, toDateToday);
                List<MT5TradingHistoryVM> totalDeals = FetchDealHistory(loginIds, fromDateTotal, toDateTotal);

                if (todayDeals.Count != 0 || totalDeals.Count != 0)
                {
                    // Calculate total profit and loss
                    double totalDailyProfit = todayDeals.Any() ? todayDeals.Sum(d => d.Profit) : 0;
                    double totalProfit = totalDeals.Any() ? totalDeals.Sum(d => d.Profit) : 0;

                    // Calculate total loss (only negative values considered)
                    double totalDailyLoss = todayDeals.Where(d => d.Profit < 0).Sum(d => d.Profit);
                    double totalLoss = totalDeals.Where(d => d.Profit < 0).Sum(d => d.Profit);


                    // Convert to percentage (only if initial balance > 0)
                    double dailyLossPercentage = (totalDailyLoss < 0) ? (totalDailyLoss * 100) / equity : 0;
                    double totalLossPercentage = (totalLoss < 0) ? (totalLoss * 100) / equity : 0;

                    double totalDailyLoss1 = Math.Abs(Math.Round(dailyLossPercentage, 2));
                    double totalLoss1 = Math.Abs(Math.Round(totalLossPercentage, 2));


                    //Console.WriteLine($"Equity: {equity}, Total Daily Loss: {totalDailyLoss}, Daily Loss Percentage: {dailyLossPercentage}%, Total Loss: {totalLoss}, Total Loss Percentage: {totalLossPercentage}%");


                    // Define challenge limits and Check conditions and take action
                    if (totalDailyLoss1 > challengeDailyLossLimit && totalDailyLoss1 != 0)
                    {
                        CloseMT5Account(LoginId, "Daily loss exceeded");
                        return new BaseResponse { Success = true, Message = "Daily loss exceeded." };
                    }

                    if (totalLoss1 > challengeTotalLossLimit && totalLoss1 != 0)
                    {
                        CloseMT5Account(LoginId, "Total loss exceeded");
                        return new BaseResponse { Success = true, Message = "Total loss exceeded." };
                    }

                    // If no conditions were met, log no action
                    LogManager.Log_MT5AccountClosedNoAction("No action", $"No action required {loginIds}.");
                    return new BaseResponse { Success = false, Message = "No action required." };
                }

                return new BaseResponse { Success = false, Message = "No action required." };

                #region Main Code

                // Function to fetch deal history
                List<MT5TradingHistoryVM> FetchDealHistory(ulong[] accountLogins, long startTime, long endTime)
                {
                    using (CIMTDealArray dealArray = _manager.DealCreateArray()) // Ensuring proper memory management
                    {
                        if (_manager.DealRequestByLogins(accountLogins, startTime, endTime, dealArray) == MTRetCode.MT_RET_OK)
                        {
                            return dealArray.ToArray()
                                .Where(x => x.Action() == 0 || x.Action() == 1)
                                .Select(deal => new MT5TradingHistoryVM
                                {
                                    Login = deal.Login(),
                                    Profit = deal.Profit(),
                                    Time = DateTimeOffset.FromUnixTimeSeconds(deal.Time()).DateTime
                                }).OrderByDescending(x => x.Time).ToList();
                        }

                        dealArray.Release();
                        dealArray.Clear();
                    }

                    LogManager.LogError("fetching deals", "Error fetching deals or no data found.");
                    return new List<MT5TradingHistoryVM>();
                }

                // Function to close the MT5 account
                void CloseMT5Account(ulong loginId, string reason)
                {
                    // Add API call to close the account here
                    CIMTUser cIMTUser = _manager.UserCreate();
                    MTRetCode resultCode = _manager.UserGet(loginId, cIMTUser);
                    if (MTRetCode.MT_RET_OK == resultCode)
                    {
                        // DISABLED the account
                        cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED);
                        // Send changes
                        MTRetCode updateResult = _manager.UserUpdate(cIMTUser);

                        LogManager.LogSuccess_MT5AccountClosed($"Closing MT5 account {loginId}", $"Closing MT5 account {loginId}. Reason: {reason}");

                    }
                    cIMTUser.Release();
                    cIMTUser.Clear();
                }

                // Helper function to get Unix timestamp
                long GetUnixTimestamp(DateTimeOffset date) =>
                    (long)(date - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
