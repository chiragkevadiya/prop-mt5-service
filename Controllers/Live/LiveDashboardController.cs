using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Linq;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    [RoutePrefix("api/livedashboard")]
    public class LiveDashboardController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();


        [HttpGet]
        [Route("mt5-dashboard-detail")]
        public BaseResponseModel<Mt5DashboardVM> MT5LiveDashboardDetail(string loginIds)
        {
            try
            {
                ulong[] loginId = Array.ConvertAll(loginIds.Split(','), ulong.Parse);
                int MT5ActiveAccounts = 0;
                int MT5InactiveAccounts = 0;
                double ActualProfitLoss = 0;
                uint onlineUser = 0;
                uint totalAccounts = 0;

                uint onlineTotal = _manager.OnlineTotal();
                CIMTOnline onlineConnection = _manager.OnlineCreate();

                // Count online users from the loginIds list
                for (uint i = 0; i < onlineTotal; i++)
                {
                    MTRetCode ret = _manager.OnlineNext(i, onlineConnection);
                    if (ret == MTRetCode.MT_RET_OK && loginId.Contains(onlineConnection.Login()))
                    {
                        onlineUser++;
                    }
                }

                foreach (var login in loginId)
                {
                    // Get user info
                    CIMTUser user = _manager.UserCreate();
                    MTRetCode userRet = _manager.UserGet(login, user);
                    if (userRet == MTRetCode.MT_RET_OK)
                    {
                        totalAccounts++;
                        uint user_rights = (uint)user.Rights();
                        if ((user_rights & (uint)CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED) > 0)
                            MT5InactiveAccounts++;
                        else
                            MT5ActiveAccounts++;
                    }
                    user.Release();

                }
                CIMTAccountArray cIMTAccountArray = _manager.UserCreateAccountArray();
                MTRetCode mTRetCode2 = _manager.UserAccountRequestArray("*", cIMTAccountArray);

                for (uint i = 0; i < cIMTAccountArray.Total(); i++)
                {
                    // Get the next account from the array
                    CIMTAccount account = cIMTAccountArray.Next(i);

                    // Get the profit of the current account
                    double profit = account.Profit();

                    // Update total profit excluding losses
                    ActualProfitLoss += profit;
                }

                //Release Account Array: Similarly, release the account array after processing.
                cIMTAccountArray.Release();

                //Release Online Connection: Ensure the online connection is released once it's no longer needed.
                onlineConnection.Release();

                var detailVM = new Mt5DashboardVM
                {
                    MT5OnlineUsers = onlineUser,
                    MT5TotalAccounts = totalAccounts,
                    MT5ActiveAccounts = MT5ActiveAccounts,
                    MT5InactiveAccounts = MT5InactiveAccounts,
                    MT5ActualProfitLoss = Math.Round(ActualProfitLoss, 2),
                };

                return new BaseResponseModel<Mt5DashboardVM>
                {
                    Data = detailVM,
                    Message = "Dashboard data retrieved successfully.",
                    Success = true,
                    MTRetErrorCode = MTRetCode.MT_RET_OK
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
