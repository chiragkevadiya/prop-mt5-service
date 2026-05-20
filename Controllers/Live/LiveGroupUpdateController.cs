using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    [RoutePrefix("api/livegroupupdate")]
    public class LiveGroupUpdateController : BaseApiController
    {
        public LiveGroupUpdateController(CIMTManagerAPI manager) : base(manager) { }

        [HttpGet]
        [Route("")]
        public IHttpActionResult MT5LiveGroupNameChanges(string account, string groupName)
        {
            if (string.IsNullOrWhiteSpace(account))
                return BadRequestResponse<List<ulong>>("Please enter LoginIds.");

            List<ulong> loginIds = account
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToUInt64(x.Trim()))
                .ToList();

            return ExecuteSafe<List<ulong>>(() =>
            {
                // Phase 1: verify all accounts exist and have no open positions
                var usersWithOpenPositions = new List<ulong>();

                foreach (ulong loginId in loginIds)
                {
                    MTRetCode getUserCode = MTRetCode.MT_RET_OK;

                    WithUser(loginId, (user, ret) =>
                    {
                        getUserCode = ret;
                        return 0;
                    });

                    if (getUserCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                        return new BaseResponse<List<ulong>>().WithError($"Login ID {loginId} not found.", 404);

                    WithPositions(loginId, (positions, ret) =>
                    {
                        if (ret == MTRetCode.MT_RET_OK && positions.Total() > 0)
                            usersWithOpenPositions.Add(loginId);
                        return 0;
                    });
                }

                if (usersWithOpenPositions.Count > 0)
                    return new BaseResponse<List<ulong>>
                    {
                        Success = false,
                        Message = "Group update aborted: the following accounts have open positions.",
                        Data = usersWithOpenPositions,
                        StatusCode = 409
                    };

                // Phase 2: apply group change with rollback on failure
                var originalGroups = new Dictionary<ulong, string>();

                foreach (ulong loginId in loginIds)
                {
                    MTRetCode updateCode = MTRetCode.MT_RET_OK;

                    WithUser(loginId, (user, ret) =>
                    {
                        if (ret != MTRetCode.MT_RET_OK)
                        {
                            updateCode = ret;
                            return 0;
                        }

                        originalGroups[loginId] = user.Group();
                        user.Group(groupName);
                        updateCode = _manager.UserUpdate(user);
                        return 0;
                    });

                    if (!IsSuccessful(updateCode))
                    {
                        RollbackGroups(originalGroups);
                        return new BaseResponse<List<ulong>>().WithError(
                            $"Update failed for login {loginId}. All changes reverted.", 400);
                    }
                }

                return new BaseResponse<List<ulong>>().WithSuccess(loginIds, "All accounts updated successfully.");
            });
        }

        private void RollbackGroups(Dictionary<ulong, string> originalGroups)
        {
            foreach (var kvp in originalGroups)
            {
                WithUser(kvp.Key, (user, ret) =>
                {
                    if (ret == MTRetCode.MT_RET_OK)
                    {
                        user.Group(kvp.Value);
                        _manager.UserUpdate(user);
                    }
                    return 0;
                });
            }
        }
    }
}
