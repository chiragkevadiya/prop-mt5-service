using MetaQuotes.MT5CommonAPI;

namespace MT5ConnectionService.ViewModels.Password
{
    public class UserPasswordChangeVM
    {
        public ulong Login { get; set; }
        public string MasterPassword { get; set; }
        public string InvestorPassword { get; set; }
        public MTRetCode mTRetCode1 { get; set; }
        public MTRetCode mTRetCode2 { get; set; }
    }

    public class UserMasterInvestorPasswordVM
    {
        public ulong Login { get; set; }
        public string MasterPassword { get; set; }
        public string InvestorPassword { get; set; }
    }

}
