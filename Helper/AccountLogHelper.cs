using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MT5ConnectionService.Helper
{
    public static class AccountLogHelper
    {
        private static string _baseLogDirectory = @"C:\MT5ServicesLogSave";

        public static void SetLogDirectory(string basePath)
        {
            _baseLogDirectory = basePath;
        }

        public static void LogSuccess(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country,
            ulong login, string masterPass, string investorPass, string prefix = "")
        {
            string logDirectory = Path.Combine(_baseLogDirectory, "Success");
            string filePrefix = string.IsNullOrEmpty(prefix) ? "" : prefix + "_";
            string logFileName = $"{filePrefix}Success_log_{login}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
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
                    writer.WriteLine($"Master Password: {masterPass}");
                    writer.WriteLine($"Investor Password: {investorPass}");
                    writer.WriteLine();
                    writer.WriteLine($"Logged at: {DateTime.Now}");
                    writer.WriteLine("-------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void LogFailed(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country,
            object errorCode, string masterPass, string investorPass, string prefix = "")
        {
            string logDirectory = Path.Combine(_baseLogDirectory, "Failed");
            string filePrefix = string.IsNullOrEmpty(prefix) ? "" : prefix + "_";
            string logFileName = $"{filePrefix}Failed_log_{DateTime.Now:yyyyMMddHHmmssfff}.txt";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"UserId: {userId}");
                    writer.WriteLine($"GroupName: {groupName}");
                    writer.WriteLine($"Leverage: {leverage}");
                    writer.WriteLine($"FirstName: {firstName}");
                    writer.WriteLine($"LastName: {lastName}");
                    writer.WriteLine($"EMail: {email}");
                    writer.WriteLine($"Phone: {phone}");
                    writer.WriteLine($"Address: {address}");
                    writer.WriteLine($"Country: {country}");
                    writer.WriteLine($"Error Code: {errorCode}");
                    writer.WriteLine("-----------------Failed--------------------------");
                    writer.WriteLine($"Master Password: {masterPass}");
                    writer.WriteLine($"Investor Password: {investorPass}");
                    writer.WriteLine();
                    writer.WriteLine($"Logged at: {DateTime.Now}");
                    writer.WriteLine("-------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void LogAccountStatusChange(ulong[] loginIds, bool isEnabled)
        {
            string logDirectory = Path.Combine(_baseLogDirectory, @"Success\MT5_Account");
            string logFileName = $"Trade_{(isEnabled ? "ENABLED" : "DISABLED")}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("-----------------MT5 Account Log--------------------------");
                    writer.WriteLine($"Operation: Trade {(isEnabled ? "ENABLED" : "DISABLED")}");
                    writer.WriteLine($"Timestamp: {DateTime.Now}");
                    writer.WriteLine("Account Details: ");

                    foreach (var loginId in loginIds)
                    {
                        writer.WriteLine($"- MT5 Account No.: {loginId}");
                    }

                    writer.WriteLine("----------------------------------------------------------");
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging information: {ex.Message}");
            }
        }
    }
}
