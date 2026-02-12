using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services.Interfaces
{
    public interface IMT5AccountService
    {
        Task<BaseResponseModel<MtfiveaccountVM>> CreateAccountAsync(
            Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address,
            string country, bool isDemo = false);

        Task<BaseResponseModel<CIMTAccount>> GetAccountAsync(ulong loginId, bool isDemo = false);

        Task<BaseResponseModel<List<CIMTAccount>>> GetAccountsByGroupAsync(string groupName, bool isDemo = false);

        Task<MTRetCode> UpdateAccountGroupAsync(ulong loginId, string newGroupName, bool isDemo = false);

        Task<MTRetCode> UpdateAccountLeverageAsync(ulong loginId, uint newLeverage, bool isDemo = false);

        Task<BaseResponseModel<bool>> DepositAsync(ulong loginId, decimal amount, string comment, bool isDemo = false);

        Task<BaseResponseModel<bool>> WithdrawAsync(ulong loginId, decimal amount, string comment, bool isDemo = false);

        Task<MTRetCode> DisableAccountAsync(ulong loginId, bool isDemo = false);

        Task<MTRetCode> EnableAccountAsync(ulong loginId, bool isDemo = false);

        Task<BaseResponseModel<AccountDetailsVM>> GetAccountDetailsAsync(long terminalId, bool isDemo = false);
    }
}
