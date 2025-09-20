using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5TradingHistoryController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public MT5TradingHistoryController()
        {

        }


        [HttpGet]
        public IEnumerable<MT5TradingHistoryVM> TradingHistoryFromDateToDate(ulong LoginId, string fromDatet, string toDatet)
        {
            try
            {
                //List<MT5TradingHistoryVM> ListTradingHistoryVM = new List<MT5TradingHistoryVM>();

                ulong[] LoginIds = { LoginId };

                #region
                //// Assuming currentDate is a DateTimeOffset representing the current date and time
                //DateTimeOffset currentDate = DateTimeOffset.Now;

                //// Calculate the start date (e.g., 7 days ago from the current date)
                //DateTimeOffset startDate = currentDate.AddDays(-1);

                //// Calculate the start date and end date in seconds since 01.01.1970
                //long fromDate = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                //long toDate = (long)(currentDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                #endregion

                // Assuming DateFormatCovert has a static method FormatDate that returns DateTimeOffset
                DateTimeOffset dateFromString = DateFormatCovert.FormatDate(fromDatet);
                DateTimeOffset dateToString = DateFormatCovert.FormatDate(toDatet);

                DateTimeOffset startDate = dateFromString; //.AddDays(-1);
                DateTimeOffset endDate = dateToString.AddDays(1);

                long fromDateAss = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                long toDateAss = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;


                CIMTDealArray ciMTDealArray = _manager.DealCreateArray();
                MTRetCode mTRetCode12 = _manager.DealRequestByLogins(LoginIds, fromDateAss, toDateAss, ciMTDealArray);

                //CIMTDeal ciMTDeal = _manager.DealCreate();
                //MTRetCode MTRetCode45 = _manager.DealRequest(LoginId, fromDate, toDate, ciMTDealArray);

                if (MTRetCode.MT_RET_OK == mTRetCode12)
                {
                    var dealsMasterTemp = ciMTDealArray.ToArray()
                    .Select(Item => new MT5TradingHistoryVM
                    {
                        Deal = Item.Deal(),
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(Item.TimeMsc()).ToUnixTimeMilliseconds(),
                        ExternalID = Item.ExternalID(),
                        Login = Item.Login(),
                        Dealer = Item.Dealer(),
                        Order = Item.Order(),
                        Action = Item.Action(),
                        Entry = Item.Entry(),
                        Reason = Item.Reason(),
                        Digits = Item.Digits(),
                        DigitsCurrency = Item.DigitsCurrency(),
                        ContractSize = Item.ContractSize(),
                        Time = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                        TimeMsc = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                        Symbol = Item.Symbol(),
                        Price = Item.Price(),
                        VolumeExt = Item.VolumeExt(),
                        Profit = Item.Profit(),
                        Storage = Item.Storage(),
                        Commission = Item.Commission(),
                        Fee = Item.Fee(),
                        RateProfit = Item.RateProfit(),
                        RateMargin = Item.RateMargin(),
                        ExpertID = Item.ExpertID(),
                        PositionID = Item.PositionID(),
                        Comment = Item.Comment(),
                        ProfitRaw = Item.ProfitRaw(),
                        PricePosition = Item.PricePosition(),
                        PriceSL = Item.PriceSL(),
                        PriceTP = Item.PriceTP(),
                        VolumeClosedExt = Item.VolumeClosedExt(),
                        TickValue = Item.TickValue(),
                        TickSize = Item.TickSize(),
                        Flags = Item.Flags(),
                        Value = Item.Value(),
                        Gateway = Item.Gateway(),
                        PriceGateway = Item.PriceGateway(),
                        ModifyFlags = Item.ModificationFlags(),
                        MarketBid = Item.MarketBid(),
                        MarketAsk = Item.MarketAsk(),
                        MarketLast = Item.MarketLast(),
                        Volume = Item.Volume(),
                        VolumeClosed = Item.VolumeClosed(),
                        ApiData = null,
                        mTRetCodeError = mTRetCode12
                    }).OrderByDescending(x => x.Time).ToList();

                    //foreach (var Item in ciMTDealArray.ToArray())
                    //{

                    //    MT5TradingHistoryVM liveAccountVM1 = new MT5TradingHistoryVM()
                    //    {
                    //        Time1 = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                    //        Order = Item.Order(),
                    //        Symbol = Item.Symbol(),
                    //        //Type = Item.GetType(),
                    //        Volume = Item.Volume(),
                    //        Price = Item.Price(),
                    //        PriceSL = Item.PriceSL(),
                    //        PriceTP = Item.PriceTP(),
                    //        Time2 = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                    //        Profit = Item.Profit(),

                    //        //Change = Item.Change()

                    //        // Additional properties

                    //        //Commission = Item.Commission().,
                    //        //DealVolume = Item.Volume(),

                    //        Comment = Item.Comment(),
                    //        Deal = Item.Deal(),
                    //        Login = Item.Login(),
                    //        MarketAsk = Item.MarketAsk(),
                    //        MarketBid = Item.MarketBid(),
                    //        PositionID = Item.PositionID(),
                    //        Print = Item.Print(),
                    //        RateProfit = Item.RateProfit(),
                    //        RateMargin = Item.RateMargin()

                    //    };

                    //    ListTradingHistoryVM.Add(liveAccountVM1);
                    //}

                    ciMTDealArray.Clear();
                    ciMTDealArray.Release();

                    return dealsMasterTemp;
                }
                else
                {
                    MT5TradingHistoryVM liveAccountVM1 = new MT5TradingHistoryVM();
                    liveAccountVM1.mTRetCodeError = mTRetCode12;

                    // Create a list containing the single object
                    List<MT5TradingHistoryVM> resultList = new List<MT5TradingHistoryVM> { liveAccountVM1 };

                    return resultList;

                }


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
