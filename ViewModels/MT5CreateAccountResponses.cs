using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.ViewModels
{
    public class MT5CreateAccountResponses
    {
        public Guid UserId { get; set; }
        public ulong LoginId { get; set; }
        public string GroupName { get; set; }
        public string MasterPassword { get; set; }
        public string InvestorPassword { get; set; }
        public uint Leverage { get; set; }
        public string ServerName { get; set; }
        public double? DepositAmount { get; set; }
        public bool? DepositStatus { get; set; }
        public string DepositMessage { get; set; }
    }
}
