using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropTradingMT5.Helpers
{
    public class EmailTemplateBuilder
    {
        private static Dictionary<string, string> replacements = new Dictionary<string, string>();

        public static void AddReplacement(string variable, string value)
        {
            if (!replacements.ContainsKey(variable))
            {
                replacements.Add(variable, value);
            }
            else
            {
                replacements[variable] = value;
            }
        }

        public static string BuildEmailBody(string template)
        {
            // Replace placeholders with actual values
            foreach (var replacement in replacements)
            {
                template = template.Replace("{{" + replacement.Key + "}}", replacement.Value)
                    .Replace("{{ " + replacement.Key + " }}", replacement.Value) // Replace with space;
                    .Replace("{{" + replacement.Key + " }}", replacement.Value) // Replace with space;
                    .Replace("{{ " + replacement.Key + "}}", replacement.Value); // Replace with space;
            }

            return template;
        }

        //public static async Task PopulateReplacementsFromDatabaseList<T>(IEnumerable<T> data, IEnumerable<EmailTemplateETKeyVM> emailVariables)
        //{
        //    if (data != null && data.Any() && data.First() != null)
        //    {
        //        // Build table rows from the list
        //        var tradeRows = new StringBuilder();

        //        foreach (var item in data)
        //        {
        //            var dealId = item.GetType().GetProperty("DealId")?.GetValue(item)?.ToString() ?? "";
        //            var symbol = item.GetType().GetProperty("Symbol")?.GetValue(item)?.ToString() ?? "";
        //            var tradeType = item.GetType().GetProperty("TradeType")?.GetValue(item)?.ToString() ?? "";
        //            var volume = item.GetType().GetProperty("TotalVolume")?.GetValue(item)?.ToString() ?? "";
        //            var openPrice = item.GetType().GetProperty("OpenPrice")?.GetValue(item)?.ToString() ?? "";
        //            var closePrice = item.GetType().GetProperty("ClosePrice")?.GetValue(item)?.ToString() ?? "";
        //            var profitLoss = item.GetType().GetProperty("ProfitLoss")?.GetValue(item)?.ToString() ?? "";

        //            tradeRows.AppendLine($"<tr><td style=\"border: 1px solid #ddd; padding: 8px;\">{dealId}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{symbol}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{tradeType}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{volume}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{openPrice}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{closePrice}</td>" +
        //                $"<td style=\"border: 1px solid #ddd; padding: 8px;\">{profitLoss}</td></tr>");
        //        }

        //        AddReplacement("TradeTable", tradeRows.ToString());

        //        var firstItem = data.First();
        //        foreach (var emailVariable in emailVariables)
        //        {
        //            string key = emailVariable.EmailVariablesMaster.EVKey;

        //            // Skip table-row keys to avoid overwriting full table
        //            if (key == "DealId" || key == "Symbol" || key == "TradeType" || key == "TotalVolume" || key == "OpenPrice" || key == "ClosePrice" || key == "ProfitLoss")
        //                continue;

        //            var value = firstItem.GetType().GetProperty(key)?.GetValue(firstItem, null)?.ToString();

        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                AddReplacement(key, value);
        //            }
        //        }

        //    }
        //}

        //public static async Task PopulateReplacementsFromDatabase<T>(IEnumerable<T> data, IEnumerable<EmailTemplateETKeyVM> emailVariables)
        //{
        //    if (data != null && data.Any() && data.First() != null)
        //    {
        //        foreach (var emailVariable in emailVariables)
        //        {
        //            string key = emailVariable.EmailVariablesMaster.EVKey;

        //            foreach (var item in data)
        //            {
        //                var value = item.GetType().GetProperty(key)?.GetValue(item, null)?.ToString();

        //                if (value != null)
        //                {
        //                    AddReplacement(key, value);
        //                    //if (key == "Password" || key == "MasterPassword" || key == "InvestorPassword" || key == "InvestPassword")
        //                    //{
        //                    //    AddReplacement(key, EncryptionHelper.DecryptString(value));
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //}

        //public static async Task ReplacementsStaticValueEmail(List<EmailTemplatesStaticKeyValueMaster> data)
        //{
        //    if (data.Any())
        //    {
        //        foreach (var item in data)
        //        {
        //            AddReplacement(item.EmailVariableKey, item.EmailVariableValue);
        //        }
        //    }

        //}
    }
}
