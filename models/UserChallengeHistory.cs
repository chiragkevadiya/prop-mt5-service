using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Models
{
    public class UserChallengeHistory : BaseGUID
    {
        public long UserId { get; set; }

        // Account Type
        public long AccountTypeId { get; set; }
        public long BalanceId { get; set; }
        public string AccountTypeName { get; set; }
        public decimal AccountTypePrice { get; set; }

        // Balance Info
        public decimal PurchasedBalanceAmount { get; set; }
        public decimal PurchasedBalancePrice { get; set; }
        public int NoOfPhases { get; set; }

        // Summary
        public decimal TotalAmount { get; set; }
        public ChallengeStatus Status { get; set; } = ChallengeStatus.Pending;
        public RequestStatus PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string Comment { get; set; }
    }

}
