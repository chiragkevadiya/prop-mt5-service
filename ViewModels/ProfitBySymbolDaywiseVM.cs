using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class SymbolProfitVM
    {
        public string Symbol { get; set; }
        public double TotalProfit { get; set; }
        public int TradeCount { get; set; }
    }

    public class ProfitBySymbolDaywiseVM
    {
        public string Date { get; set; } // Format: yyyy-MM-dd
        public List<SymbolProfitVM> Pairs { get; set; }
    }

    public class ProfitLossBySymbolDaywiseVM
    {
        public List<ProfitBySymbolDaywiseVM> ProfitablePairsChartData { get; set; }
        public List<ProfitBySymbolDaywiseVM> LosingPairsChartData { get; set; }
    }
}
