using System.ComponentModel.DataAnnotations;

namespace PropTradingMT5.Helpers
{
    public class MailRequestAPI
    {
        [Required]
        public string to_email { get; set; }
        [Required]
        public string subject { get; set; }
        [Required]
        public dynamic body { get; set; }
    }
}
