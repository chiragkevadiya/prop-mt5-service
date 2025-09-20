using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels.GroupName
{
    public class GroupNameVM
    {
        public string GroupName { get; set; }
        public double MarginCall { get; set; }
        public double StopOutLevel { get; set; }
        public string Currency { get; set; }
        public uint CurrencyDigits { get; set; }
        public uint Instruments { get; set; }
        public int Spread { get; set; }
    }

    public class GroupListVM
    {
        public uint GroupTotal { get; set; }
        public List<GroupNameVM> GroupList { get; set; }
        public List<GroupCommissionVM> GroupCommissionList { get; set; }
    }

    public class GroupCommissionVM
    {
        public string GroupNames { get; set; }
        public double GroupCommissions { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string ChargeMode { get; set; }
    }
}
