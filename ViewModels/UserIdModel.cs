using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class UserIdModel
    {
        [Required]
        public Guid UserId { get; set; }
        public string GroupName { get; set; }
        public uint Leverage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public ulong? LoginId { get; set; }
        public decimal? Balance { get; set; }
    }
}
