using System;
using System.ComponentModel.DataAnnotations;

namespace PropMT5ConnectionService.Models.Email
{
    public class EmailTemplatesMaster
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string ETKey { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
