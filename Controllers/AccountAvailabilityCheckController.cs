using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class AccountAvailabilityCheckController : ApiController
    {
        private readonly CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpPost]
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
            catch (Exception ex)
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
