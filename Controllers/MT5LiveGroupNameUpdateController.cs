using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using Nancy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class MT5LiveGroupNameUpdateController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public BaseResponseModel<List<ulong>> MT5LiveGroupNameChanges(string accoount, string GroupName)
        {
            if (string.IsNullOrWhiteSpace(accoount))
                return new BaseResponseModel<List<ulong>>
                {
                    Message = $"Please enter LoginIds",
                    Success = false,
                    MTRetErrorCode = 0
                }; 

            List<ulong> loginIds = accoount?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToUInt64(x)).ToList();


            var originalGroups = new Dictionary<ulong, string>();
            var usersWithOpenPositions = new List<ulong>();
            try
            {
                // Step 1: Pre-Check for Open Positions
                foreach (var loginId in loginIds)
                {
                    CIMTUser user = _manager.UserCreate();
                    var getUserCode = _manager.UserGet(loginId, user);

                    if (getUserCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                        return new BaseResponseModel<List<ulong>>
                        {
                            Data = new List<ulong> { loginId },
                            Message = $"Login ID {loginId} not found.",
                            Success = false,
                            MTRetErrorCode = getUserCode
                        };

                    // Check open positions
                    var positionArray = _manager.PositionCreateArray();
                    var positionCheck = _manager.PositionGet(loginId, positionArray);

                    if (positionCheck == MTRetCode.MT_RET_OK && positionArray.Total() > 0)
                    {
                        usersWithOpenPositions.Add(loginId);
                    }
                }

                // If any user has open positions, return and abort
                if (usersWithOpenPositions.Count > 0)
                {
                    return new BaseResponseModel<List<ulong>>
                    {
                        Data = usersWithOpenPositions,
                        Message = "Group name update aborted. Some users have open positions.",
                        Success = false,
                        MTRetErrorCode = MTRetCode.MT_RET_OK
                    };
                }

                // Step 2: All good, perform update
                foreach (var loginId in loginIds)
                {
                    var user = _manager.UserCreate();
                    var getUserCode = _manager.UserGet(loginId, user);

                    if (getUserCode != MTRetCode.MT_RET_OK)
                        throw new Exception($"User fetch failed for {loginId}");

                    originalGroups[loginId] = user.Group();

                    user.Group(GroupName);
                    var updateCode = _manager.UserUpdate(user);

                    if (updateCode != MTRetCode.MT_RET_OK)
                        throw new Exception($"User update failed for {loginId}: {updateCode}");
                }
                return new BaseResponseModel<List<ulong>>
                {
                    Data = loginIds,
                    Message = "All users updated successfully.",
                    Success = true,
                    MTRetErrorCode = MTRetCode.MT_RET_OK
                };
            }
            catch (Exception ex)
            {
                // Rollback in case of any error
                foreach (var kvp in originalGroups)
                {
                    var user = _manager.UserCreate();
                    if (_manager.UserGet(kvp.Key, user) == MTRetCode.MT_RET_OK)
                    {
                        user.Group(kvp.Value);
                        _manager.UserUpdate(user); // rollback
                    }
                }
                return new BaseResponseModel<List<ulong>>
                {
                    Data = new List<ulong>(),
                    Message = $"Error occurred: {ex.Message}. All changes reverted.",
                    Success = false,
                    MTRetErrorCode = MTRetCode.MT_RET_ERROR
                };
                throw;
            }
        }
    }
}
