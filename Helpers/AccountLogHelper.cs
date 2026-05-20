using PropMT5Service.Constants;
using System;
using System.IO;

namespace PropMT5Service.Helpers
{
    public static class AccountLogHelper
    {
        // ──────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────

        public static void LogSuccess(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country,
            ulong login, string masterPass, string investorPass, string prefix = "")
        {
            string filePrefix = BuildFilePrefix(prefix, "Success", login.ToString());
            string body = BuildAccountBody(userId, groupName, leverage, firstName, lastName,
                email, phone, address, country)
                + $"MT5 Account No.: {login}{Environment.NewLine}"
                + $"Master Password: {masterPass}{Environment.NewLine}"
                + $"Investor Password: {investorPass}{Environment.NewLine}";

            WriteLog(MT5Constants.Logging.SuccessPath, filePrefix, body);
        }

        public static void LogFailed(Guid userId, string groupName, uint leverage, string firstName,
            string lastName, string email, string phone, string address, string country,
            object errorCode, string masterPass, string investorPass, string prefix = "")
        {
            string filePrefix = BuildFilePrefix(prefix, "Failed", null);
            string body = BuildAccountBody(userId, groupName, leverage, firstName, lastName,
                email, phone, address, country)
                + $"Error Code: {errorCode}{Environment.NewLine}"
                + $"Master Password: {masterPass}{Environment.NewLine}"
                + $"Investor Password: {investorPass}{Environment.NewLine}";

            WriteLog(MT5Constants.Logging.FailedPath, filePrefix, body);
        }

        public static void LogReconnect(bool success, string retCode = null, Exception exception = null)
        {
            string outcome = success ? "SUCCESS" : "FAILED";
            string fileName = $"Reconnect_{outcome}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            var body = new System.Text.StringBuilder();
            body.AppendLine($"Event    : Scheduled MT5 Reconnect");
            body.AppendLine($"Outcome  : {outcome}");
            body.AppendLine($"Time     : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            if (!string.IsNullOrEmpty(retCode))
                body.AppendLine($"RetCode  : {retCode}");

            if (exception != null)
            {
                body.AppendLine($"Exception: {exception.Message}");
                body.AppendLine($"StackTrace: {exception.StackTrace}");
            }

            WriteLog(MT5Constants.Logging.ReconnectPath, fileName, body.ToString());
        }

        public static void LogAccountStatusChange(ulong[] loginIds, bool isEnabled)
        {
            string status = isEnabled ? "ENABLED" : "DISABLED";
            string logDirectory = Path.Combine(MT5Constants.Logging.SuccessPath, "MT5_Account");
            string fileName = $"Trade_{status}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";

            var body = new System.Text.StringBuilder();
            body.AppendLine($"Operation: Trade {status}");
            body.AppendLine("Account Details:");
            foreach (var loginId in loginIds)
                body.AppendLine($"  - MT5 Account No.: {loginId}");

            WriteLog(logDirectory, fileName, body.ToString(), appendSeparator: true);
        }

        // ──────────────────────────────────────────────────────────────
        // Private helpers
        // ──────────────────────────────────────────────────────────────

        private static string BuildFilePrefix(string prefix, string outcome, string loginSuffix)
        {
            string p = string.IsNullOrEmpty(prefix) ? "" : prefix + "_";
            string l = loginSuffix != null ? $"_{loginSuffix}" : "";
            return $"{p}{outcome}_log{l}_{DateTime.Now:yyyyMMddHHmmssfff}.txt";
        }

        private static string BuildAccountBody(Guid userId, string groupName, uint leverage,
            string firstName, string lastName, string email,
            string phone, string address, string country)
        {
            return $"UserId: {userId}{Environment.NewLine}"
                 + $"GroupName: {groupName}{Environment.NewLine}"
                 + $"Leverage: {leverage}{Environment.NewLine}"
                 + $"FirstName: {firstName}{Environment.NewLine}"
                 + $"LastName: {lastName}{Environment.NewLine}"
                 + $"EMail: {email}{Environment.NewLine}"
                 + $"Phone: {phone}{Environment.NewLine}"
                 + $"Address: {address}{Environment.NewLine}"
                 + $"Country: {country}{Environment.NewLine}";
        }

        private static void WriteLog(string directory, string fileName, string body,
            bool appendSeparator = false)
        {
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string path = Path.Combine(directory, fileName);

                using (StreamWriter writer = File.AppendText(path))
                {
                    writer.WriteLine("-------------------------------------------");
                    writer.Write(body);
                    if (appendSeparator)
                        writer.WriteLine("----------------------------------------------------------");
                    writer.WriteLine($"Logged at: {DateTime.Now}");
                    writer.WriteLine("-------------------------------------------");
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
