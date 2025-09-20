using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5LiveOnlineUserActiveController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public BaseResponseModel<MT5LiveOnlineUserActiveVM> MT5LiveOnlineUserActiveDetail(string groupNames)
        {
            try
            {
                var mT5LiveOnlineUserActiveVM = new MT5LiveOnlineUserActiveVM();

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

                return new BaseResponseModel<MT5LiveOnlineUserActiveVM>
                {
                    Data = mT5LiveOnlineUserActiveVM,
                    Message = "Online and active/inactive user data retrieved successfully.",
                    Success = true,
                    MTRetErrorCode = MTRetCode.MT_RET_OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<MT5LiveOnlineUserActiveVM>
                {
                    Data = null,
                    Message = $"An error occurred: {ex.Message}",
                    Success = false,
                    MTRetErrorCode = MTRetCode.MT_RET_ERROR
                };
            }
        }


        //[HttpGet]
        //public BaseResponseModel<MT5LiveOnlineUserActiveVM> MT5LiveOnlineUserActiveDetail(string groupNames)
        //{
        //    try
        //    {
        //        MT5LiveOnlineUserActiveVM mT5LiveOnlineUserActiveVM = new MT5LiveOnlineUserActiveVM();

        //        uint onlineTotal = _manager.OnlineTotal();
        //        CIMTOnline onlineConnection = _manager.OnlineCreate();

        //        for (uint i = 0; i < onlineTotal; i++)
        //        {
        //            MTRetCode mTRetCode3 = _manager.OnlineNext(i, onlineConnection);
        //            if (mTRetCode3 == MTRetCode.MT_RET_OK)
        //            {
        //                mT5LiveOnlineUserActiveVM.OnlineTrader.Add(new UserLogin { Login = onlineConnection.Login() });
        //            }
        //        }

        //        // MT5 Active Accounts and Inactive Accounts
        //        CIMTUserArray cIMTUserArray = _manager.UserCreateArray();

        //        MTRetCode mTRetCode = _manager.UserGetByGroup(groupNames, cIMTUserArray);

        //        for (uint i = 0; i < cIMTUserArray.Total(); i++)
        //        {
        //            CIMTUser cIMTUser = cIMTUserArray.Next(i);

        //            //--- Check permission
        //            uint user_rights = (uint)cIMTUser.Rights();

        //            if ((user_rights & (uint)CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED) > 0)
        //            {
        //                // If the user is not enabled (inactive), increment the inactive accounts counter
        //                mT5LiveOnlineUserActiveVM.InActiveTrader.Add(new UserLogin { Login = cIMTUser.Login() });
        //            }
        //            else
        //            {
        //                // If the user is enabled (active), increment the active accounts counter
        //                mT5LiveOnlineUserActiveVM.ActiveTrader.Add(new UserLogin { Login = cIMTUser.Login() });
        //            }
        //        }


        //        //Release User Array: After processing the user array, release it to free up memory.
        //        cIMTUserArray.Release();

        //        //Release Online Connection: Ensure the online connection is released once it's no longer needed.
        //        onlineConnection.Release();

        //        return new BaseResponseModel<MT5LiveOnlineUserActiveVM>
        //        {
        //            Data = mT5LiveOnlineUserActiveVM,
        //            Message = "online or active inactive data retrieved successfully.",
        //            Success = true,
        //            MTRetErrorCode = MTRetCode.MT_RET_OK
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        return new BaseResponseModel<MT5LiveOnlineUserActiveVM>
        //        {
        //            Data = null,
        //            Message = $"An error occurred: {ex.Message}",
        //            Success = false,
        //            MTRetErrorCode = MTRetCode.MT_RET_ERROR
        //        };
        //    }
        //}
    }
}
