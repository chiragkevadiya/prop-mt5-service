using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using NaptunePropTrading_Service.Helper;
using NaptunePropTrading_Service.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NaptunePropTrading_Service.Controllers
{
    public class EnableAccountController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public EnableAccountController()
        {

        }
        [HttpPost]
        public BaseResponse<int> MT5EnableAccount([FromBody] MT5AccountInActiveVM entity)
        {
            try
            {
                foreach (ulong loginId in entity.LoginId)
                {
                    CIMTUser cIMTUser = _manager.UserCreate();
                    MTRetCode resultCode = _manager.UserGet(loginId, cIMTUser);

                    if (MTRetCode.MT_RET_OK == resultCode)
                    {
                        // Check permission

                        if (entity.UserStatus)
                        {
                            // Enable the account
                            cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_ENABLED);
                        }
                        else
                        {
                            // DISABLED the account
                            cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED);
                        }

                        // Send changes
                        MTRetCode updateResult = _manager.UserUpdate(cIMTUser);

                    }

                }

                LogDirectorySuccess(entity);

                return new BaseResponse<int> { Success = true, Message = "Success" };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void LogDirectorySuccess(MT5AccountInActiveVM entity)
        {
            // Define log directory and file name
            string logDirectory = @"C:\MT5ServicesLogSave\Sucess\MT5_Account";
            string logFileName = $"Trade_{(entity.UserStatus ? "ENABLED" : "DISABLED")}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                // Ensure the log directory exists
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Write the log data to the file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("-----------------MT5 Account Log--------------------------");
                    writer.WriteLine($"Operation: Trade {(entity.UserStatus ? "ENABLED" : "DISABLED")}");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine("Account Details: ");

                    foreach (var loginId in entity.LoginId)
                    {
                        writer.WriteLine($"- MT5 Account No.: {loginId}");
                    }

                    writer.WriteLine("----------------------------------------------------------");
                    writer.WriteLine(); // Optional: Adds a blank line for readability
                }

                // Optional: Console message for successful log
                // Console.WriteLine("Information logged successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging information: {ex.Message}");
            }
        }
    }
}
