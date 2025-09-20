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
    public class MT5AccountDeleteController : ApiController
    {
        private readonly CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpPost]
        public BaseResponseModel<List<AccountDeleteResult>> DeleteAccounts([FromBody] List<ulong> loginIds)
        {
            var responseList = new List<AccountDeleteResult>();

            try
            {
                if (loginIds == null || loginIds.Count == 0)
                {
                    return new BaseResponseModel<List<AccountDeleteResult>>
                    {
                        Success = false,
                        Message = "No login IDs provided.",
                        Data = responseList
                    };
                }

                foreach (ulong loginId in loginIds)
                {
                    var result = new AccountDeleteResult { LoginId = loginId };

                    // Create position array
                    CIMTPositionArray positions = _manager.PositionCreateArray();
                    if (positions == null)
                    {
                        result.Status = "Failed";
                        result.Message = "Unable to create position array.";
                        responseList.Add(result);
                        continue;
                    }

                    // Check for open positions
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

                    // Get user
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

                    // Delete user
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

                return new BaseResponseModel<List<AccountDeleteResult>>
                {
                    Success = true,
                    Message = "Process completed.",
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseModel<List<AccountDeleteResult>>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Data = responseList
                };
            }
        }
    }
}
