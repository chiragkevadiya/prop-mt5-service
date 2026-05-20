using MetaQuotes.MT5CommonAPI;
using System;

namespace PropMT5Service.ViewModels
{
    public class Mt5TradingDataVM
    {
        public ulong Deal { get; set; }
        public string Symbol { get; set; }
        public long Timestamp { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeMsc { get; set; }
        public ulong Login { get; set; }
        public ulong PositionID { get; set; }
        public string Action { get; set; }
        public string Entry { get; set; }
        public ulong Volume { get; set; }
        public double Swap { get; set; }
        public double Price { get; set; }
        public double PriceSL { get; set; }
        public double PriceTP { get; set; }
        public double Profit { get; set; }
        public MTRetCode mTRetCodeError { get; set; }
    }
}
