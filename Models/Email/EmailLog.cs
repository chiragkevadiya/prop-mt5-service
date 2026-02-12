using System;
using System.ComponentModel.DataAnnotations;

namespace PropMT5ConnectionService.Models.Email
{
    public class MailRequestLog
    {
        [Key]
        public long Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
