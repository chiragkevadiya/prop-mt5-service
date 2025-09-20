using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels.Password;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5LiveMasterInvestorPasswordChangeController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();


        [HttpGet]
        public BaseResponseModel<UserMasterInvestorPasswordVM> MT5LiveMasterInvestorPasswordChange(ulong LoginId, string newPassword, int pType)
        {

            try
            {
                // Set master and investor passwords (replace with your logic)


                if (pType == 0) // pType =0 master_password
                {
                    string master_password = newPassword; // Replace with a valid master password

                    // Master Password Change
                    MTRetCode master_pass_mTRetCode = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_MAIN, LoginId, master_password);

                    if (MTRetCode.MT_RET_USR_INVALID_PASSWORD == master_pass_mTRetCode)
                    {
                        return new BaseResponseModel<UserMasterInvestorPasswordVM>
                        {
                            Data = null,
                            Message = "Invalid trading password. Please try again.",
                            Success = false,
                            MTRetErrorCode = MTRetCode.MT_RET_USR_INVALID_PASSWORD
                        };
                    }

                    UserMasterInvestorPasswordVM userPasswordChangeVM = new UserMasterInvestorPasswordVM()
                    {
                        Login = LoginId,
                        InvestorPassword = null,
                        MasterPassword = master_password,
                    };

                    return new BaseResponseModel<UserMasterInvestorPasswordVM>
                    {
                        Data = userPasswordChangeVM,
                        Message = "Trading password changed successfully.",
                        Success = true,
                        MTRetErrorCode = MTRetCode.MT_RET_OK
                    };

                }
                else if (pType == 1) // pType =1 investor_password
                {
                    string investor_password = newPassword; // Replace with a valid investor password

                    // Investor Password Change
                    MTRetCode investor_pass_mTRetCode = _manager.UserPasswordChange(CIMTUser.EnUsersPasswords.USER_PASS_INVESTOR, LoginId, investor_password);

                    if (MTRetCode.MT_RET_USR_INVALID_PASSWORD == investor_pass_mTRetCode)
                    {
                        return new BaseResponseModel<UserMasterInvestorPasswordVM>
                        {
                            Data = null,
                            Message = "Invalid investor password. Please try again.",
                            Success = false,
                            MTRetErrorCode = MTRetCode.MT_RET_USR_INVALID_PASSWORD
                        };
                    }

                    UserMasterInvestorPasswordVM userPasswordChangeVM = new UserMasterInvestorPasswordVM()
                    {
                        Login = LoginId,
                        InvestorPassword = investor_password,
                        MasterPassword = null,
                    };

                    return new BaseResponseModel<UserMasterInvestorPasswordVM>
                    {
                        Data = userPasswordChangeVM,
                        Message = "Investor password changed successfully.",
                        Success = true,
                        MTRetErrorCode = MTRetCode.MT_RET_OK
                    };

                }
                else
                {
                    return new BaseResponseModel<UserMasterInvestorPasswordVM>
                    {
                        Data = null,
                        Message = "Unknown password type",
                        Success = false,
                        MTRetErrorCode = MTRetCode.MT_RET_USR_INVALID_PASSWORD
                    };
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
