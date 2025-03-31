using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.ViewModels
{
    public class MT5TradingHistoryVM
    {
        public ulong Login { get; set; }
        public DateTime Time { get; set; }
        public double Profit { get; set; }
        //public double RateProfit { get; set; }
        //public double ProfitRaw { get; set; }
        //public uint Action { get; set; }
    }
}
