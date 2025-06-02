using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using NaptunePropTrading_Service.Helper;
using NaptunePropTrading_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NaptunePropTrading_Service.Controllers
{
    public class MT5CreateAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public MT5CreateAccountController()
        {

        }

        [HttpPost]
        public BaseResponse<MT5CreateAccountResponses> CreateLiveAccount([FromBody] MT5CreateAccountVM entity)
        {
            try
            {

                if (entity.DepositAmount <= 0)
                {
                    return new BaseResponse<MT5CreateAccountResponses>
                    {
                        Success = false,
                        Message = $"Deposit amount must be greater than zero.",
                        Data = null
                    };
                }

                // Create an empty user object
                CIMTUser cIMTUser = _manager.UserCreate();
                cIMTUser.Login((ulong)(entity.LoginId != 0 ? entity.LoginId : 0)); // 0 default value
                cIMTUser.FirstName(entity.FirstName);
                cIMTUser.LastName(entity.LastName);
                cIMTUser.Leverage(entity.Leverage);
                cIMTUser.Group(entity.GroupName);
                cIMTUser.EMail(entity.Email);
                cIMTUser.Phone(entity.Phone);
                cIMTUser.Address(entity.Address);
                cIMTUser.Country(entity.Country);

                // Generate passwords
                string master_pass = GenerateRandomPass.GenerateMasterPassword(11);
                string investor_pass = GenerateRandomPass.GenerateInvestorPassword(9);

                // Set user rights
                cIMTUser.Rights((CIMTUser.EnUsersRights.USER_RIGHT_ENABLED |
                                   CIMTUser.EnUsersRights.USER_RIGHT_OTP_ENABLED | CIMTUser.EnUsersRights.USER_RIGHT_ALL |
                                   CIMTUser.EnUsersRights.USER_RIGHT_PASSWORD));

                // Attempt to add the user
                MTRetCode mTRetCode = _manager.UserAdd(cIMTUser, master_pass, investor_pass);
                ulong[] userLogins = _manager.UserLogins(entity.GroupName, out MTRetCode res);

                if (mTRetCode == MTRetCode.MT_RET_OK && userLogins.Any())
                {

                    // MT5 Deposit Balance
                    MTRetCode mTRetCode_Deposit = MT5DepositBalance((ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()), (double)entity.DepositAmount);

                    // Create a response object
                    MT5CreateAccountResponses userAccount = new MT5CreateAccountResponses
                    {
                        UserId = entity.UserId,
                        LoginId = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()),
                        GroupName = entity.GroupName,
                        MasterPassword = master_pass,
                        InvestorPassword = investor_pass,
                        Leverage = entity.Leverage,
                        DepositAmount = entity.DepositAmount,
                        DepositMessage = mTRetCode_Deposit == MTRetCode.MT_RET_OK ? "Deposit Success" : "Deposit Failed",
                        DepositStatus = mTRetCode_Deposit == MTRetCode.MT_RET_OK ? true : false,
                        ServerName = "Prop Trading Live"
                    };

                    // Log success
                    LogManager.LogAccountCreation(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                        entity.LastName, entity.Email, entity.Phone, entity.Address, entity.Country, (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()), master_pass, investor_pass);

                    return new BaseResponse<MT5CreateAccountResponses>
                    {
                        Success = true,
                        Message = "Account created successfully.",
                        Data = userAccount
                    };
                }
                else
                {
                    // Log failure
                    LogManager.LogAccountFailure(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                        entity.LastName, entity.Email, entity.Phone, entity.Address, entity.Country, mTRetCode, master_pass, investor_pass);

                    return new BaseResponse<MT5CreateAccountResponses>
                    {
                        Success = false,
                        Message = $"Account creation failed. Error Code: {mTRetCode}",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError("Error in CreateLiveAccount", ex.ToString());
                throw;
            }
        }


        // private method MT5 Deposit Balance
        private MTRetCode MT5DepositBalance(ulong login, double amount)
        {
            try
            {
                if (amount <= 0)
                {
                    LogManager.LogError("Deposit Failed", $"Invalid deposit amount: {amount} for Login ID: {login}");
                    return MTRetCode.MT_RET_ERROR; // Invalid deposit amount
                }

                // Perform deposit operation (DEAL_BALANCE = 2)
                ulong transactionId;
                MTRetCode mTRetCode = _manager.DealerBalanceRaw(login, amount, 2, "Deposit", out transactionId);

                if (mTRetCode == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    LogManager.LogSuccess_Deposit("Deposit Successful",
                        $"Deposit of {amount} for Login ID: {login} completed successfully. Transaction ID: {transactionId}");
                    return MTRetCode.MT_RET_OK;
                }
                else
                {
                    LogManager.LogError_Deposit("Deposit Failed",
                        $"Deposit of {amount} for Login ID: {login} failed. Error Code: {mTRetCode}");
                    return MTRetCode.MT_RET_ERR_NOTFOUND;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError("Error in UserDepositBalance", ex.ToString());
                return MTRetCode.MT_RET_ERROR;
            }
        }

        [HttpGet]
        public IEnumerable<AccountDetailVM> GetAllAccount(string Email)
        {
            try
            {
                List<AccountDetailVM> AccountDetailsVM = new List<AccountDetailVM>();

                CIMTUserArray cIMTUserArray = _manager.UserCreateArray();

                MTRetCode mTRetCode1 = _manager.UserGetByGroup("*", cIMTUserArray);

                if (MTRetCode.MT_RET_OK == mTRetCode1)
                {
                    cIMTUserArray.Total();

                    for (uint i = 0; i < cIMTUserArray.Total(); i++)
                    {
                        CIMTUser cIMTUser1 = cIMTUserArray.Next(i);

                        if (cIMTUser1.EMail().ToLower().Trim() == Email.ToLower().Trim())
                        {
                            AccountDetailVM AccountDetailsVM1 = new AccountDetailVM()
                            {
                                Login = cIMTUser1.Login(),
                                FirstName = cIMTUser1.FirstName(),
                                LastName = cIMTUser1.LastName(),
                                Group = cIMTUser1.Group(),
                                Country = cIMTUser1.Country(),
                                Credit = cIMTUser1.Credit(),
                                Balance = cIMTUser1.Balance(),
                                Leverage = cIMTUser1.Leverage(),
                                Status = cIMTUser1.Status(),
                                CreatedDate = DateTimeOffset.FromUnixTimeSeconds(cIMTUser1.Registration()).DateTime,
                            };
                            AccountDetailsVM.Add(AccountDetailsVM1);
                        }
                    }

                    cIMTUserArray.Clear();
                    cIMTUserArray.Release();
                    return AccountDetailsVM;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public List<AccountDetailVM> GetLiveAccount(string LoginId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginId))
                    return new List<AccountDetailVM>();

                CIMTUser cIMTUser = _manager.UserCreate();
                List<AccountDetailVM> Data = new List<AccountDetailVM>();

                List<ulong> loginIds = LoginId?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToUInt64(x)).ToList();

                foreach (var item in loginIds)
                {
                    MTRetCode mTRetCode = _manager.UserGet(item, cIMTUser);

                    CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                    MTRetCode mTRetCode1 = _manager.UserAccountGet(item, cIMTAccountInfo);

                    if (MTRetCode.MT_RET_OK == mTRetCode)
                    {
                        AccountDetailVM liveAccount = new AccountDetailVM();
                        liveAccount.Login = cIMTUser.Login();
                        liveAccount.FirstName = cIMTUser.FirstName();
                        liveAccount.LastName = cIMTUser.LastName();
                        liveAccount.Group = cIMTUser.Group();
                        liveAccount.Country = cIMTUser.Country();
                        liveAccount.BalancePrevDay = cIMTUser.BalancePrevDay();
                        liveAccount.EquityPrevDay = cIMTUser.EquityPrevDay();
                        liveAccount.Credit = cIMTAccountInfo.Credit();
                        liveAccount.Balance = cIMTAccountInfo.Balance();
                        liveAccount.Leverage = cIMTUser.Leverage();
                        liveAccount.Status = cIMTUser.Status();
                        liveAccount.Margin = cIMTAccountInfo.Margin();
                        liveAccount.MarginFree = cIMTAccountInfo.MarginFree();
                        liveAccount.Profit = cIMTAccountInfo.Profit();
                        liveAccount.Commission = 0;
                        liveAccount.Equity = cIMTAccountInfo.Equity();
                        liveAccount.CreatedDate = DateTimeOffset.FromUnixTimeSeconds(cIMTUser.Registration()).DateTime;

                        Data.Add(liveAccount);
                    }
                    else
                    {
                        continue;
                    }
                }
                cIMTUser.Clear();
                cIMTUser.Release();

                return Data;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
