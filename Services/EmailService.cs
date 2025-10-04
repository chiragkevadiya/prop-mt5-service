using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using PropMT5ConnectionService.Data;
using PropMT5ConnectionService.Helper;
using PropMT5ConnectionService.Models.Email;
using PropMT5ConnectionService.ViewModels.Email;
using PropTradingMT5.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task<bool> SendEmailAPI(string to_email, string subject, string body);
        Task<bool> SendEmailAsync<T>(string ETKey, string EmailId, IEnumerable<T> LinqQuery = null, string attachmentUrl = null);
        Task<bool> SendEmailAsyncList<T>(string ETKey, string EmailId, IEnumerable<T> LinqQuery = null);
        Task<TViewModel> GetEmail<TViewModel>(TViewModel entity, Expression<Func<TViewModel, bool>> expression) where TViewModel : class;
    }


    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly PropTradingDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly EmailHelper _emailHelper;

        public EmailService(IOptions<MailSettings> mailSettings, PropTradingDBContext dbContext, IConfiguration configuration, EmailHelper emailHelper)
        {
            _mailSettings = mailSettings.Value;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailHelper = emailHelper;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();
                if (mailRequest.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in mailRequest.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
                        }
                    }
                }
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();
                var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.CheckCertificateRevocation = false;
                smtp.Disconnect(true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailAPI(string to_email, string subject, string body)
        {
            try
            {
                // Create MailRequestAPI object
                MailRequestAPI mailRequestAPI = new MailRequestAPI()
                {
                    to_email = to_email,
                    subject = subject,
                    body = body
                };

                // Serialize MailRequestAPI object to JSON
                string jsonBody = JsonConvert.SerializeObject(mailRequestAPI);

                // Create HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Create StringContent with JSON body
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(_mailSettings.EmailApiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Optionally, you can process the response here
                        // For example, you might want to read response content:
                        // string responseContent = await response.Content.ReadAsStringAsync();

                        MailRequestLog mailrequestlog = new MailRequestLog()
                        {
                            Email = to_email,
                            Subject = subject,
                            EmailBody = null,
                            Status = "Sucess",
                            RequestDate = DateTime.UtcNow,
                        };
                        _dbContext.Add(mailrequestlog);
                        _dbContext.SaveChanges();

                        return true;
                    }
                    else
                    {
                        // Handle unsuccessful request
                        // You might want to log the response content or throw an exception

                        MailRequestLog mailrequestlog = new MailRequestLog()
                        {
                            Email = to_email,
                            Subject = subject,
                            EmailBody = null,
                            Status = "Fail",
                            RequestDate = DateTime.UtcNow,
                        };
                        _dbContext.Add(mailrequestlog);
                        _dbContext.SaveChanges();

                        return false;
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions
                // Log or throw the exception based on your requirements
                return false;
            }
        }

        public async Task<bool> SendEmailAsync<T>(string ETKey, string ToEmailId, IEnumerable<T> LinqQuery = null, string attachmentUrl = null)
        {
            try
            {
                var emailTemplate = await GetEmailTemplateByETKey(ETKey);

                if (emailTemplate == null || emailTemplate.Count <= 0)
                {
                    return false;
                }

                if (emailTemplate.Count > 0 && (emailTemplate[0].EmailTemplatesDetailsMaster == null || emailTemplate[0].EmailTemplatesMaster == null))
                {
                    return false;
                }

                string emailBody = emailTemplate[0].EmailTemplatesDetailsMaster.Body;
                string emailSubject = emailTemplate[0].EmailTemplatesDetailsMaster.Subject;

                if (LinqQuery != null && LinqQuery.First() != null)
                {
                    await EmailTemplateBuilder.PopulateReplacementsFromDatabase(
                         LinqQuery,
                         emailTemplate
                     );
                }

                // add and replaced Email Templates Static Key Value Master
                List<EmailTemplatesStaticKeyValueMaster> emailStaticKeyValue = await _dbContext.EmailTemplatesStaticKeyValueMaster
                                            .Where(x => x.EmailTemplateId == emailTemplate[0].EmailTemplatesDetailsMaster.EmailTemplateId)
                                            .ToListAsync();

                if (emailStaticKeyValue.Any())
                {
                    await EmailTemplateBuilder.ReplacementsStaticValueEmail(emailStaticKeyValue);
                }

                // Now emailBody contains the email template with dynamic values replaced

                string emailBodyreplaced = EmailTemplateBuilder.BuildEmailBody(emailBody);

                string html = $@"<!DOCTYPE html>
<html>

<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>PropTrading</title>
    <!--[if mso]>
            <noscript>
                <xml>
                    <o:OfficeDocumentSettings>
                        <o:PixelsPerInch>96</o:PixelsPerInch>
                    </o:OfficeDocumentSettings>
                </xml>
            </noscript>
        <![endif]-->
    <style type=""text/css"">
        /* Mobile Responsive Styles */
        @media only screen and (max-width: 600px) {{
            .email-container {{
                width: 100% !important;
                max-width: 100% !important;
            }}

            .mobile-padding {{
                padding: 16px 12px !important;
            }}

            .mobile-text {{
                font-size: 14px !important;
            }}

            .mobile-title {{
                font-size: 20px !important;
            }}

            .mobile-button {{
                padding: 10px 24px !important;
                font-size: 14px !important;
            }}

            .mobile-logo {{
                width: 120px !important;
            }}

            .mobile-social {{
                width: 20px !important;
                height: 20px !important;
            }}

            .mobile-disclaimer {{
                font-size: 9px !important;
            }}
        }}
    </style>
</head>

<body style=""margin: 0; padding: 0; background-color: #f3f4f6; font-family: Arial, Helvetica, sans-serif;"">
    <!-- Main Container -->
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%""
        style=""background-color: #f3f4f6;"">
        <tr>
            <td align=""center"" style=""padding: 20px 0;"">
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""600""
                    style=""max-width: 600px; background-color: #ffffff; box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);""
                    class=""email-container"">
                    <!-- Email Header -->
                    <tr>
                        <td style=""background-color: #242424; padding: 24px 20px; border-bottom: 1px solid #4b5563;""
                            class=""mobile-padding"">
                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                <tr>
                                    <td align=""left"">
                                        <img src=""https://prop-cabinet.neptunefxcrm.com/assets/logo/logo.png""
                                            alt=""PropTrading Logo"" style=""width:152px; height:50px; display: block; border: 0;""
                                            class=""mobile-logo"" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                  {emailBodyreplaced}
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #18191a; padding: 32px 20px;"" class=""mobile-padding"">
                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
                                <tr>
                                    <td align=""center"">
                                        <!-- Social Media Icons -->
                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0""
                                            style=""margin-bottom: 16px;"">
                                            <tr>
                                                <td style=""padding: 0 12px;"">
                                                    <a href=""https://cdn.tools.unlayer.com/social/icons/squared/facebook.png"" style=""text-decoration: none;"">
                                                        <img src=""https://cdn.tools.unlayer.com/social/icons/squared/facebook.png""
                                                            alt=""Facebook""
                                                            style=""width:24px; height:24px;display: block; border: 0;"" class=""mobile-social"" />
                                                    </a>
                                                </td>
                                                <td style=""padding: 0 12px;"">
                                                    <a href=""$$instagramURL$$"" style=""text-decoration: none;"">
                                                        <img src=""https://cdn.tools.unlayer.com/social/icons/squared/instagram.png""
                                                            alt=""Instagram"" width=""24"" height=""24""
                                                            style=""width:24px; height:24px;display: block; border: 0;"" class=""mobile-social"" />
                                                    </a>
                                                </td>
                                                <td style=""padding: 0 12px;"">
                                                    <a href=""https://cdn.tools.unlayer.com/social/icons/squared/youtube.png"" style=""text-decoration: none;"">
                                                        <img src=""https://cdn.tools.unlayer.com/social/icons/squared/youtube.png""
                                                            alt=""You Tube"" width=""24"" height=""24""
                                                            style=""width:24px; height:24px;display: block; border: 0;"" class=""mobile-social"" />
                                                    </a>
                                                </td>
                                                <td style=""padding: 0 12px;"">
                                                    <a href=""https://cdn.tools.unlayer.com/social/icons/squared/telegram.png"" style=""text-decoration: none;"">
                                                        <img src=""https://cdn.tools.unlayer.com/social/icons/squared/telegram.png""
                                                            alt=""Telegram"" width=""24"" height=""24""
                                                            style=""width:24px; height:24px;display: block; border: 0;"" class=""mobile-social"" />
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- Footer Links -->
                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0""
                                            width=""100%""
                                            style=""border-top: 1px solid #374151; padding-top: 16px; margin-top: 16px;"">
                                            <tr>
                                                <td align=""center"">
                                                    <table role=""presentation"" cellspacing=""0"" cellpadding=""0""
                                                        border=""0"">
                                                        <tr>
                                                            <td align=""center"" colspan=""5"">
                                                                <p
                                                                    style=""color: #9ca3af; font-size: 14px; margin: 0 0 8px 0; font-family: Arial, Helvetica, sans-serif;"">
                                                                    <strong style=""color: #ffffff;"">Neptune-Prop</strong>
                                                                </p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align=""center"">
                                                                <p
                                                                    style=""color: #6b7280; font-size: 12px; margin: 0; font-family: Arial, Helvetica, sans-serif;"">
                                                                    &copy; 2025 Neptune-Prop. All rights
                                                                    reserved.
                                                                </p>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <!-- Legal Disclaimers -->
                                        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0""
                                            width=""100%""
                                            style=""border-top: 1px solid #374151; padding-top: 16px; margin-top: 16px;"">
                                            <tr>
                                                <td align=""center"">
                                                    <p style=""color: #6b7280; font-size: 10px; line-height: 1.4; margin: 0 0 12px 0; font-family: Arial, Helvetica, sans-serif;""
                                                        class=""mobile-disclaimer"">
                                                        <strong>Risk Warning:</strong> Trading financial instruments
                                                        carries a significant risk, and you may lose the entire amount
                                                        invested in the market. We strongly recommend trading
                                                        only with funds that you can afford to lose. Trading in
                                                        financial markets, particularly in forex and CFDs, may not be
                                                        suitable for all investors. We encourage you to carefully
                                                        assess the risks involved in trading and to seek professional
                                                        advice before committing real money. We urge you to read and
                                                        understand our Risk Disclosure to gain a comprehensive
                                                        understanding of the potential risks involved in trading.
                                                    </p>
                                                    <p style=""color: #6b7280; font-size: 10px; line-height: 1.4; margin: 0 0 12px 0; font-family: Arial, Helvetica, sans-serif;""
                                                        class=""mobile-disclaimer"">
                                                        <strong>Age Restriction:</strong> Neptune-Prop does not
                                                        accept clients under the age of 18 or below the legal age as
                                                        determined by your country. By registering or opening
                                                        an account with Neptune-Prop, you confirm your agreement
                                                        with our policies and provide your free consent to use our
                                                        services. You must be at least 18 years old or of legal
                                                        age as per your country's regulations. By registering an account
                                                        with Neptune-Prop, you confirm that you are doing so
                                                        voluntarily and without any solicitation on our part.
                                                    </p>
                                                    <p style=""color: #6b7280; font-size: 10px; line-height: 1.4; margin: 0; font-family: Arial, Helvetica, sans-serif;""
                                                        class=""mobile-disclaimer"">
                                                        <strong>Jurisdictional Restrictions:</strong> We do not offer
                                                        our services to individuals in countries where local laws
                                                        prohibit such services or products. It is your
                                                        responsibility to ensure that the use of our services and
                                                        products complies with the laws and regulations of your country.
                                                        Additionally, the information on our website may not be
                                                        applicable or accurate in all jurisdictions. Our services are
                                                        not available to residents of certain jurisdictions, including
                                                        Afghanistan, Cuba, Iran, Libya, Myanmar, North Korea,
                                                        Sudan, Puerto Rico, USA, Syria, and Yemen. Please review our
                                                        Restricted Countries List before proceeding.
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                      
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";



                if (Convert.ToBoolean(_configuration["LiveEmail"]))
                {
                    BaseResponse result = await _emailHelper.SendEmail(ToEmailId, emailSubject, html, attachmentUrl);

                    return result != null ? result.Success : false;
                }
                else
                {
                    bool sendResult = await SendEmailCallAPI(ToEmailId, emailSubject, html);
                    return sendResult;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> SendEmailCallAPI(string to_email, string subject, string body)
        {
            try
            {
                var mailRequestContent = new
                {
                    to_email,
                    subject,
                    body
                };

                // Serialize MailRequestAPI object to JSON
                string jsonBody = JsonConvert.SerializeObject(mailRequestContent);

                // Create HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Create StringContent with JSON body
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(_configuration["EmailApiUrl"], content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Optionally, you can process the response here
                        // For example, you might want to read response content:
                        // string responseContent = await response.Content.ReadAsStringAsync();

                        return true;
                    }
                    else
                    {
                        // Handle unsuccessful request
                        // You might want to log the response content or throw an exception
                        return false;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<EmailTemplateETKeyVM>> GetEmailTemplateByETKey(string etKey)
        {
            var emailTemplate = await (from etm in _dbContext.EmailTemplatesMasters
                                       join etdm in _dbContext.EmailTemplatesDetailsMaster
                                            on etm.Id equals etdm.EmailTemplateId
                                       join etam in _dbContext.EmailTemplateAttachmentsMaster
                                            on etdm.EmailTemplateId equals etam.EmailTemplateId into etamGroup
                                       from etam in etamGroup.DefaultIfEmpty()
                                       join evm in _dbContext.EmailVariablesMaster
                                            on etm.Id equals evm.EmailTemplateId into evmGroup
                                       from evm in evmGroup.DefaultIfEmpty()
                                       where etm.ETKey == etKey
                                       select new EmailTemplateETKeyVM
                                       {
                                           EmailTemplatesMaster = etm,
                                           EmailTemplatesDetailsMaster = etdm,
                                           EmailTemplateAttachmentsMaster = etam,
                                           EmailVariablesMaster = evm,
                                       })
                               .AsNoTracking()
                               .ToListAsync();
            return emailTemplate;
        }

        public async Task<TViewModel> GetEmail<TViewModel>(TViewModel entity, Expression<Func<TViewModel, bool>> expression) where TViewModel : class
        {
            return await _dbContext.Set<TViewModel>().Where(expression).FirstOrDefaultAsync();
        }

        public async Task<bool> SendEmailAsyncList<T>(string ETKey, string ToEmailId, IEnumerable<T> LinqQuery = null)
        {
            try
            {
                var emailTemplate = await GetEmailTemplateByETKey(ETKey);

                if (emailTemplate == null || emailTemplate.Count <= 0)
                {
                    return false;
                }

                if (emailTemplate.Count > 0 && (emailTemplate[0].EmailTemplatesDetailsMaster == null || emailTemplate[0].EmailTemplatesMaster == null))
                {
                    return false;
                }

                string emailBody = emailTemplate[0].EmailTemplatesDetailsMaster.Body;
                string emailSubject = emailTemplate[0].EmailTemplatesDetailsMaster.Subject;

                if (LinqQuery != null && LinqQuery.First() != null)
                {
                    await EmailTemplateBuilder.PopulateReplacementsFromDatabaseList(
                         LinqQuery,
                         emailTemplate
                     );
                }

                List<EmailTemplatesStaticKeyValueMaster> emailStaticKeyValue = await _dbContext.EmailTemplatesStaticKeyValueMaster
                                            .Where(x => x.EmailTemplateId == emailTemplate[0].EmailTemplatesDetailsMaster.EmailTemplateId)
                                            .ToListAsync();

                if (emailStaticKeyValue.Any())
                {
                    await EmailTemplateBuilder.ReplacementsStaticValueEmail(emailStaticKeyValue);
                }

                string emailBodyreplaced = EmailTemplateBuilder.BuildEmailBody(emailBody);

                bool sendResult = await SendEmailCallAPI(ToEmailId, emailSubject, emailBodyreplaced);

                return sendResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
