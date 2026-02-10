using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5TradingHistoryController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public IEnumerable<MT5TradingHistoryVM> TradingHistoryFromDateToDate(ulong LoginId, string fromDatet, string toDatet)
        {
            try
            {
                ulong[] LoginIds = { LoginId };

                DateTimeOffset dateFromString = DateFormatCovert.FormatDate(fromDatet);
                DateTimeOffset dateToString = DateFormatCovert.FormatDate(toDatet);

                DateTimeOffset startDate = dateFromString;
                DateTimeOffset endDate = dateToString.AddDays(1);

                long fromDateAss = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                long toDateAss = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

                CIMTDealArray ciMTDealArray = _manager.DealCreateArray();
                MTRetCode mTRetCode12 = _manager.DealRequestByLogins(LoginIds, fromDateAss, toDateAss, ciMTDealArray);

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

                    ciMTDealArray.Clear();
                    ciMTDealArray.Release();

                    return dealsMasterTemp;
                }
                else
                {
                    MT5TradingHistoryVM liveAccountVM1 = new MT5TradingHistoryVM();
                    liveAccountVM1.mTRetCodeError = mTRetCode12;

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
