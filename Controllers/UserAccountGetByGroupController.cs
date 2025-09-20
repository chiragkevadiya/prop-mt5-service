using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using MT5ConnectionService.ViewModels.GroupName;
using Nancy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class UserAccountGetByGroupController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();


        [HttpGet]
        public BaseResponseModel<IEnumerable<UserAccountGetByGroupVM>> UserAccountGetByGroup()
        {
            try
            {

                if (_manager == null)
                {
                    return new BaseResponseModel<IEnumerable<UserAccountGetByGroupVM>>
                    {
                        Success = false,
                        Message = "Failed"
                    };
                }

                IEnumerable<UserAccountGetByGroupVM> accountArrayTemp = new List<UserAccountGetByGroupVM>();


                CIMTAccountArray cIMTAccountArray = _manager.UserCreateAccountArray();
                MTRetCode MTRetCode = _manager.UserAccountRequestArray("*", cIMTAccountArray);

                //string filePath = "E:\\OfficeProject\\9dot\\OPFX\\LoginFile\\LoginUserAccount.txt"; // Specify the path to your text file

                string filePath = "C:\\inetpub\\wwwroot\\mt5.neptunefxcrm.com\\Logs\\LoginUserAccount.txt";

                if (!File.Exists(filePath))
                {
                    return new BaseResponseModel<IEnumerable<UserAccountGetByGroupVM>>
                    {
                        Success = false,
                        Message = "File not found."
                    };
                }

                // Read all lines from the text file into an array of strings
                string[] loginLines = File.ReadAllLines(filePath);
                // Combine the lines into a single string, assuming each line might contain comma-separated values
                string combinedLoginValues = string.Join(",", loginLines);
                // Convert the comma-separated string to an array of ulong
                ulong[] loginList = combinedLoginValues
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)  // Split the string by commas
                    .Select(line => ulong.Parse(line.Trim()))                     // Parse each value to ulong
                    .Distinct()                                                   // Remove any duplicates
                    .ToArray();


                if (loginList.Any())
                {
                    accountArrayTemp = cIMTAccountArray.ToArray()
                    .Where(x => loginList.Contains(x.Login()))
                    .Select(Item => new UserAccountGetByGroupVM
                    {
                        Login = Item.Login(),
                        Balance = Item.Balance(),
                        Credit = Item.Credit(),
                        Equity = Item.Equity(),
                        Margin = Item.Margin(),
                        MarginFree = Item.MarginFree(),
                        Profit = Item.Profit()
                    }).ToList();
                }
                else
                {
                    accountArrayTemp = cIMTAccountArray.ToArray()
                   .Select(Item => new UserAccountGetByGroupVM
                   {
                       Login = Item.Login(),
                       Balance = Item.Balance(),
                       Credit = Item.Credit(),
                       Equity = Item.Equity(),
                       Margin = Item.Margin(),
                       MarginFree = Item.MarginFree(),
                       Profit = Item.Profit()
                   }).ToList();
                }

                cIMTAccountArray.Release();

                return new BaseResponseModel<IEnumerable<UserAccountGetByGroupVM>>
                {
                    Data = accountArrayTemp,
                    Success = true,
                    Message = "Success"
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
