using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service.ViewModels
{
    public class MT5CreateAccountVM
    {
        [Required]
        public Guid UserId { get; set; }
        public string GroupName { get; set; }
        public uint Leverage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public ulong? LoginId { get; set; }
        public double? DepositAmount { get; set; }
    }
}
