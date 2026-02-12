using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services.Interfaces
{
    public interface IMT5GroupService
    {
        Task<BaseResponseModel<List<CIMTConGroup>>> GetAllGroupsAsync(bool isDemo = false);

        Task<BaseResponseModel<CIMTConGroup>> GetGroupByNameAsync(string groupName, bool isDemo = false);

        Task<BaseResponseModel<List<ulong>>> UpdateGroupForAccountsAsync(
            List<ulong> loginIds, string newGroupName, bool isDemo = false);

        Task<BaseResponseModel<GroupwithSymbolVM>> GetGroupWithSymbolsAsync(string groupName, bool isDemo = false);
    }
}
