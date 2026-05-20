using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Helpers;
using PropMT5Service.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for retrieving user accounts filtered by group
    /// </summary>
    [RoutePrefix("api/mt5/accounts")]
    public class UserAccountGetByGroupController : BaseApiController
    {
        public UserAccountGetByGroupController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all user accounts filtered by login IDs from a configured file
        /// </summary>
        [HttpGet]
        [Route("by-group")]
        public IHttpActionResult GetUserAccountsByGroup()
        {
            return ExecuteSafe(() =>
            {
                CIMTAccountArray accountArray = _manager.UserCreateAccountArray();
                MTRetCode retCode = _manager.UserAccountRequestArray("*", accountArray);

                if (retCode != MTRetCode.MT_RET_OK)
                    throw new InvalidOperationException($"Failed to request account array: {retCode}");

                string filePath = "C:\\inetpub\\wwwroot\\mt5.neptunefxcrm.com\\Logs\\LoginUserAccount.txt";

                IEnumerable<UserAccountGetByGroupVM> accounts;

                if (File.Exists(filePath))
                {
                    string[] loginLines = File.ReadAllLines(filePath);
                    string combinedLoginValues = string.Join(",", loginLines);
                    ulong[] loginList = combinedLoginValues
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => ulong.Parse(line.Trim()))
                        .Distinct()
                        .ToArray();

                    if (loginList.Any())
                    {
                        accounts = accountArray.ToArray()
                            .Where(x => loginList.Contains(x.Login()))
                            .Select(item => MapToGroupVM(item))
                            .ToList();
                    }
                    else
                    {
                        accounts = accountArray.ToArray()
                            .Select(item => MapToGroupVM(item))
                            .ToList();
                    }
                }
                else
                {
                    accounts = accountArray.ToArray()
                        .Select(item => MapToGroupVM(item))
                        .ToList();
                }

                accountArray.Release();

                return new BaseResponse<IEnumerable<UserAccountGetByGroupVM>>().WithSuccess(accounts, "Accounts retrieved successfully");
            });
        }

        private static UserAccountGetByGroupVM MapToGroupVM(CIMTAccount item)
        {
            return new UserAccountGetByGroupVM
            {
                Login = item.Login(),
                Balance = item.Balance(),
                Credit = item.Credit(),
                Equity = item.Equity(),
                Margin = item.Margin(),
                MarginFree = item.MarginFree(),
                Profit = item.Profit()
            };
        }
    }
}
