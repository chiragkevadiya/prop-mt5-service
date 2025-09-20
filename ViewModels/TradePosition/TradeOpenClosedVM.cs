using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels.TradePosition
{
    public class TradeOpenClosedVM
    {
        public ulong Login { get; set; }
        public string Symbol { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; } // e.g., "Buy" or "Sell"
        public decimal Volume { get; set; }
        public double PriceCurrent { get; set; }
        public double PriceOpen { get; set; }
        public double StopLoss { get; set; } // S/L
        public double TakeProfit { get; set; } // T/P
        public decimal Swap { get; set; }
        public double Profit { get; set; }
        public ulong PositionId { get; set; }
    }

    public class TradeOCP
    {
        public ulong[] Logins { get; set; }
        public int EntryType { get; set; }
    }
}
