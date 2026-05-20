using System;

namespace PropMT5Service.ViewModels
{
    public class Mt5AccountCreatedVM
    {
        public Guid UserId { get; set; }
        public ulong Login { get; set; }
        public string GroupName { get; set; }
        public string MasterPassword { get; set; }
        public string InvestorPassword { get; set; }
        public uint Leverage { get; set; }
        public string ServerName { get; set; }
    }
}
