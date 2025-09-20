using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MetaQuotes.MT5CommonAPI.CIMTUser;

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
