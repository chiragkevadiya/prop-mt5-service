using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.Services;
using PropMT5Service.ViewModels;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for managing MT5 account status
    /// </summary>
    [RoutePrefix("api/account/status")]
    public class LiveAccountStatusController : BaseApiController
    {
        private readonly IMT5AccountService _accountService;

        public LiveAccountStatusController(CIMTManagerAPI manager) : base(manager)
        {
            _accountService = new MT5AccountService(manager);
        }

        /// <summary>
        /// Set MT5 account active/inactive status
        /// </summary>
        [HttpPost]
        [Route("update")]
        public IHttpActionResult UpdateAccountStatus([FromBody] Mt5AccountStatusVM model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return ExecuteSafe(() =>
            {
                var result = _accountService.SetAccountStatus(model);
                return new BaseResponse<int>
                {
                    Success = result.Success,
                    Message = result.Message,
                    StatusCode = result.StatusCode,
                    Data = result.Success ? 1 : 0
                };
            });
        }
    }
}
