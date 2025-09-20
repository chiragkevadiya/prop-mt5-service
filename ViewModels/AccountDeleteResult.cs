using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class AccountDeleteResult
    {
        public ulong LoginId { get; set; }
        public string Status { get; set; }  // "Success", "Skipped", or "Failed"
        public string Message { get; set; }
    }
}
