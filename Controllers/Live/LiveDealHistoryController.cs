using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.Utilities;
using PropMT5Service.ViewModels;
using System;
using System.Linq;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/livedealhistory")]
    public class LiveDealHistoryController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpGet]
        [Route("")]
        public BaseResponseModel<DealMasterList> MT5DealRequestByGroup(string fromDate, string toDate, string actions = null, string byGroups = null)
        {
            string byGroup = byGroups ?? "*"; // * pass all group data get

            DateTimeOffset dateFromString = DateFormatConverter.FormatDate(fromDate);
            DateTimeOffset dateToString = DateFormatConverter.FormatDate(toDate);

            DateTimeOffset startDate = dateFromString;
            DateTimeOffset endDate = dateToString.AddDays(2);

            long fromDateAssign = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
            long toDateAssign = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

            CIMTDealArray ciMTDealArray = _manager.DealCreateArray();
            MTRetCode mTRetCode = _manager.DealRequestByGroup(byGroup, fromDateAssign, toDateAssign, ciMTDealArray);

            if (mTRetCode != MTRetCode.MT_RET_OK)
                return new BaseResponseModel<DealMasterList> { Data = null, Message = "Deal data not found.", Success = false, MTRetErrorCode = mTRetCode };

            var actionsList = actions?.Split(',') ?? null; // Split actions string by comma or initialize empty list if actions is null

            if (actionsList != null && actionsList.Any())
            {
                var dealsMasterTemp = ciMTDealArray.ToArray()
                .Where(Item => actionsList.Contains(Item.Action().ToString()))
                .Select(Item => new DealMasterVM
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
                    TimeMsc = DateTimeOffset.FromUnixTimeMilliseconds(Item.TimeMsc()).DateTime,
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
                }).OrderByDescending(x => x.Time).ToList();

                ciMTDealArray.Clear();
                ciMTDealArray.Release();

                return new BaseResponseModel<DealMasterList> { Data = new DealMasterList { DealMasterLists = dealsMasterTemp, TotalDeal = dealsMasterTemp.Count }, Message = "Deal data retrieved successfully.", Success = true, MTRetErrorCode = mTRetCode };
            }
            else
            {
                var dealsMasterTemp = ciMTDealArray.ToArray()
                //.Where(Item => Item.Action() == 0 || Item.Action() == 1)
                .Select(Item => new DealMasterVM
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
                    TimeMsc = DateTimeOffset.FromUnixTimeMilliseconds(Item.TimeMsc()).DateTime,
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
                }).OrderByDescending(x => x.Time).ToList();

                ciMTDealArray.Clear();
                ciMTDealArray.Release();

                return new BaseResponseModel<DealMasterList> { Data = new DealMasterList { DealMasterLists = dealsMasterTemp, TotalDeal = dealsMasterTemp.Count }, Message = "Deal data retrieved successfully.", Success = true, MTRetErrorCode = mTRetCode };
            }

        }
    }
}
