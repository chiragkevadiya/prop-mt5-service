using MT5ConnectionService.ViewModels.GroupName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels.SymbolName
{
    public class SymbolNameVM
    {
        public string SymbolName { get; set; }
        public double TickSize { get; set; }
        public double TickValue { get; set; }
        public ulong VolumeLimit { get; set; }
        public string Country { get; set; }
        public string CurrencyMargin { get; set; }
        public string Description { get; set; }
        public decimal Spread { get; set; }
        public int StopsLevel { get; set; }
        public string SymbolPath { get; set; }
        public double SymbolPoint { get; set; }
        public string Category { get; set; }
        public string Source { get; set; }
    }
    public class SymbolNameListVM
    {
        public uint SymbolTotal { get; set; }
        public List<SymbolNameVM> SymbolList { get; set; }
    }

}
