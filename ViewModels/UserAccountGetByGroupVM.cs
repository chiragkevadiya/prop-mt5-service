using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class UserAccountGetByGroupVM
    {
        public ulong Login { get; set; }
        public double Balance { get; set; }
        public double Credit { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
    }
}
