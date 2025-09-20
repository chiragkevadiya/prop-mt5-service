using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5DealRequestByGroupController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public BaseResponseModel<DealMasterList> MT5DealRequestByGroup(string fromDate, string toDate, string actions = null, string byGroups = null)
        {
            try
            {
                string byGroup = byGroups ?? "*"; // * pass all group data get

                DateTimeOffset dateFromString = DateFormatCovert.FormatDate(fromDate);
                DateTimeOffset dateToString = DateFormatCovert.FormatDate(toDate);

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
            catch (Exception)
            {
                throw;
            }
        }


        #region old code
        //[HttpGet]
        //public BaseResponseModel<DealMasterList> MT5DealRequestByGroup(string fromDate, string toDate, string actions = null)
        //{
        //    try
        //    {
        //        //string fromDatet, string toDatet
        //        //string fromDate = "2024-01-01";
        //        //string toDate = "2024-05-13";


        //        // Split the query string into key-value pairs actions
        //        string[] keyValuePairsActions = actions.Split(',');

        //        // Assuming DateFormatCovert has a static method FormatDate that returns DateTimeOffset
        //        DateTimeOffset dateFromString = DateFormatCovert.FormatDate(fromDate);
        //        DateTimeOffset dateToString = DateFormatCovert.FormatDate(toDate);

        //        DateTimeOffset startDate = dateFromString; //.AddDays(-1);
        //        DateTimeOffset endDate = dateToString.AddDays(1);

        //        long fromDateAssign = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
        //        long toDateAssign = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

        //        CIMTDealArray ciMTDealArray = _manager.DealCreateArray();
        //        MTRetCode mTRetCode = _manager.DealRequestByGroup("*", fromDateAssign, toDateAssign, ciMTDealArray);

        //        List<DealMasterVM> dealMastertempList = new List<DealMasterVM>();

        //        if (MTRetCode.MT_RET_OK == mTRetCode)
        //        {
        //            foreach (var Item in ciMTDealArray.ToArray())
        //            {
        //                // Convert the timestamp to a DateTime object

        //                long unixTimestamp = Item.Time();
        //                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
        //                DateTime dateTime = dateTimeOffset.DateTime;


        //                long timestampInMilliseconds = Item.TimeMsc();
        //                DateTime dateTimeMsc = DateTimeOffset.FromUnixTimeMilliseconds(timestampInMilliseconds).DateTime;

        //                long unixTimestampMilliseconds = new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeMilliseconds();

        //                if (Item.Action() == 0 || Item.Action() == 1)
        //                {
        //                    DealMasterVM dealMaster = new DealMasterVM()
        //                    {
        //                        Deal = Item.Deal(),
        //                        Timestamp = unixTimestampMilliseconds,
        //                        ExternalID = Item.ExternalID(),
        //                        Login = Item.Login(),
        //                        Dealer = Item.Dealer(),
        //                        Order = Item.Order(),
        //                        Action = Item.Action(),
        //                        Entry = Item.Entry(),
        //                        Reason = Item.Reason(),
        //                        Digits = Item.Digits(),
        //                        DigitsCurrency = Item.DigitsCurrency(),
        //                        ContractSize = Item.ContractSize(),
        //                        Time = dateTime,
        //                        TimeMsc = dateTimeMsc,
        //                        Symbol = Item.Symbol(),
        //                        Price = Item.Price(),
        //                        VolumeExt = Item.VolumeExt(),
        //                        Profit = Item.Profit(),
        //                        Storage = Item.Storage(),
        //                        Commission = Item.Commission(),
        //                        Fee = Item.Fee(),
        //                        RateProfit = Item.RateProfit(),
        //                        RateMargin = Item.RateMargin(),
        //                        ExpertID = Item.ExpertID(),
        //                        PositionID = Item.PositionID(),
        //                        Comment = Item.Comment(),
        //                        ProfitRaw = Item.ProfitRaw(),
        //                        PricePosition = Item.PricePosition(),
        //                        PriceSL = Item.PriceSL(),
        //                        PriceTP = Item.PriceTP(),
        //                        VolumeClosedExt = Item.VolumeClosedExt(),
        //                        TickValue = Item.TickValue(),
        //                        TickSize = Item.TickSize(),
        //                        Flags = Item.Flags(),
        //                        Value = Item.Value(),
        //                        Gateway = Item.Gateway(),
        //                        PriceGateway = Item.PriceGateway(),
        //                        ModifyFlags = Item.ModificationFlags(),
        //                        MarketBid = Item.MarketBid(),
        //                        MarketAsk = Item.MarketAsk(),
        //                        MarketLast = Item.MarketLast(),
        //                        Volume = Item.Volume(),
        //                        VolumeClosed = Item.VolumeClosed(),
        //                        ApiData = null,
        //                    };
        //                    dealMastertempList.Add(dealMaster);
        //                }
        //            }

        //            DealMasterList dealsMasterTemp = new DealMasterList()
        //            {
        //                DealMasterLists = dealMastertempList,
        //                TotalDeal = dealMastertempList.Count() //ciMTDealArray.Total(),
        //            };

        //            ciMTDealArray.Clear();
        //            ciMTDealArray.Release();

        //            return new BaseResponseModel<DealMasterList> { Data = dealsMasterTemp, Message = "MT5 deal history found.", Success = true, MTRetErrorCode = mTRetCode };
        //        }
        //        return new BaseResponseModel<DealMasterList> { Data = null, Message = "MT5 deal history not found.", Success = false, MTRetErrorCode = mTRetCode };

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        #endregion


    }
}
