using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/livewithdrawal")]
    public class LiveWithdrawalController : BaseApiController
    {
        public LiveWithdrawalController(CIMTManagerAPI manager) : base(manager) { }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Withdraw([FromBody] Mt5DepositBalanceVM entity)
        {
            if (entity == null)
                return BadRequestResponse<object>(MT5Constants.ResponseMessages.ModelCannotBeNull);

            if (entity.Amount <= 0)
                return BadRequestResponse<object>("Withdrawal amount must be greater than zero.");

            return ExecuteSafe(() =>
            {
                double balance = MT5AccountOperations.GetBalanceForLogin(_manager, entity.Login);

                if (balance <= 0 || balance < entity.Amount)
                    return new BaseResponse<object>().WithError(
                        $"Insufficient balance. Available: {balance:F2}, Requested: {entity.Amount:F2}.", 400);

                MTRetCode ret = _manager.DealerBalance(
                    entity.Login, -entity.Amount,
                    MT5Constants.AccountOperations.DealerBalanceType,
                    entity.Comment, out _);

                if (ret == MTRetCode.MT_RET_REQUEST_DONE)
                    return new BaseResponse<object>().WithSuccess(
                        new { entity.Login, entity.Amount }, "Withdrawal completed successfully.");

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(ret), 400);
            });
        }
    }
}
