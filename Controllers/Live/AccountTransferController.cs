using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/accounttransfer")]
    public class AccountTransferController : BaseApiController
    {
        public AccountTransferController(CIMTManagerAPI manager) : base(manager) { }

        [HttpPost]
        [Route("")]
        public IHttpActionResult TransferTerminalToTerminal([FromBody] TransferTerminalToTerminalVM entity)
        {
            if (entity == null)
                return BadRequestResponse<TransferTerminalToTerminalResponse>(MT5Constants.ResponseMessages.ModelCannotBeNull);

            if (entity.Amount <= 0)
                return BadRequestResponse<TransferTerminalToTerminalResponse>("Transfer amount must be greater than zero.");

            return ExecuteSafe<TransferTerminalToTerminalResponse>(() =>
            {
                // Verify sender exists
                MTRetCode senderCheck = MTRetCode.MT_RET_OK;
                WithUser(entity.From, (user, ret) => { senderCheck = ret; return 0; });

                if (senderCheck == MTRetCode.MT_RET_ERR_NOTFOUND)
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        $"Sender account {entity.From} was not found.", 404);

                if (!IsSuccessful(senderCheck))
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        GetMT5ErrorMessage(senderCheck), 400);

                // Verify receiver exists
                MTRetCode receiverCheck = MTRetCode.MT_RET_OK;
                WithUser(entity.To, (user, ret) => { receiverCheck = ret; return 0; });

                if (receiverCheck == MTRetCode.MT_RET_ERR_NOTFOUND)
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        $"Receiver account {entity.To} was not found.", 404);

                if (!IsSuccessful(receiverCheck))
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        GetMT5ErrorMessage(receiverCheck), 400);

                // Check sender balance
                double balance = MT5AccountOperations.GetBalanceForLogin(_manager, entity.From);
                if (balance <= 0 || balance < entity.Amount)
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        $"Insufficient funds in account {entity.From}. Available: {balance:F2}, Requested: {entity.Amount:F2}.", 400);

                // Withdraw from sender
                MTRetCode withdrawalResult = _manager.DealerBalanceRaw(
                    entity.From, -entity.Amount,
                    MT5Constants.AccountOperations.DealerBalanceType,
                    "Fund Withdrawal", out _);

                if (withdrawalResult != MTRetCode.MT_RET_REQUEST_DONE)
                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        $"Withdrawal failed: {GetMT5ErrorMessage(withdrawalResult)}", 400);

                // Deposit to receiver — revert on failure
                MTRetCode depositResult = _manager.DealerBalanceRaw(
                    entity.To, entity.Amount,
                    MT5Constants.AccountOperations.DealerBalanceType,
                    "Fund Received", out _);

                if (depositResult != MTRetCode.MT_RET_REQUEST_DONE)
                {
                    _manager.DealerBalanceRaw(
                        entity.From, entity.Amount,
                        MT5Constants.AccountOperations.DealerBalanceType,
                        "Fund Reversal", out _);

                    return new BaseResponse<TransferTerminalToTerminalResponse>().WithError(
                        "Deposit failed. Withdrawal has been reversed.", 400);
                }

                return new BaseResponse<TransferTerminalToTerminalResponse>().WithSuccess(
                    new TransferTerminalToTerminalResponse
                    {
                        Success = true,
                        SenderLoginId = entity.From,
                        ReceiveLoginId = entity.To,
                        TransferAmount = entity.Amount,
                        mTRetCode = MTRetCode.MT_RET_REQUEST_DONE.ToString()
                    },
                    "Transfer completed successfully.");
            });
        }
    }
}
