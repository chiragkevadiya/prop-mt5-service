using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MT5TradingHistoryVM
    {
        //public DateTime Time1 { get; set; }
        //public ulong Order { get; set; }
        //public string Symbol { get; set; }

        ////public object Type { get; set; }
        //public decimal Volume { get; set; }
        //public double Price { get; set; }
        //public double PriceSL { get; set; }
        //public double PriceTP { get; set; }
        //public DateTime Time2 { get; set; }
        //public double Profit { get; set; }
        //public decimal Change { get; set; }

        //// Additional properties
        ////public decimal Commission { get; set; }
        ////public decimal DealVolume { get; set; }
        //public string Comment { get; set; }
        //public ulong Deal { get; set; }
        //public ulong Login { get; set; }
        //public double MarketAsk { get; set; }
        //public double MarketBid { get; set; }
        //public ulong PositionID { get; set; }
        //public string Print { get; set; }
        //public double RateProfit { get; set; }
        //public double RateMargin { get; set; }
        //public MTRetCode mTRetCodeError { get; set; }

        public ulong Deal { get; set; }
        public long Timestamp { get; set; }
        public string ExternalID { get; set; }
        public ulong Login { get; set; }
        public ulong Dealer { get; set; }
        public ulong Order { get; set; }
        public uint Action { get; set; }
        public uint Entry { get; set; }
        public uint Reason { get; set; }
        public decimal Digits { get; set; }
        public decimal DigitsCurrency { get; set; }
        public double ContractSize { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeMsc { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public ulong VolumeExt { get; set; }
        public double Profit { get; set; }
        public double Storage { get; set; }
        public double Commission { get; set; }
        public double Fee { get; set; }
        public double RateProfit { get; set; }
        public double RateMargin { get; set; }
        public ulong ExpertID { get; set; }
        public ulong PositionID { get; set; }
        public string Comment { get; set; }
        public double ProfitRaw { get; set; }
        public double PricePosition { get; set; }
        public double PriceSL { get; set; }
        public double PriceTP { get; set; }
        public ulong VolumeClosedExt { get; set; }
        public double TickValue { get; set; }
        public double TickSize { get; set; }
        public ulong Flags { get; set; }
        public double Value { get; set; }
        public string Gateway { get; set; }
        public double PriceGateway { get; set; }
        public uint ModifyFlags { get; set; }
        public double MarketBid { get; set; }
        public double MarketAsk { get; set; }
        public double MarketLast { get; set; }
        public ulong Volume { get; set; }
        public ulong VolumeClosed { get; set; }
        public string ApiData { get; set; }
        public MTRetCode mTRetCodeError { get; set; }
    }
}
