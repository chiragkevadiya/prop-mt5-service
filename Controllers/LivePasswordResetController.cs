using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Utilities;
using PropMT5ConnectionService.ViewModels.Password;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class LivePasswordResetController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        public LivePasswordResetController()
        {

        }

        [HttpGet]
        public UserPasswordChangeVM UserPasswordChange(ulong LoginId)
        {

            // Set master and investor passwords (replace with your logic)
            string master_pass = PasswordGenerator.GenerateMasterPassword(11); // Replace with a valid master password
            string investor_pass = PasswordGenerator.GenerateInvestorPassword(9); // Replace with a valid investor password

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
