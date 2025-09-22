using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace MT5ConnectionService.Controllers
{
    public class UserAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public UserAccountController()
        {

        }

        [HttpGet]
        public UserDetailsAccountVM UsersAccountGet(ulong LoginId)
        {
            try
            {
                ulong[] loginIds = { LoginId };
                // Step 1: Create User object
                CIMTUser user = _manager.UserCreate();
                if (user == null)
                    throw new Exception("Failed to create user object.");

                // Step 2: Get user info
                MTRetCode userCode = _manager.UserGet(LoginId, user);
                if (userCode != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    throw new Exception($"Failed to get user info: {userCode}");
                }

                //DateTimeOffset startDate = DateTimeOffset.FromUnixTimeSeconds(user.Registration());
                DateTimeOffset endDate = DateTimeOffset.UtcNow.AddDays(1);

                long fromTimestamp = 0;
                long toTimestamp = endDate.ToUnixTimeSeconds();

                // Fetch historical deals
                CIMTDealArray dealArray = _manager.DealCreateArray();
                if (dealArray == null)
                    throw new Exception("Failed to create deal array.");

                MTRetCode dealRequestCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);
                if (dealRequestCode != MTRetCode.MT_RET_OK)
                    throw new Exception($"Deal request failed: {dealRequestCode}");

                var closedDeals = dealArray.ToArray()
                                           .Where(deal => deal.Entry() == 1) // DEAL_ENTRY_OUT (Closed deals)
                                           .ToList();

                double closedProfit = Math.Round(closedDeals.Sum(deal => deal.Profit()), 2);
                dealArray.Release();

                // Fetch live account information
                CIMTAccount account = _manager.UserCreateAccount();
                if (account == null)
                    throw new Exception("Failed to create account object.");

                MTRetCode accountCode = _manager.UserAccountGet(LoginId, account);
                if (accountCode != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return null;
                }

                // Combine live and historical profit
                double totalProfit = account.Profit() + closedProfit;

                var userDetails = new UserDetailsAccountVM
                {
                    Balance = account.Balance(),
                    Credit = account.Credit(),
                    Equity = account.Equity(),
                    Margin = account.Margin(),
                    MarginFree = account.MarginFree(),
                    Profit = Math.Round(totalProfit, 2),
                    TotalCloseProfit = closedProfit
                };

                account.Release();
                return userDetails;
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message or use ILogger
                throw new Exception("Error occurred in UsersAccountGet: " + ex.Message, ex);
            }
        }


        [HttpPost]
        public MTRetCode UserDepositBalance([FromBody] MTFiveDepositBalanceVM entity)
        {
            try
            {
                // (DEAL_CREDIT 3) =>  Use This Id 3 Credit operation.
                // (DEAL_BALANCE 2) =>  Use This Id 2 A balance operation.

                MTRetCode mTRetCode;
                ulong variable = 0;

                if (entity.Comment == "Withdraw")
                {
                    var balance = GetBalanceForLogin(entity.Login);

                    if (balance < 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }
                    if (entity.Amount <= 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    if (balance == 0)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    if (balance < entity.Amount)
                    {
                        return MTRetCode.MT_RET_ERROR;
                    }

                    mTRetCode = _manager.DealerBalanceRaw(entity.Login, -entity.Amount, 2, entity.Comment, out variable);
                }
                else
                {
                    mTRetCode = _manager.DealerBalanceRaw(entity.Login, entity.Amount, 2, entity.Comment, out variable);
                }

                if (MTRetCode.MT_RET_REQUEST_DONE == mTRetCode)
                {
                    return mTRetCode;
                }
                else
                {
                    return MTRetCode.MT_RET_ERR_NOTFOUND;
                }


                #region old code
                //MTRetCode mTRetCode;
                //ulong variable = 0;

                //if (entity.Comment == "Withdraw")
                //{
                //    mTRetCode = _manager.DealerBalanceRaw(entity.Login, -entity.Amount, 2, entity.Comment, out variable);
                //}
                //else
                //{
                //    mTRetCode = _manager.DealerBalanceRaw(entity.Login, entity.Amount, 2, entity.Comment, out variable);
                //}


                //if (MTRetCode.MT_RET_REQUEST_DONE == mTRetCode)
                //{
                //    return mTRetCode;
                //}
                //else
                //{
                //    return mTRetCode;
                //}
                #endregion

            }
            catch (Exception)
            {

                throw;
            }
        }

        private double GetBalanceForLogin(ulong login)
        {
            CIMTUser cIMTUserc = _manager.UserCreate();
            MTRetCode mTRetCode1 = _manager.UserGet(login, cIMTUserc);

            if (MTRetCode.MT_RET_OK == mTRetCode1)
            {
                return cIMTUserc.Balance();
                
            }

            return 0;
        }
        
        [HttpPost]
        public MTRetCode DisableUserAndTrading(ulong loginId)
        {
            try
            {
                // Step 1: Create user object
                CIMTUser user = _manager.UserCreate();
                if (user == null)
                    throw new Exception("Failed to create user object.");

                // Step 2: Fetch user info
                MTRetCode ret = _manager.UserGet(loginId, user);
                if (ret != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    return ret;
                }

                // Step 3: Get current rights
                CIMTUser.EnUsersRights rights = user.Rights();

                // Step 4: Disable trading and disable account
                rights |= CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED; // block trading
                rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_ENABLED;       // block login

                // Step 5: Update rights
                user.Rights(rights);

                // Step 6: Push changes back to MT5
                ret = _manager.UserUpdate(user);

                // Step 7: Cleanup
                user.Release();

                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Error disabling user + trading: " + ex.Message, ex);
            }
        }





    }

}
