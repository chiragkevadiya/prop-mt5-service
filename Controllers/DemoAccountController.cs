using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Controllers;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Services;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing Demo MT5 accounts
    /// </summary>
    [RoutePrefix("api/demo")]
    public class DemoAccountController : BaseApiController
    {
        private readonly IMT5AccountService _accountService;

        public DemoAccountController(CIMTManagerAPI managerDemo) : base(managerDemo)
        {
            var config = new AccountCreationConfig
            {
                LoginPrefix = "999",
                ServerName = "QuorionexMarketsTestOnly-Trade",
                AccountType = "Demo"
            };
            _accountService = new MT5AccountService(managerDemo, config);
        }

        /// <summary>
        /// Get a single demo account by login ID
        /// </summary>
        [HttpGet]
        [Route("account/{loginId:long}")]
        public IHttpActionResult GetSingleDemoAccount(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var account = _accountService.GetSingleAccount((ulong)loginId);
                return new BaseResponse<Mt5LiveAccountVM>().WithSuccess(account, "Demo account retrieved successfully");
            });
        }

        /// <summary>
        /// Get all demo accounts
        /// </summary>
        [HttpGet]
        [Route("accounts")]
        public IHttpActionResult GetAllDemoAccounts()
        {
            return ExecuteSafe(() =>
            {
                var accounts = _accountService.GetAllAccounts();
                return new BaseResponse<IEnumerable<Mt5LiveAccountVM>>().WithSuccess(accounts, "Demo accounts retrieved successfully");
            });
        }

        /// <summary>
        /// Create a new demo account
        /// </summary>
        [HttpPost]
        [Route("account/create")]
        public IHttpActionResult CreateDemoAccount([FromBody] UserIdModel model)
        {
            if (model == null)
                return BadRequest("Model cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return ExecuteSafe(() => _accountService.CreateAccount(model, null));
        }
    }
}
