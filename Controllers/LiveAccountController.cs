using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Utilities;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;


namespace MT5ConnectionService
{


    [RoutePrefix("api/mt5")]
    public class LiveAccountController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();
        public LiveAccountController()
        {

        }

        [HttpGet]
        [Route("account/{loginId:long}")]
        public Mt5LiveAccountVM UserGetSingleLiveAccount(long LoginId)
        {
            return MT5AccountOperations.GetSingleAccount(_manager, (ulong)LoginId);
        }

        [HttpGet]
        [Route("accounts")]
        public IEnumerable<Mt5LiveAccountVM> GetAllLiveAccount()
        {
            return MT5AccountOperations.GetAllAccounts(_manager);
        }

        [HttpPost]
        [Route("account/create")]
        public IEnumerable<Mt5AccountCreatedVM> CreateLiveAccount([FromBody] UserIdModel entity)
        {
            try
            {
                bool maxRetry = true;
                int attemptCount = 0;

                while (maxRetry)
                {
                    var prefix = "555";
                    Random random = new Random();
                    string randomPart = random.Next(0, 1000).ToString("D3");
                    string loginString = prefix + randomPart;
                    entity.LoginId = ulong.Parse(loginString);

                    CIMTUser cIMTUser = _manager.UserCreate();

                    cIMTUser.Login((ulong)entity.LoginId);
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

                    MTRetCode mTRetCode = _manager.UserAdd(cIMTUser, master_pass, investor_pass);

                    ulong[] userLogins = _manager.UserLogins(entity.GroupName, out MTRetCode res);

                    if (mTRetCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                    {
                        attemptCount++;
                        if (attemptCount < 100)
                        {
                            continue;
                        }
                    }

                    if (MTRetCode.MT_RET_OK == mTRetCode)
                    {
                        maxRetry = false;

                        Mt5AccountCreatedVM userAccount = new Mt5AccountCreatedVM
                        {
                            UserId = entity.UserId,
                            Login = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()),
                            GroupName = entity.GroupName,
                            MasterPassword = master_pass,
                            InvestorPassword = investor_pass,
                            Leverage = entity.Leverage,
                            ServerName = "PropTradingMT5"
                        };

                        List<Mt5AccountCreatedVM> userAccounts = new List<Mt5AccountCreatedVM>();
                        userAccounts.Add(userAccount);

                        AccountLogHelper.LogSuccess(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                            entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, userLogins.Last(), master_pass, investor_pass);

                        return userAccounts;
                    }
                    else
                    {
                        AccountLogHelper.LogFailed(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                            entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, mTRetCode, master_pass, investor_pass);
                        return null;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
