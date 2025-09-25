using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Models
{
    public class PropAccountMaster : BaseEntity
    {
        public long UserId { get; set; }

        [Required]
        public TradingAccountType TradingAccountType { get; set; }

        [Required]
        public RequestStatus ActionStatus { get; set; }
        public long AccountTypeId { get; set; }
        public long GroupId { get; set; }
        public long TerminalID { get; set; }
        public string Password { get; set; }
        public string InvestorPassword { get; set; }
        public string GroupName { get; set; }
        public int Leverage { get; set; }
        public string FullName { get; set; }
        //public long ClientId { get; set; }
        public bool DemoAccount { get; set; }
        public bool Locked { get; set; }
        public bool DontLiquidate { get; set; }
        public int AccountType { get; set; }
        public bool MarginAccount { get; set; }
        public bool Liquidated { get; set; }
        //public string? ParentClientDescription { get; set; }
        public double FreeMargin { get; set; }
        public bool TradeStopped { get; set; }
        public double MarginLevel { get; set; }
        public double UsedMargin { get; set; }
        //public double FloatingProfitLoss { get; set; }
        public double Equity { get; set; }
        public double Credit { get; set; }
        public double Balance { get; set; }
        //public double TotalInterest { get; set; }
        public double TotalCommission { get; set; }
        public double NetProfit { get; set; }
        public double PreviousMargin { get; set; }
        public double TotalDeposit { get; set; }
        public bool LockLiquidate { get; set; }

        //public string? MailToParentId { get; set; }
        public string Username { get; set; }
        //public int ClientType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        //public int InterestCalcType { get; set; }
        public string CRMComment { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActionDate { get; set; }
        public long ActionBy { get; set; }
        //public bool FTDVerify { get; set; } = false;
    }
}


