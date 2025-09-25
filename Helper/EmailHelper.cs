//using MailKit.Net.Smtp;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using PropMT5ConnectionService.Data;
//using PropMT5ConnectionService.Helper;
//using PropTradingMT5.Data;
//using PropTradingMT5.Models.Email;
//using System;
//using System.Threading.Tasks;

//namespace PropTradingMT5.Helpers
//{
//    public class ClientEmailSetting
//    {
//        public string? Host { get; set; }
//        public int Port { get; set; }
//        public string? User { get; set; }
//        public string? Password { get; set; }
//        public string? Mail { get; set; }
//        public string? DisplayName { get; set; }
//    }
//    public class EmailHelper
//    {
//        private readonly ClientEmailSetting _emailSettings;
//        private readonly IConfiguration _configuration;
//        private readonly PropTradingDBContext _dbcontext;

//        public EmailHelper(IOptions<ClientEmailSetting> emailSettings, IConfiguration configuration, PropTradingDBContext appDbContext)
//        {
//            _emailSettings = emailSettings.Value;
//            _configuration = configuration;
//            _dbcontext = appDbContext;
//        }

//        public async Task<BaseResponse> SendEmail(string to, string subject, string body, string attachmentUrl = null, string fileName = null)
//        {
//            try
//            {
//                var smtpHost = _emailSettings.Host;
//                var smtpPort = _emailSettings.Port;
//                var smtpUser = _emailSettings.User;
//                var smtpPass = _emailSettings.Password;
//                var smtpEmail = _emailSettings.Mail;
//                var smtpDisplayName = _emailSettings.DisplayName;

//                // Create a new message  
//                var message = new MimeMessage();
//                message.From.Add(new MailboxAddress(smtpDisplayName, smtpEmail));
//                message.To.Add(new MailboxAddress("", to));
//                message.Subject = subject;

//                // Set the body part (HTML or plain text)
//                var bodyPart = new TextPart("html")
//                {
//                    Text = body
//                };

//                var multipart = new Multipart("mixed");
//                multipart.Add(bodyPart);

//                // Check if an attachment is provided
//                if (!string.IsNullOrEmpty(attachmentUrl))
//                {
//                    // Get the attachment MIME type based on the file extension
//                    var mimeType = GetMimeType(attachmentUrl);

//                    var attachment = new MimePart(mimeType)
//                    {
//                        Content = new MimeContent(File.OpenRead(Path.Combine("UploadFiles", "ApprovedKYCDocument", Path.GetFileName(attachmentUrl)))),
//                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
//                        ContentTransferEncoding = ContentEncoding.Base64,
//                        FileName = fileName ?? Path.GetFileName(attachmentUrl),
//                    };
//                    multipart.Add(attachment);
//                }

//                message.Body = multipart;

//                MailRequestLog mailrequestlog = new MailRequestLog()
//                {
//                    Email = to,
//                    Subject = subject,
//                    EmailBody = body,
//                    Status = "Sucess",
//                    RequestDate = DateTime.UtcNow,
//                };
//                _dbcontext.Add(mailrequestlog);
//                _dbcontext.SaveChanges();

//                // Connect to the SMTP server and send the email  
//                using (var client = new SmtpClient())
//                {
//                    await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

//                    await client.AuthenticateAsync(smtpUser, smtpPass);
//                    await client.SendAsync(message);
//                    await client.DisconnectAsync(true);
//                }
//                return new BaseResponse { Success = true };
//            }
//            catch (SmtpCommandException smtpEx)
//            {
//                return new BaseResponse { Success = false };
//            }
//            catch (Exception ex)
//            {
//                return new BaseResponse { Success = false };
//            }
//        }

//        private string GetMimeType(string filePath)
//        {
//            var extension = Path.GetExtension(filePath).ToLowerInvariant();
//            return extension switch
//            {
//                ".pdf" => "application/pdf",
//                ".jpg" => "image/jpeg",
//                ".jpeg" => "image/jpeg",
//                ".png" => "image/png",
//                ".gif" => "image/gif",
//                ".doc" => "application/msword",
//                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                ".xls" => "application/vnd.ms-excel",
//                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                ".txt" => "text/plain",
//                ".csv" => "text/csv",
//                _ => "application/octet-stream",
//            };
//        }

//    }
//}
