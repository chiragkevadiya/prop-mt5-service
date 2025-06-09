using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.ViewModels
{
    public class MT5AccountInActiveVM
    {
        public List<ulong> LoginId { get; set; }
        public bool UserStatus { get; set; }
    }
}
