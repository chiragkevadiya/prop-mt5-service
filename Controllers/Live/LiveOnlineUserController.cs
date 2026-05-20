using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/liveonlineuser")]
    public class LiveOnlineUserController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpGet]
        [Route("")]
        public BaseResponseModel<Mt5OnlineUserVM> MT5LiveOnlineUserActiveDetail(string groupNames)
        {
            try
            {
                var mT5LiveOnlineUserActiveVM = new Mt5OnlineUserVM();

                // Retrieve and populate online users
                uint onlineTotal = _manager.OnlineTotal();
                CIMTOnline onlineConnection = _manager.OnlineCreate();
                List<ulong> OnlineTraderId = new List<ulong>();
                for (uint i = 0; i < onlineTotal; i++)
                {
                    if (_manager.OnlineNext(i, onlineConnection) == MTRetCode.MT_RET_OK)
                    {
                        if (onlineConnection.Login() != 1000)
                        {
                            OnlineTraderId.Add(onlineConnection.Login());
                        }
                        //mT5LiveOnlineUserActiveVM.OnlineTrader.Add(new UserLogin { Login = onlineConnection.Login() });
                    }
                }
                mT5LiveOnlineUserActiveVM.OnlineTrader = OnlineTraderId;

                // Retrieve and populate active and inactive users by group
                CIMTUserArray userArray = _manager.UserCreateArray();
                if (_manager.UserGetByGroup(groupNames, userArray) == MTRetCode.MT_RET_OK)
                {
                    List<ulong> ActiveTraderId = new List<ulong>();
                    List<ulong> InActiveTraderId = new List<ulong>();

                    for (uint i = 0; i < userArray.Total(); i++)
                    {
                        CIMTUser user = userArray.Next(i);
                        uint userRights = (uint)user.Rights();

                        // var userLogin = new UserLogin { Login = user.Login() };

                        if ((userRights & (uint)CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED) > 0)
                        {
                            //mT5LiveOnlineUserActiveVM.InActiveTrader.Add(userLogin);
                            InActiveTraderId.Add(user.Login());
                            //mT5LiveOnlineUserActiveVM.InActiveTrader = userLogin;
                        }
                        else
                        {
                            ActiveTraderId.Add(user.Login());
                            //mT5LiveOnlineUserActiveVM.ActiveTrader.Add(userLogin);
                        }
                    }
                    mT5LiveOnlineUserActiveVM.ActiveTrader = ActiveTraderId;
                    mT5LiveOnlineUserActiveVM.InActiveTrader = InActiveTraderId;
                }

                // Release resources
                userArray.Release();
                onlineConnection.Release();

                return new BaseResponseModel<Mt5OnlineUserVM>
                {
                    Data = mT5LiveOnlineUserActiveVM,
                    Message = "Online and active/inactive user data retrieved successfully.",
                    Success = true,
                    MTRetErrorCode = MTRetCode.MT_RET_OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<Mt5OnlineUserVM>
                {
                    Data = null,
                    Message = $"An error occurred: {ex.Message}",
                    Success = false,
                    MTRetErrorCode = MTRetCode.MT_RET_ERROR
                };
            }
        }
    }
}
