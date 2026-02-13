using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 mail operations
    /// </summary>
    [RoutePrefix("api/mail")]
    public class MailController : BaseApiController
    {
        public MailController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Send mail to a specific user
        /// </summary>
        [HttpPost]
        [Route("send")]
        public IHttpActionResult SendMail([FromBody] MailRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            if (request.LoginId == 0)
                return BadRequest("LoginId is required");

            if (string.IsNullOrWhiteSpace(request.Subject))
                return BadRequest("Subject is required");

            if (string.IsNullOrWhiteSpace(request.Body))
                return BadRequest("Body is required");

            return ExecuteSafe(() =>
            {
                var mail = _manager.MailCreate();
                mail.To(request.LoginId);
                mail.Subject(request.Subject);
                
                // Convert string to byte array for Body
                byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(request.Body);
                mail.Body(bodyBytes);

                var result = _manager.MailSend(mail);
                mail.Release();

                if (result == MTRetCode.MT_RET_OK)
                {
                    return new BaseResponse<object>().WithSuccess(null, "Mail sent successfully");
                }

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
            });
        }

        /// <summary>
        /// Send mail to multiple users
        /// </summary>
        [HttpPost]
        [Route("send/bulk")]
        public IHttpActionResult SendBulkMail([FromBody] BulkMailRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            if (request.LoginIds == null || request.LoginIds.Length == 0)
                return BadRequest("LoginIds array is required");

            if (string.IsNullOrWhiteSpace(request.Subject))
                return BadRequest("Subject is required");

            if (string.IsNullOrWhiteSpace(request.Body))
                return BadRequest("Body is required");

            return ExecuteSafe(() =>
            {
                int successCount = 0;
                int failCount = 0;

                foreach (var loginId in request.LoginIds)
                {
                    var mail = _manager.MailCreate();
                    mail.To(loginId);
                    mail.Subject(request.Subject);
                    
                    // Convert string to byte array for Body
                    byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(request.Body);
                    mail.Body(bodyBytes);

                    var result = _manager.MailSend(mail);
                    mail.Release();

                    if (result == MTRetCode.MT_RET_OK)
                        successCount++;
                    else
                        failCount++;
                }

                var data = new
                {
                    TotalSent = successCount,
                    TotalFailed = failCount,
                    TotalRecipients = request.LoginIds.Length
                };

                return new BaseResponse<object>().WithSuccess(data, $"Bulk mail completed. Sent: {successCount}, Failed: {failCount}");
            });
        }

        /// <summary>
        /// Send mail to all users in a group
        /// </summary>
        [HttpPost]
        [Route("send/group/{groupName}")]
        public IHttpActionResult SendGroupMail(string groupName, [FromBody] SimpleMailRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Subject))
                return BadRequest("Subject is required");

            if (string.IsNullOrWhiteSpace(request.Body))
                return BadRequest("Body is required");

            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var getUsersResult = _manager.UserAccountRequestArray(groupName, accountArray);

                if (getUsersResult != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(getUsersResult), 400);
                }

                int successCount = 0;
                int failCount = 0;
                uint total = accountArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        var mail = _manager.MailCreate();
                        mail.To(account.Login());
                        mail.Subject(request.Subject);
                        
                        // Convert string to byte array for Body
                        byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(request.Body);
                        mail.Body(bodyBytes);

                        var result = _manager.MailSend(mail);
                        mail.Release();

                        if (result == MTRetCode.MT_RET_OK)
                            successCount++;
                        else
                            failCount++;
                    }
                }

                accountArray.Release();

                var data = new
                {
                    GroupName = groupName,
                    TotalSent = successCount,
                    TotalFailed = failCount
                };

                return new BaseResponse<object>().WithSuccess(data, $"Group mail completed. Sent: {successCount}, Failed: {failCount}");
            });
        }
    }

    public class MailRequest
    {
        public ulong LoginId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class BulkMailRequest
    {
        public ulong[] LoginIds { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class SimpleMailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
