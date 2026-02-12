using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for checking account availability (deleted accounts)
    /// </summary>
    [RoutePrefix("api/mt5/account-availability")]
    public class AccountAvailabilityController : BaseApiController
    {
        public AccountAvailabilityController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Check which login IDs correspond to deleted accounts
        /// </summary>
        [HttpPost]
        [Route("deleted")]
        public IHttpActionResult GetDeletedAccounts([FromBody] List<ulong> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var deletedAccounts = new List<ulong>();

                foreach (var loginId in loginIds)
                {
                    CIMTUser user = _manager.UserCreate();
                    var getResult = _manager.UserGet(loginId, user);

                    if (getResult == MTRetCode.MT_RET_ERR_NOTFOUND)
                    {
                        deletedAccounts.Add(loginId);
                    }

                    user.Release();
                }

                return new BaseResponse<List<ulong>>().WithSuccess(deletedAccounts, "Deleted accounts retrieved successfully.");
            });
        }
    }
}
