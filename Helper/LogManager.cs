using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.Helper
{
    public class LogManager
    {
        private static readonly string BaseLogDirectory = @"C:\PropTradingServices\Logs\";

        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static void WriteLog(string directory, string fileName, string logContent)
        {
            string logFilePath = Path.Combine(directory, fileName);
            EnsureDirectoryExists(directory);

            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(logContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging information: {ex.Message}");
            }
        }

        public static void LogError(string logtitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "ErrorLog");
            string fileName = $"Error_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void LogAccountCreation(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country, ulong login, string masterPass, string investorPass)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Success");
            string fileName = $"Success_log_{login}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            string logContent = $"UserId: {userId}{Environment.NewLine}" +
                                $"GroupName: {groupName}{Environment.NewLine}" +
                                $"Leverage: {leverage}{Environment.NewLine}" +
                                $"FirstName: {firstName}{Environment.NewLine}" +
                                $"LastName: {lastName}{Environment.NewLine}" +
                                $"Email: {email}{Environment.NewLine}" +
                                $"Phone: {phone}{Environment.NewLine}" +
                                $"Address: {address}{Environment.NewLine}" +
                                $"Country: {country}{Environment.NewLine}" +
                                $"-----------------MT5 Account Created--------------------------{Environment.NewLine}" +
                                $"MT5 Account No.: {login}{Environment.NewLine}" +
                                $"Master Password: {masterPass}{Environment.NewLine}" +
                                $"Investor Password: {investorPass}{Environment.NewLine}" +
                                $"Logged at: {DateTime.Now}{Environment.NewLine}" +
                                $"-------------------------------------------------------------{Environment.NewLine}";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void LogAccountFailure(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country, MTRetCode errorCode, string masterPass, string investorPass)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Failed");
            string fileName = $"Failed_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            string logContent = $"UserId: {userId}{Environment.NewLine}" +
                                $"GroupName: {groupName}{Environment.NewLine}" +
                                $"Leverage: {leverage}{Environment.NewLine}" +
                                $"FirstName: {firstName}{Environment.NewLine}" +
                                $"LastName: {lastName}{Environment.NewLine}" +
                                $"Email: {email}{Environment.NewLine}" +
                                $"Phone: {phone}{Environment.NewLine}" +
                                $"Address: {address}{Environment.NewLine}" +
                                $"Country: {country}{Environment.NewLine}" +
                                $"Error Code: {errorCode}{Environment.NewLine}" +
                                $"-----------------Failed--------------------------{Environment.NewLine}" +
                                $"Master Password: {masterPass}{Environment.NewLine}" +
                                $"Investor Password: {investorPass}{Environment.NewLine}" +
                                $"Logged at: {DateTime.Now}{Environment.NewLine}" +
                                $"-------------------------------------------------{Environment.NewLine}";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void LogSuccess_Deposit(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Success");
            string fileName = $"Success_Deposit_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }
        public static void LogError_Deposit(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Failed");
            string fileName = $"Error_Deposit_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void LogSuccess_Withdrawal(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Success");
            string fileName = $"Success_Withdrawal_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }
        public static void LogError_Withdrawal(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "Failed");
            string fileName = $"Error_Withdrawal_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void LogSuccess_MT5AccountClosed(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "MT5AccountClosed");
            string fileName = $"Success_MT5AccountClosed_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }

        public static void Log_MT5AccountClosedNoAction(string logTitle, string logContent)
        {
            string logDirectory = Path.Combine(BaseLogDirectory, "MT5AccountClosedNoAction");
            string fileName = $"MT5AccountClosedNoAction_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            WriteLog(logDirectory, fileName, logContent);
        }

    }
}
