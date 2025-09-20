using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MTFiveDepositBalanceVM
    {
        public ulong Login { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
    }
}
