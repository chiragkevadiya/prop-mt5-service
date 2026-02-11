using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Utilities;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class LiveTradingDataController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        public LiveTradingDataController()
        {

        }

        [HttpGet]
        public IEnumerable<Mt5TradingDataVM> TradingHistory(ulong loginId, uint entryType, string fromDate, string toDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                    throw new ArgumentException("Date parameters cannot be null or empty.");

                DateTimeOffset startDate = DateFormatConverter.FormatDate(fromDate);
                DateTimeOffset endDate = DateFormatConverter.FormatDate(toDate).AddDays(1);

                long fromTimestamp = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                long toTimestamp = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

                ulong[] loginIds = { loginId };

                // Create deal array
                CIMTDealArray dealArray = _manager.DealCreateArray();

                if (dealArray == null)
                    throw new InvalidOperationException("Failed to create CIMTDealArray.");

                try
                {
                    MTRetCode requestCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);

                    if (requestCode == MTRetCode.MT_RET_OK)
                    {
                        var deals = dealArray.ToArray().Where(deal => deal.Entry() == entryType)
                                   .Select(deal => new Mt5TradingDataVM
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
                                       Profit = deal.Profit(),
                                       mTRetCodeError = requestCode
                                   }).OrderByDescending(deal => deal.Time).ToList();

                        return deals;
                    }
                    else
                    {
                        return new List<Mt5TradingDataVM>
                        {
                            new Mt5TradingDataVM { mTRetCodeError = requestCode }
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
                throw new ApplicationException("An error occurred while fetching trading history.", ex);
            }
        }
    }
}
