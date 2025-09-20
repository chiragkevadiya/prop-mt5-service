using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MtfiveaccountVM
    {
        public Guid UserId { get; set; }
        public ulong Login { get; set; }
        public string GroupName { get; set; }
        public string master_pass { get; set; }
        public string investor_pass { get; set; }
        public uint Leverage { get; set; }
        public string ServerName { get; set; }
    }
}
