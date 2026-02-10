using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoMT5Controller : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();
        public DemoMT5Controller()
        {

        }

        [HttpGet]
        public MtGetAllLiveAccountVM UserGetSingleDemoAccount(ulong LoginId)
        {
            return MT5AccountOperations.GetSingleAccount(_managerDemo, LoginId);
        }

        [HttpGet]
        public IEnumerable<MtGetAllLiveAccountVM> GetAllDemoAccount()
        {
            return MT5AccountOperations.GetAllAccounts(_managerDemo);
        }

        [HttpPost]
        public IEnumerable<MtfiveaccountVM> CreateDemoAccount([FromBody] UserIdModel entity)
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

                string master_pass = GenerateRandomPass.GenerateMasterPassword(11);
                string investor_pass = GenerateRandomPass.GenerateInvestorPassword(9);

                cIMTUser.Rights((CIMTUser.EnUsersRights.USER_RIGHT_ENABLED |
                    CIMTUser.EnUsersRights.USER_RIGHT_OTP_ENABLED |
                    CIMTUser.EnUsersRights.USER_RIGHT_PASSWORD));

                MTRetCode mTRetCode = _managerDemo.UserAdd(cIMTUser, master_pass, investor_pass);

                ulong[] userLogins = _managerDemo.UserLogins(entity.GroupName, out MTRetCode res);

                if (MTRetCode.MT_RET_OK == mTRetCode)
                {
                    cIMTUser.Clear();

                    MtfiveaccountVM userAccount = new MtfiveaccountVM
                    {
                        UserId = entity.UserId,
                        Login = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()),
                        GroupName = entity.GroupName,
                        master_pass = master_pass,
                        investor_pass = investor_pass,
                        Leverage = entity.Leverage,
                        ServerName = "QuorionexMarketsTestOnly-Trade"
                    };

                    List<MtfiveaccountVM> userAccounts = new List<MtfiveaccountVM>();
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
