using System;

namespace NaptunePropTrading_Service.ViewModels
{
    public class AccountDetailVM
    {
        public ulong Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Group { get; set; }
        public string Country { get; set; }
        public double Balance { get; set; }
        public uint Leverage { get; set; }
        public double Margin { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
        public double Credit { get; set; }
        public double Equity { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
