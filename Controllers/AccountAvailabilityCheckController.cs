using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    [RoutePrefix("api/account-availability")] // Updated route prefix for consistency
    public class AccountAvailabilityCheckController : ApiController
    {
        private readonly CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpPost]
        [Route("deleted")] // Explicit route for the action
        public BaseResponseModel<List<ulong>> GetAllDeletedAccount([FromBody] List<ulong> loginIds)
        {
            var responseList = new List<ulong>();

            try
            {
                if (loginIds == null || loginIds.Count == 0)
                {
                    return new BaseResponseModel<List<ulong>>
                    {
                        Success = false,
                        Message = "No login IDs provided."
                    };
                }

                // STEP 1: Pre-fetch and store original details
                foreach (var loginId in loginIds)
                {
                    CIMTUser originalUser = _manager.UserCreate();
                    var getResult = _manager.UserGet(loginId, originalUser);

                    if (getResult == MTRetCode.MT_RET_ERR_NOTFOUND)
                    {
                        responseList.Add(loginId);
                    }
                }
                return new BaseResponseModel<List<ulong>>
                {
                    Success = true,
                    Message = "Deleted account retrieved successfully.",
                    Data = responseList
                };

            }
            catch (Exception)
            {
                return new BaseResponseModel<List<ulong>>
                {
                    Success = true,
                    Message = "Something went wrong while gathering deleted account information.",
                };
            }
        }
    }
}
