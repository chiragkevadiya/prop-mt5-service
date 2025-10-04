using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Models.Email
{
    public class EmailTemplatesDetailsMaster
    {
        [Key]
        public long Id { get; set; }
        public long EmailTemplateId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Priority Priority { get; set; }
        public bool IsAttachments { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
