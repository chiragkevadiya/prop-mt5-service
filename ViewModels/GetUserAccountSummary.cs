using MetaQuotes.MT5CommonAPI;
using MT5ConnectionService.ViewModels.TradePosition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class GetUserAccountSummary
    {
        public ulong AccountNumber { get; set; }
        public string Name { get; set; }
        public DateTime? JoinedDate { get; set; }
        public decimal CurrentEquity { get; set; }
        public decimal Balance { get; set; }
        public decimal Profit { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdraw { get; set; }
        public decimal InOut { get; set; } //=> Deposit + Withdraw; // Derived property
        public decimal OpenTradeProfit { get; set; }
        public decimal CloseTradeProfit { get; set; }
        public decimal RMSAmount { get; set; }     // e.g., -1165.03
        public decimal RMSPercentage { get; set; } // e.g., -23.07
        public uint OpenTrades { get; set; }
        public uint CloseTrades { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public uint TotalTrades { get; set; } //=> OpenTrades + CloseTrades;
        public string TotalLots { get; set; }
        public string AverageTradeLength { get; set; }
        public decimal HighestEquity { get; set; }
        public DateTime? HighestEquityDate { get; set; }
        public decimal DrawDown { get; set; }
        public DateTime? DrawDownDate { get; set; }
    }

    public class TradeData
    {
        public ulong Action { get; set; }
        public ulong Entry { get; set; }
        public DateTime? Time1 { get; set; }
        public ulong Order { get; set; }
        public string Symbol { get; set; }
        public decimal Volume { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSL { get; set; }
        public decimal PriceTP { get; set; }
        public DateTime? Time2 { get; set; }
        public decimal Profit { get; set; }
        public string Comment { get; set; }
        public ulong Deal { get; set; }
        public ulong Login { get; set; }
        public decimal MarketAsk { get; set; }
        public decimal MarketBid { get; set; }
        public ulong PositionID { get; set; }
        public decimal RateProfit { get; set; }
        public decimal RateMargin { get; set; }
    }
}
