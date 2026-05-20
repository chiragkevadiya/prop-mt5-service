using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Controllers;
using PropMT5Service.Helpers;
using PropMT5Service.Services;
using PropMT5Service.ViewModels;
using System.Collections.Generic;
using System.Web.Http;
using static PropMT5Service.Constants.MT5Constants;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace PropMT5Service
{
    /// <summary>
    /// Controller for managing Live MT5 accounts
    /// </summary>
    [RoutePrefix("api/mt5")]
    public class LiveAccountController : BaseApiController
    {
        private readonly IMT5AccountService _accountService;

        public LiveAccountController(CIMTManagerAPI manager) : base(manager)
        {
            var config = new AccountCreationConfig
            {

            };
            _accountService = new MT5AccountService(manager, config);
        }

        /// <summary>
        /// Get a single live account by login ID
        /// </summary>
        [HttpGet]
        [Route("account/{loginId:long}")]
        public IHttpActionResult GetSingleLiveAccount(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var account = _accountService.GetSingleAccount((ulong)loginId);
                return new BaseResponse<Mt5LiveAccountVM>().WithSuccess(account, "Account retrieved successfully");
            });
        }

        /// <summary>
        /// Get all live accounts
        /// </summary>
        [HttpGet]
        [Route("accounts")]
        public IHttpActionResult GetAllLiveAccounts()
        {
            return ExecuteSafe(() =>
            {
                var accounts = _accountService.GetAllAccounts();
                return new BaseResponse<IEnumerable<Mt5LiveAccountVM>>().WithSuccess(accounts, "Accounts retrieved successfully");
            });
        }

        /// <summary>
        /// Create a new live account
        /// </summary>
        [HttpPost]
        [Route("account/create")]
        public IHttpActionResult CreateLiveAccount([FromBody] UserIdModel model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return ExecuteSafe(() => _accountService.CreateAccount(model, null));
        }
    }
}
