using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class AccountPerformanceVM
    {
        public ulong Login { get; set; }              // MT5 Account Login ID
        public double Balance { get; set; }         // Account balance
        public double Credit { get; set; }          // Credit (if any)
        public double Equity { get; set; }          // Current equity
        public double Profit { get; set; }          // Current floating profit/loss
    }
}
