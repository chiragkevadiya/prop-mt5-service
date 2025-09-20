using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels.LeaderBoard
{
    public class TraderLeaderboardVM
    {
        public int Rank { get; set; }
        public ulong Login { get; set; }
        public string Trader { get; set; } = string.Empty;

        public decimal Profit { get; set; }                // total realized profit (closed trades)
        public decimal ProfitPercent { get; set; }         // Profit / Balance * 100 (safe-div)
        public decimal WinRatio { get; set; }              // winning trades / total trades * 100

        public string Pair { get; set; } = string.Empty;   // most-traded symbol (by closed deals)
        public decimal AvgWin { get; set; }                // avg profit of winning trades (>0)
        public decimal AvgLoss { get; set; }               // avg loss of losing trades (<0)
        public string AvgDuration { get; set; } = "0m";    // average position duration (closed)

        public int Trades { get; set; }                    // number of closed trades
        public int LosingStreak { get; set; }              // current consecutive losses
        public int WinningStreak { get; set; }             // current consecutive wins

        public string TotalLots { get; set; } = "0.00";    // sum(volume)/10000 formatted
    }

    public class LeaderboardResponse
    {
        public IReadOnlyList<TraderLeaderboardVM> Rows { get; set; } = Array.Empty<TraderLeaderboardVM>();
    }

}
