using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MtGetAllLiveAccountVM
    {
        public ulong Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Group { get; set; }
        public string Country { get; set; }
        public double Credit { get; set; }
        public double Balance { get; set; }
        public uint Leverage { get; set; }
        public double Margin { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
        public double Commission { get; set; }
        public double Equity { get; set; }
        public double BalancePrevDay { get; set; }
        public double EquityPrevDay { get; set; }
        public string Status { get; set; }
        public MTRetCode MTRetCodeError { get; set; }

    }
}
