using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 deposit operations
    /// </summary>
    [RoutePrefix("api/deposit")]
    public class DepositController : BaseApiController
    {
        public DepositController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Deposit funds into an account
        /// </summary>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Deposit([FromBody] Mt5DepositBalanceVM model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (model.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            return ExecuteSafe(() =>
            {
                ulong transactionId;
                var result = _manager.DealerBalance(model.Login, model.Amount, 2, model.Comment ?? "Deposit", out transactionId);

                if (result == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    return new BaseResponse<object>().WithSuccess(new { TransactionId = transactionId }, "Deposit successful");
                }

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
            });
        }

        /// <summary>
        /// Deposit funds with raw balance operation (no margin check)
        /// </summary>
        [HttpPost]
        [Route("raw")]
        public IHttpActionResult DepositRaw([FromBody] Mt5DepositBalanceVM model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (model.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            return ExecuteSafe(() =>
            {
                ulong transactionId;
                var result = _manager.DealerBalanceRaw(model.Login, model.Amount, 2, model.Comment ?? "Deposit", out transactionId);

                if (result == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    return new BaseResponse<object>().WithSuccess(new { TransactionId = transactionId }, "Deposit successful");
                }

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
            });
        }
    }
}
