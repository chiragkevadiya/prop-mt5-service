using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for batch retrieval of demo user accounts
    /// </summary>
    [RoutePrefix("api/demo/accounts")]
    public class DemoUserAccountBatchController : BaseApiController
    {
        public DemoUserAccountBatchController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get multiple demo accounts by login IDs
        /// </summary>
        [HttpPost]
        [Route("batch")]
        public IHttpActionResult GetDemoAccountsBatch([FromBody] List<ulong> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var accounts = MT5AccountOperations.GetAccountsByLoginIds(_manager, loginIds);
                return new BaseResponse<List<Mt5LiveAccountVM>>().WithSuccess(accounts, "Demo accounts retrieved successfully");
            });
        }
    }
}
