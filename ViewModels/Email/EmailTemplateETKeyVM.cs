using PropMT5ConnectionService.Models.Email;

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
