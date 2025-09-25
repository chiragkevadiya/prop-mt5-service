using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.ViewModels.ChallengeSettlement
{
    public class ChallengeSettlementResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public decimal Profit { get; set; }
        public decimal UserShare { get; set; }
        public decimal AdminShare { get; set; }
    }
}
