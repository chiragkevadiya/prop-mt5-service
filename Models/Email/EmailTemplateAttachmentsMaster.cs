using System;
using System.ComponentModel.DataAnnotations;

namespace PropMT5ConnectionService.Models.Email
{
    public class EmailTemplateAttachmentsMaster
    {
        [Key]
        public long Id { get; set; }
        public long EmailTemplateId { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public long UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
