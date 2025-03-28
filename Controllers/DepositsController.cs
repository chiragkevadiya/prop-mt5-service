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
    public class DepositsController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public DepositsController()
        {

        }


        [HttpPost]
        public BaseResponse<DWAccountResponseVM> DepositAmount([FromBody] DepositAccountVM entity)
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
                            : "Deposit amount must be greater than zero.",
                        Data = null
                    };
                }


                // Perform deposit operation (DEAL_BALANCE = 2)
                ulong transactionId;
                MTRetCode mTRetCode = _manager.DealerBalanceRaw(entity.LoginId, entity.Amount, 2, "Deposit", out transactionId);

                if (mTRetCode == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    LogManager.LogSuccess_Deposit("Deposit Successful",
                        $"Deposit of {entity.Amount} for Login ID: {entity.LoginId} completed successfully. Transaction ID: {transactionId}");

                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = true,
                        Message = $"Deposit successful! Your funds have been added to your account.",
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
                    LogManager.LogError_Deposit("Deposit Failed",
                        $"Deposit of {entity.Amount} for Login ID: {entity.LoginId} failed. Error Code: {mTRetCode}");

                    return new BaseResponse<DWAccountResponseVM>
                    {
                        Success = false,
                        Message = $"Deposit of {entity.Amount} for Login ID: {entity.LoginId} failed. Error Code: {mTRetCode}.",
                        Data = null
                    };
                }


            }
            catch (Exception ex)
            {
                LogManager.LogError("Error in DepositAmount", ex.ToString());
                throw;
            }
        }

    }
}
