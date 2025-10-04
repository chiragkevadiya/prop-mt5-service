using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Models.Email
{
    public class EmailVariablesMaster
    {
        [Key]
        public long Id { get; set; }
        public long EmailTemplateId { get; set; }
        public string EmailVariableName { get; set; }
        public string EVKey { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
