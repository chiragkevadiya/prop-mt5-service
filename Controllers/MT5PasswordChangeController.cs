using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using MT5ConnectionService.ViewModels.Password;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5PasswordChangeController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public MT5PasswordChangeController()
        {

        }

        [HttpGet]
        public UserPasswordChangeVM UserPasswordChange(ulong LoginId)
        {

            // Set master and investor passwords (replace with your logic)
            string master_pass = GenerateRandomPass.GenerateMasterPassword(11); // Replace with a valid master password
            string investor_pass = GenerateRandomPass.GenerateInvestorPassword(9); // Replace with a valid investor password

            // Password Change
            MTRetCode investor_pass_mTRetCode = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_INVESTOR, LoginId, investor_pass);
            MTRetCode master_pass_mTRetCode = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_MAIN, LoginId, master_pass);

            UserPasswordChangeVM userPasswordChangeVM = new UserPasswordChangeVM()
            {
                Login = LoginId,
                InvestorPassword = investor_pass,
                MasterPassword = master_pass,
                mTRetCode1 = investor_pass_mTRetCode,
                mTRetCode2 = master_pass_mTRetCode
            };

            return userPasswordChangeVM;

        }
    }
}
