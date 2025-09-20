using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class TransferTerminalToTerminalController : ApiController
    {
        private readonly CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public TransferTerminalToTerminalController()
        {

        }

        [HttpPost]
        public BaseResponseModel<TransferTerminalToTerminalResponse> TransferTerminalToTerminal([FromBody] TransferTerminalToTerminalVM entity)
        {
            try
            {
                if (_manager == null)
                {
                    return new BaseResponseModel<TransferTerminalToTerminalResponse>
                    {
                        Success = false,
                        Message = "Manager instance is not initialized."
                    };
                }

                // Sender
                CIMTUser sender = _manager.UserCreate();
                MTRetCode senderUserStatus = _manager.UserGet(entity.From, sender);

                if (senderUserStatus != MTRetCode.MT_RET_OK)
                {
                    return new BaseResponseModel<TransferTerminalToTerminalResponse>
                    {
                        Success = true,
                        Message = $"Sender account {entity.From} was not found.",
                        Data = new TransferTerminalToTerminalResponse
                        {
                            Success = false,
                            SenderLoginId = entity.From,
                            ReceiveLoginId = entity.To,
                            TransferAmount = entity.Amount,
                            ErrorMessage = $"Sender account {entity.From} was not found.",
                            mTRetCode = MTRetCode.MT_RET_ERR_NOTFOUND.ToString(),
                        }
                    };
                }

                // Receiver
                CIMTUser receiver = _manager.UserCreate();
                MTRetCode receiverUserStatus = _manager.UserGet(entity.To, receiver);

                if (receiverUserStatus != MTRetCode.MT_RET_OK)
                {
                    return new BaseResponseModel<TransferTerminalToTerminalResponse>
                    {
                        Success = true,
                        Message = $"Receiver account {entity.To} was not found.",
                        Data = new TransferTerminalToTerminalResponse
                        {
                            Success = false,
                            SenderLoginId = entity.From,
                            ReceiveLoginId = entity.To,
                            TransferAmount = entity.Amount,
                            ErrorMessage = $"Receiver account {entity.To} was not found.",
                            mTRetCode = MTRetCode.MT_RET_ERR_NOTFOUND.ToString(),
                        }
                    };
                }

                // Balance check
                double senderAccountBalance = GetBalanceForLogin(entity.From);
                if (senderAccountBalance <= 0 || senderAccountBalance < entity.Amount || entity.Amount <= 0)
                {
                    return new BaseResponseModel<TransferTerminalToTerminalResponse>
                    {
                        Success = true,
                        Message = $"Insufficient funds in account {entity.From}. Available balance: {senderAccountBalance}, Requested: {entity.Amount}.",
                        Data = new TransferTerminalToTerminalResponse
                        {
                            Success = false,
                            SenderLoginId = entity.From,
                            ReceiveLoginId = entity.To,
                            TransferAmount = entity.Amount,
                            ErrorMessage = $"Insufficient funds in account {entity.From}.",
                            mTRetCode = MTRetCode.MT_RET_REQUEST_NO_MONEY.ToString(),
                        }
                    };
                }

                // Perform transfer
                ulong withdrawalTransactionId, depositTransactionId;
                MTRetCode withdrawalResult = _manager.DealerBalanceRaw(entity.From, -entity.Amount, 2, "Fund Withdrawal", out withdrawalTransactionId);

                if (withdrawalResult == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    MTRetCode depositResult = _manager.DealerBalanceRaw(entity.To, entity.Amount, 2, "Fund Received", out depositTransactionId);

                    if (depositResult != MTRetCode.MT_RET_REQUEST_DONE)
                    {
                        // Revert withdrawal
                        _manager.DealerBalanceRaw(entity.From, entity.Amount, 2, "Fund Reversal", out _);

                        return new BaseResponseModel<TransferTerminalToTerminalResponse>
                        {
                            Success = true,
                            Message = "Deposit failed. Withdrawal has been reversed.",
                            Data = new TransferTerminalToTerminalResponse
                            {
                                Success = false,
                                SenderLoginId = entity.From,
                                ReceiveLoginId = entity.To,
                                TransferAmount = entity.Amount,
                                ErrorMessage = "Deposit failed. Withdrawal has been reversed.",
                                mTRetCode = MTRetCode.MT_RET_ERROR.ToString(),
                            }
                        };
                    }
                }
                else
                {
                    return new BaseResponseModel<TransferTerminalToTerminalResponse>
                    {
                        Success = true,
                        Message = "Withdrawal failed.",
                        Data = new TransferTerminalToTerminalResponse
                        {
                            Success = false,
                            SenderLoginId = entity.From,
                            ReceiveLoginId = entity.To,
                            TransferAmount = entity.Amount,
                            ErrorMessage = "Withdrawal failed.",
                            mTRetCode = withdrawalResult.ToString(),

                        }
                    };
                }

                receiver.Release();
                sender.Release();

                return new BaseResponseModel<TransferTerminalToTerminalResponse>
                {
                    Success = true,
                    Message = "Transfer completed successfully.",
                    Data = new TransferTerminalToTerminalResponse
                    {
                        Success = true,
                        SenderLoginId = entity.From,
                        ReceiveLoginId = entity.To,
                        TransferAmount = entity.Amount,
                        ErrorMessage = "Transfer completed successfully.",
                        mTRetCode = MTRetCode.MT_RET_REQUEST_DONE.ToString(),
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<TransferTerminalToTerminalResponse>
                {
                    Success = false,
                    Message = "An error occurred while processing the fund transfer request."
                };
            }
        }


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
