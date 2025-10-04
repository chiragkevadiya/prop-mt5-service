using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Models.Email
{
    public class EmailTemplatesStaticKeyValueMaster
    {
        [Key]
        public long Id { get; set; }
        public long EmailTemplateId { get; set; }
        public string EmailVariableKey { get; set; }
        public string EmailVariableValue { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
