using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MT5LiveDashboardDetailVM
    {
        public uint MT5OnlineUsers { get; set; }
        public uint MT5TotalAccounts { get; set; }
        public int MT5ActiveAccounts { get; set; }
        public int MT5InactiveAccounts { get; set; }
        public double MT5ActualProfitLoss { get; set; }
    }
}
