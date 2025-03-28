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
    public class WithdrawalsController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public WithdrawalsController()
        {

        }

        [HttpPost]
        public BaseResponse<DWAccountResponseVM> WithdrawalAmount([FromBody] WithdrawalAccountVM entity)
        {
            try
            {
                if (entity.LoginId == 0 || entity.Amount <= 0)
                {
                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = false,
                        Message = entity.LoginId == 0
                            ? "LoginId is required. Please enter a valid LoginId."
                            : "Withdrawal amount must be greater than zero.",
                        Data = null
                    };
                }

                double balance = GetBalanceForLogin(entity.LoginId);

                if (balance <= 0 || balance < entity.Amount || entity.Amount <= 0)
                {

                    LogManager.LogError_Withdrawal("Withdrawal Failed", $"Error: {entity.LoginId} : Insufficient funds {entity.LoginId} Balance: {balance}, Requested: {entity.Amount}");

                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = false,
                        Message = "Insufficient balance for this withdrawal.",
                        Data = null
                    };
                }


                // Perform Withdrawal operation (DEAL_BALANCE = 2)
                ulong transactionId;
                MTRetCode mTRetCode = _manager.DealerBalanceRaw(entity.LoginId, -entity.Amount, 2, "Withdrawal", out transactionId);

                if (mTRetCode == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    LogManager.LogSuccess_Withdrawal("Withdrawal Successful",
                        $"Withdrawal of {entity.Amount} for Login ID: {entity.LoginId} completed successfully. Transaction ID: {transactionId}");

                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = true,
                        Message = $"Withdrawal successful! The amount has been deducted from your account.",
                        Data = new DWAccountResponseVM
                        {
                            Amount = entity.Amount,
                            LoginId = entity.LoginId,
                            TransactionId = transactionId,
                            TransactionDate = DateTime.UtcNow,
                        }
                    };
                }
                else
                {
                    LogManager.LogError_Withdrawal("Withdrawal Failed",
                        $"Withdrawal of {entity.Amount} for Login ID: {entity.LoginId} failed. Error Code: {mTRetCode}");

                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = false,
                        Message = $"Withdrawal of {entity.Amount} for Login ID: {entity.LoginId} failed. Error Code: {mTRetCode}.",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError("Error in WithdrawalAmount", ex.ToString());
                throw;
            }
        }

        // Get Balance For Login
        private double GetBalanceForLogin(ulong login)
        {
            CIMTUser user = _manager.UserCreate();
            try
            {
                if (_manager.UserGet(login, user) == MTRetCode.MT_RET_OK)
                {
                    return user.Balance();
                }
            }
            finally
            {
                user?.Release();
            }
            return 0;
        }
    }
}
