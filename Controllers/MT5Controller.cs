using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
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
    public class MT5Controller : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public MT5Controller()
        {

        }

        [HttpGet]
        [Route("account/{loginId:long}")]
        public MtGetAllLiveAccountVM UserGetSingleLiveAccount(long LoginId)
        {
            return MT5AccountOperations.GetSingleAccount(_manager, (ulong)LoginId);
        }

        [HttpGet]
        [Route("accounts")]
        public IEnumerable<MtGetAllLiveAccountVM> GetAllLiveAccount()
        {
            return MT5AccountOperations.GetAllAccounts(_manager);
        }

        [HttpPost]
        [Route("account/create")]
        public IEnumerable<MtfiveaccountVM> CreateLiveAccount([FromBody] UserIdModel entity)
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

                    string master_pass = GenerateRandomPass.GenerateMasterPassword(11);
                    string investor_pass = GenerateRandomPass.GenerateInvestorPassword(9);

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

                        MtfiveaccountVM userAccount = new MtfiveaccountVM
                        {
                            UserId = entity.UserId,
                            Login = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()),
                            GroupName = entity.GroupName,
                            master_pass = master_pass,
                            investor_pass = investor_pass,
                            Leverage = entity.Leverage,
                            ServerName = "PropTradingMT5"
                        };

                        List<MtfiveaccountVM> userAccounts = new List<MtfiveaccountVM>();
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
