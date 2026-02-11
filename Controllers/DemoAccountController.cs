using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Utilities;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class DemoAccountController : ApiController
    {
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();
        public DemoAccountController()
        {

        }

        [HttpGet]
        public Mt5LiveAccountVM UserGetSingleDemoAccount(ulong LoginId)
        {
            return MT5AccountOperations.GetSingleAccount(_managerDemo, LoginId);
        }

        [HttpGet]
        public IEnumerable<Mt5LiveAccountVM> GetAllDemoAccount()
        {
            return MT5AccountOperations.GetAllAccounts(_managerDemo);
        }

        [HttpPost]
        public IEnumerable<Mt5AccountCreatedVM> CreateDemoAccount([FromBody] UserIdModel entity)
        {
            try
            {
                CIMTUser cIMTUser = _managerDemo.UserCreate();

                cIMTUser.Login((ulong)(entity.LoginId != 0 ? entity.LoginId : 0));
                cIMTUser.FirstName(entity.FirstName);
                cIMTUser.LastName(entity.LastName);
                cIMTUser.Leverage(entity.Leverage);
                cIMTUser.Group(entity.GroupName);
                cIMTUser.EMail(entity.EMail);
                cIMTUser.Phone(entity.Phone);
                cIMTUser.Address(entity.Address);
                cIMTUser.Country(entity.Country);

                string master_pass = PasswordGenerator.GenerateMasterPassword(11);
                string investor_pass = PasswordGenerator.GenerateInvestorPassword(9);

                cIMTUser.Rights((CIMTUser.EnUsersRights.USER_RIGHT_ENABLED |
                    CIMTUser.EnUsersRights.USER_RIGHT_OTP_ENABLED |
                    CIMTUser.EnUsersRights.USER_RIGHT_PASSWORD));

                MTRetCode mTRetCode = _managerDemo.UserAdd(cIMTUser, master_pass, investor_pass);

                ulong[] userLogins = _managerDemo.UserLogins(entity.GroupName, out MTRetCode res);

                if (MTRetCode.MT_RET_OK == mTRetCode)
                {
                    cIMTUser.Clear();

                    Mt5AccountCreatedVM userAccount = new Mt5AccountCreatedVM
                    {
                        UserId = entity.UserId,
                        Login = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()),
                        GroupName = entity.GroupName,
                        MasterPassword = master_pass,
                        InvestorPassword = investor_pass,
                        Leverage = entity.Leverage,
                        ServerName = "QuorionexMarketsTestOnly-Trade"
                    };

                    List<Mt5AccountCreatedVM> userAccounts = new List<Mt5AccountCreatedVM>();
                    userAccounts.Add(userAccount);

                    AccountLogHelper.LogSuccess(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                        entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, userLogins.Last(), master_pass, investor_pass, "Demo");

                    return userAccounts;
                }
                else
                {
                    AccountLogHelper.LogFailed(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                        entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, mTRetCode, master_pass, investor_pass, "Demo");
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
