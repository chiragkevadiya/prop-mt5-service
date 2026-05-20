using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for deleting live MT5 accounts
    /// </summary>
    [RoutePrefix("api/mt5/account")]
    public class LiveAccountDeleteController : BaseApiController
    {
        public LiveAccountDeleteController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Delete multiple live accounts by login IDs.
        /// Skips accounts with open positions.
        /// </summary>
        [HttpPost]
        [Route("delete")]
        public IHttpActionResult DeleteAccounts([FromBody] List<ulong> loginIds)
        {
            if (loginIds == null || loginIds.Count == 0)
                return BadRequest("No login IDs provided.");

            return ExecuteSafe(() =>
            {
                var responseList = new List<AccountDeleteResult>();

                foreach (ulong loginId in loginIds)
                {
                    var result = new AccountDeleteResult { LoginId = loginId };

                    CIMTPositionArray positions = _manager.PositionCreateArray();
                    if (positions == null)
                    {
                        result.Status = "Failed";
                        result.Message = "Unable to create position array.";
                        responseList.Add(result);
                        continue;
                    }

                    var posResult = _manager.PositionGet(loginId, positions);
                    if (posResult == MTRetCode.MT_RET_OK && positions.Total() > 0)
                    {
                        result.Status = "Skipped";
                        result.Message = "Open positions exist.";
                        positions.Release();
                        responseList.Add(result);
                        continue;
                    }
                    positions.Release();

                    CIMTUser user = _manager.UserCreate();
                    if (user == null)
                    {
                        result.Status = "Failed";
                        result.Message = "Unable to create user object.";
                        responseList.Add(result);
                        continue;
                    }

                    var getUserCode = _manager.UserGet(loginId, user);
                    if (getUserCode != MTRetCode.MT_RET_OK)
                    {
                        result.Status = "Failed";
                        result.Message = $"User not found (code {getUserCode}).";
                        user.Release();
                        responseList.Add(result);
                        continue;
                    }

                    var deleteCode = _manager.UserDelete(loginId);
                    if (deleteCode == MTRetCode.MT_RET_OK)
                    {
                        result.Status = "Success";
                        result.Message = "User deleted.";
                    }
                    else
                    {
                        result.Status = "Failed";
                        result.Message = $"Delete failed (code {deleteCode}).";
                    }

                    user.Release();
                    responseList.Add(result);
                }

                return new BaseResponse<List<AccountDeleteResult>>().WithSuccess(responseList, "Delete process completed.");
            });
        }
    }
}
