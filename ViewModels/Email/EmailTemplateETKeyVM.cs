using PropMT5ConnectionService.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.ViewModels.Email
{
    public class EmailTemplateETKeyVM
    {
        public EmailTemplatesMaster EmailTemplatesMaster { get; set; }
        public EmailTemplatesDetailsMaster EmailTemplatesDetailsMaster { get; set; }
        public EmailTemplateAttachmentsMaster EmailTemplateAttachmentsMaster { get; set; }
        public EmailVariablesMaster EmailVariablesMaster { get; set; }
    }
}
