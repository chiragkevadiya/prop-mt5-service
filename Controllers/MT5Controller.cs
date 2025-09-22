using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.BuilderProperties;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using MT5ConnectionService.ViewModels.GroupName;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;


namespace MT5ConnectionService
{

    public class MT5Controller : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();
        public MT5Controller()
        {

        }

        // Get by User Id

        [HttpGet]
        public MtGetAllLiveAccountVM UserGetSingleLiveAccount(ulong LoginId)
        {
            try
            {

                MtGetAllLiveAccountVM liveAccountVM = new MtGetAllLiveAccountVM();

                CIMTUser cIMTUser = _manager.UserCreate();
                MTRetCode mTRetCode = _manager.UserGet(LoginId, cIMTUser);

                CIMTAccount cIMTAccountInfo = _manager.UserCreateAccount();
                MTRetCode mTRetCode1 = _manager.UserAccountGet(LoginId, cIMTAccountInfo);

                if (MTRetCode.MT_RET_OK == mTRetCode)
                {
                    liveAccountVM.Login = cIMTUser.Login();
                    liveAccountVM.FirstName = cIMTUser.FirstName();
                    liveAccountVM.LastName = cIMTUser.LastName();
                    liveAccountVM.Group = cIMTUser.Group();
                    liveAccountVM.Country = cIMTUser.Country();

                    liveAccountVM.BalancePrevDay = cIMTUser.BalancePrevDay();
                    liveAccountVM.EquityPrevDay = cIMTUser.EquityPrevDay();

                    liveAccountVM.Credit = cIMTAccountInfo.Credit();
                    liveAccountVM.Balance = cIMTAccountInfo.Balance();
                    liveAccountVM.Leverage = cIMTUser.Leverage();
                    liveAccountVM.Status = cIMTUser.Status();

                    liveAccountVM.Margin = cIMTAccountInfo.Margin();
                    liveAccountVM.MarginFree = cIMTAccountInfo.MarginFree();
                    liveAccountVM.Profit = cIMTAccountInfo.Profit();
                    liveAccountVM.Commission = 0;
                    liveAccountVM.Equity = cIMTAccountInfo.Equity();

                }
                else
                {
                    liveAccountVM.Status = "Data Not Found.";
                    liveAccountVM.MTRetCodeError = mTRetCode;
                }

                // Release resources
                cIMTUser.Clear();
                cIMTUser.Release();

                return liveAccountVM;


                #region old code
                //MtGetAllLiveAccountVM liveAccountVM1 = new MtGetAllLiveAccountVM();

                //CIMTUser cIMTUserc = _manager.UserCreate();

                //MTRetCode mTRetCode1 = _manager.UserGet(LoginId, cIMTUserc);

                //if (MTRetCode.MT_RET_OK == mTRetCode1)
                //{

                //    //MtGetAllLiveAccountVM liveAccountVM1 = new MtGetAllLiveAccountVM()
                //    //{
                //    //    Login = cIMTUserc.Login(),
                //    //    FirstName = cIMTUserc.FirstName(),
                //    //    LastName = cIMTUserc.LastName(),
                //    //    Group = cIMTUserc.Group(),
                //    //    Country = cIMTUserc.Country(),
                //    //    Credit = cIMTUserc.Credit(),
                //    //    Balance = cIMTUserc.Balance(),
                //    //    Leverage = cIMTUserc.Leverage(),
                //    //    Status = cIMTUserc.Status()
                //    //};

                //    liveAccountVM1.Login = cIMTUserc.Login();
                //    liveAccountVM1.FirstName = cIMTUserc.FirstName();
                //    liveAccountVM1.LastName = cIMTUserc.LastName();
                //    liveAccountVM1.Group = cIMTUserc.Group();
                //    liveAccountVM1.Country = cIMTUserc.Country();
                //    liveAccountVM1.Credit = cIMTUserc.Credit();
                //    liveAccountVM1.Balance = cIMTUserc.Balance();
                //    liveAccountVM1.Leverage = cIMTUserc.Leverage();
                //    liveAccountVM1.Status = cIMTUserc.Status();
                //    liveAccountVM1.MTRetCodeError = mTRetCode1;

                //    cIMTUserc.Clear();
                //    cIMTUserc.Release();
                //    return liveAccountVM1;
                //}
                //else
                //{
                //    return liveAccountVM1.MTRetCodeError = mTRetCode1;
                //}
                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpGet]
        public IEnumerable<MtGetAllLiveAccountVM> GetAllLiveAccount()
        {
            try
            {
                List<MtGetAllLiveAccountVM> mtGetAllLiveAccountVM = new List<MtGetAllLiveAccountVM>();

                CIMTUserArray cIMTUserArray = _manager.UserCreateArray();

                MTRetCode mTRetCode1 = _manager.UserGetByGroup("*", cIMTUserArray);

                if (MTRetCode.MT_RET_OK == mTRetCode1)
                {
                    cIMTUserArray.Total();

                    for (uint i = 0; i < cIMTUserArray.Total(); i++)
                    {
                        CIMTUser cIMTUser1 = cIMTUserArray.Next(i);

                        MtGetAllLiveAccountVM liveAccountVM1 = new MtGetAllLiveAccountVM()
                        {
                            Login = cIMTUser1.Login(),
                            FirstName = cIMTUser1.FirstName(),
                            LastName = cIMTUser1.LastName(),
                            Group = cIMTUser1.Group(),
                            Country = cIMTUser1.Country(),
                            Credit = cIMTUser1.Credit(),
                            Balance = cIMTUser1.Balance(),
                            Leverage = cIMTUser1.Leverage(),
                            Status = cIMTUser1.Status()
                        };
                        mtGetAllLiveAccountVM.Add(liveAccountVM1);
                    }

                    cIMTUserArray.Clear();
                    cIMTUserArray.Release();
                    return mtGetAllLiveAccountVM;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public IEnumerable<MtfiveaccountVM> CreateLiveAccount([FromBody] UserIdModel entity)
        {
            try
            {
                bool maxRetry = true;
                int attemptCount = 0;

                while (maxRetry)
                {
                    var prefix = "555";
                    Random random = new Random();

                    // always 3 digits (000–999)
                    string randomPart = random.Next(0, 1000).ToString("D3");

                    // final login string: 555XXX
                    string loginString = prefix + randomPart;

                    // parse to ulong
                    entity.LoginId = ulong.Parse(loginString);


                    // Create an empty user object

                    CIMTUser cIMTUser = _manager.UserCreate();

                    cIMTUser.Login((ulong)entity.LoginId);
                    //cIMTUser.Group("demo\\demo\\2x");

                    cIMTUser.FirstName(entity.FirstName);
                    cIMTUser.LastName(entity.LastName);
                    cIMTUser.Leverage(entity.Leverage);
                    cIMTUser.Group(entity.GroupName);

                    cIMTUser.EMail(entity.EMail);
                    cIMTUser.Phone(entity.Phone);
                    cIMTUser.Address(entity.Address);
                    cIMTUser.Country(entity.Country);
                    
                    // Set master and investor passwords (replace with your logic)
                    string master_pass = GenerateRandomPass.GenerateMasterPassword(11); // Replace with a valid master password
                    string investor_pass = GenerateRandomPass.GenerateInvestorPassword(9); // Replace with a valid investor password


                    cIMTUser.Rights((CIMTUser.EnUsersRights.USER_RIGHT_ENABLED |
                        CIMTUser.EnUsersRights.USER_RIGHT_OTP_ENABLED |
                        CIMTUser.EnUsersRights.USER_RIGHT_PASSWORD));


                    // Attempt to add the user with passwords

                    MTRetCode mTRetCode = _manager.UserAdd(cIMTUser, master_pass, investor_pass);

                    ulong[] userLogins = _manager.UserLogins(entity.GroupName, out MTRetCode res);

                    //MTRetCode userGet = _manager.UserGet(userLogins.Last(), cIMTUser);
                    if (mTRetCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                    {
                        attemptCount++;
                        if (attemptCount < 100)
                        {
                            continue;
                        }
                    }

                    if (MTRetCode.MT_RET_OK == mTRetCode)
                    {
                        maxRetry = false;
                        //cIMTUser.Clear();

                        // Create an instance of MtfiveaccountVM and populate it
                        MtfiveaccountVM userAccount = new MtfiveaccountVM
                        {
                            UserId = entity.UserId, // Replace with the actual property/method to get the user ID
                            Login = (ulong)(entity.LoginId != 0 ? entity.LoginId : userLogins.Last()), // Set the login as needed
                            GroupName = entity.GroupName,
                            master_pass = master_pass, // Set the master password
                            investor_pass = investor_pass, // Set the investor password
                            Leverage = entity.Leverage,
                            ServerName = "PropTradingMT5"//"HorizonFXTradings-Server" //"Fxo"
                        };

                        List<MtfiveaccountVM> userAccounts = new List<MtfiveaccountVM>();
                        userAccounts.Add(userAccount);

                        // log Directory Sucess

                        logDirectorySucess(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                            entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, userLogins.Last(), master_pass, investor_pass);

                        return userAccounts;
                    }
                    else
                    {
                        // log Directory Failed
                        logDirectoryFailed(entity.UserId, entity.GroupName, entity.Leverage, entity.FirstName,
                            entity.LastName, entity.EMail, entity.Phone, entity.Address, entity.Country, mTRetCode, master_pass, investor_pass);
                        return null;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void logDirectorySucess(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country, ulong login, string master_pass, string investor_pass)
        {
            // Log directory path
            //string logDirectory = @"C:\MT5ServicesLogSave\Sucess";
            //string logFilePath = Path.Combine(logDirectory, "Sucess_log.txt");

            // Log directory path
            string logDirectory = @"C:\MT5ServicesLogSave\Sucess";
            string logFileName = $"Sucess_log_{login}_{DateTime.Now:yyyyMMddHHmmssfff}.txt"; // Unique file name based on timestamp
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                // Create the log directory if it doesn't exist
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create or append to the log file
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    // Log the information
                    writer.WriteLine($"UserId: {userId}");
                    writer.WriteLine($"GroupName: {groupName}");
                    writer.WriteLine($"Leverage: {leverage}");
                    writer.WriteLine($"FirstName: {firstName}");
                    writer.WriteLine($"LastName: {lastName}");
                    writer.WriteLine($"EMail: {email}");
                    writer.WriteLine($"Phone: {phone}");
                    writer.WriteLine($"Address: {address}");
                    writer.WriteLine($"Country: {country}");
                    writer.WriteLine("-----------------MT5 Account Create--------------------------");
                    writer.WriteLine($"MT5 Account No.: {login}");
                    writer.WriteLine($"Master Password: {master_pass}");
                    writer.WriteLine($"Investor Password: {investor_pass}");

                    writer.WriteLine(); // Add a blank line for better readability

                    // Optionally, you can add a timestamp
                    writer.WriteLine($"Logged at: {DateTime.Now}");
                    writer.WriteLine("-------------------------------------------");
                }

                //Console.WriteLine("Information logged successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private void logDirectoryFailed(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country, MTRetCode mTRetCode, string master_pass, string investor_pass)
        {
            // Log directory path
            //string logDirectory = @"C:\MT5ServicesLogSave\Failed";
            //string logFilePath = Path.Combine(logDirectory, "Failed_log.txt");
            // Log directory path
            string logDirectory = @"C:\MT5ServicesLogSave\Failed";
            string logFileName = $"Failed_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt"; // Unique file name based on timestamp
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                // Create the log directory if it doesn't exist
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create or append to the log file
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    // Log the information
                    writer.WriteLine($"UserId: {userId}");
                    writer.WriteLine($"GroupName: {groupName}");
                    writer.WriteLine($"Leverage: {leverage}");
                    writer.WriteLine($"FirstName: {firstName}");
                    writer.WriteLine($"LastName: {lastName}");
                    writer.WriteLine($"EMail: {email}");
                    writer.WriteLine($"Phone: {phone}");
                    writer.WriteLine($"Address: {address}");
                    writer.WriteLine($"Country: {country}");
                    writer.WriteLine($"Error Code: {mTRetCode}");
                    writer.WriteLine("-----------------Failed--------------------------");
                    writer.WriteLine($"Master Password: {master_pass}");
                    writer.WriteLine($"Investor Password: {investor_pass}");
                    writer.WriteLine(); // Add a blank line for better readability

                    // Optionally, you can add a timestamp
                    writer.WriteLine($"Logged at: {DateTime.Now}");
                    writer.WriteLine("-------------------------------------------");
                }

                //Console.WriteLine("Information logged Failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
