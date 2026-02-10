using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Linq;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class UserAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public UserDetailsAccountVM UsersAccountGet(ulong LoginId)
        {
            try
            {
                ulong[] loginIds = { LoginId };
                CIMTUser user = _manager.UserCreate();
                if (user == null)
                    throw new Exception("Failed to create user object.");

                MTRetCode userCode = _manager.UserGet(LoginId, user);
                if (userCode != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    throw new Exception($"Failed to get user info: {userCode}");
                }

                DateTimeOffset endDate = DateTimeOffset.UtcNow.AddDays(1);

                long fromTimestamp = 0;
                long toTimestamp = endDate.ToUnixTimeSeconds();

                CIMTDealArray dealArray = _manager.DealCreateArray();
                if (dealArray == null)
                    throw new Exception("Failed to create deal array.");

                MTRetCode dealRequestCode = _manager.DealRequestByLogins(loginIds, fromTimestamp, toTimestamp, dealArray);
                if (dealRequestCode != MTRetCode.MT_RET_OK)
                    throw new Exception($"Deal request failed: {dealRequestCode}");

                var closedDeals = dealArray.ToArray()
                                           .Where(deal => deal.Entry() == 1)
                                           .ToList();

                double closedProfit = Math.Round(closedDeals.Sum(deal => deal.Profit()), 2);
                dealArray.Release();

                CIMTAccount account = _manager.UserCreateAccount();
                if (account == null)
                    throw new Exception("Failed to create account object.");

                MTRetCode accountCode = _manager.UserAccountGet(LoginId, account);
                if (accountCode != MTRetCode.MT_RET_OK)
                {
                    account.Release();
                    return null;
                }

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
                throw new Exception("Error occurred in UsersAccountGet: " + ex.Message, ex);
            }
        }

        [HttpPost]
        public MTRetCode UserDepositBalance([FromBody] MTFiveDepositBalanceVM entity)
        {
            return MT5AccountOperations.DepositOrWithdrawBalance(_manager, entity);
        }
    }
}
