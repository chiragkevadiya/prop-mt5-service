using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.ViewModels
{
    public class DepositAccountVM
    {
        [Required]
        public ulong LoginId { get; set; }
        [Required]
        public double Amount { get; set; }
    }

    public class WithdrawalAccountVM
    {
        [Required]
        public ulong LoginId { get; set; }
        [Required]
        public double Amount { get; set; }
    }

    public class DWAccountResponseVM
    {
        public ulong LoginId { get; set; }
        public double Amount { get; set; }
        public ulong TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
