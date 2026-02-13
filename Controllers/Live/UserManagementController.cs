using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for MT5 user management operations
    /// </summary>
    [RoutePrefix("api/users")]
    public class UserManagementController : BaseApiController
    {
        public UserManagementController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllUsers()
        {
            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var result = _manager.UserAccountRequestArray("*", accountArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var users = new List<object>();
                uint total = accountArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        var user = _manager.UserCreate();
                        if (_manager.UserGet(account.Login(), user) == MTRetCode.MT_RET_OK)
                        {
                            users.Add(new
                            {
                                Login = user.Login(),
                                Group = user.Group(),
                                Name = user.Name(),
                                Country = user.Country(),
                                City = user.City(),
                                State = user.State(),
                                Address = user.Address(),
                                Phone = user.Phone(),
                                ID = user.ID(),
                                Status = user.Status(),
                                Comment = user.Comment(),
                                Color = user.Color(),
                                PhonePassword = user.PhonePassword(),
                                Leverage = user.Leverage(),
                                Agent = user.Agent(),
                                Balance = account.Balance(),
                                Credit = account.Credit(),
                                InterestRate = user.InterestRate(),
                                CommissionDaily = user.CommissionDaily(),
                                CommissionMonthly = user.CommissionMonthly(),
                                BalancePrevDay = user.BalancePrevDay(),
                                BalancePrevMonth = user.BalancePrevMonth(),
                                EquityPrevDay = user.EquityPrevDay(),
                                EquityPrevMonth = user.EquityPrevMonth(),
                                Rights = user.Rights(),
                                Registration = DateTimeOffset.FromUnixTimeSeconds(user.Registration()).LocalDateTime,
                                LastAccess = user.LastAccess() > 0 ? DateTimeOffset.FromUnixTimeSeconds(user.LastAccess()).LocalDateTime : (DateTime?)null,
                                LastIP = user.LastIP(),
                                LeadSource = user.LeadSource(),
                                LeadCampaign = user.LeadCampaign()
                            });
                        }
                        user.Release();
                    }
                }

                accountArray.Release();
                return new BaseResponse<object>().WithSuccess(users, $"Retrieved {users.Count} users");
            });
        }

        /// <summary>
        /// Get user by login
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetUser(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var user = _manager.UserCreate();
                var result = _manager.UserGet((ulong)loginId, user);

                if (result != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var userData = new
                {
                    Login = user.Login(),
                    Group = user.Group(),
                    CertSerialNumber = user.CertSerialNumber(),
                    Rights = user.Rights(),
                    Registration = DateTimeOffset.FromUnixTimeSeconds(user.Registration()).LocalDateTime,
                    LastAccess = user.LastAccess() > 0 ? DateTimeOffset.FromUnixTimeSeconds(user.LastAccess()).LocalDateTime : (DateTime?)null,
                    LastPassChange = user.LastPassChange() > 0 ? DateTimeOffset.FromUnixTimeSeconds(user.LastPassChange()).LocalDateTime : (DateTime?)null,
                    LastIP = user.LastIP(),
                    Name = user.Name(),
                    FirstName = user.FirstName(),
                    LastName = user.LastName(),
                    MiddleName = user.MiddleName(),
                    Country = user.Country(),
                    Language = user.Language(),
                    City = user.City(),
                    State = user.State(),
                    Address = user.Address(),
                    Phone = user.Phone(),
                    ID = user.ID(),
                    Status = user.Status(),
                    Comment = user.Comment(),
                    Color = user.Color(),
                    PhonePassword = user.PhonePassword(),
                    Leverage = user.Leverage(),
                    Agent = user.Agent(),
                    Balance = user.Balance(),
                    Credit = user.Credit(),
                    InterestRate = user.InterestRate(),
                    CommissionDaily = user.CommissionDaily(),
                    CommissionMonthly = user.CommissionMonthly(),
                    CommissionAgentDaily = user.CommissionAgentDaily(),
                    CommissionAgentMonthly = user.CommissionAgentMonthly(),
                    BalancePrevDay = user.BalancePrevDay(),
                    BalancePrevMonth = user.BalancePrevMonth(),
                    EquityPrevDay = user.EquityPrevDay(),
                    EquityPrevMonth = user.EquityPrevMonth(),
                    LeadSource = user.LeadSource(),
                    LeadCampaign = user.LeadCampaign(),
                    MQID = user.MQID()
                };

                user.Release();
                return new BaseResponse<object>().WithSuccess(userData, "User retrieved successfully");
            });
        }

        /// <summary>
        /// Update user information
        /// </summary>
        [HttpPut]
        [Route("{loginId:long}")]
        public IHttpActionResult UpdateUser(long loginId, [FromBody] UserUpdateRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            return ExecuteSafe(() =>
            {
                var user = _manager.UserCreate();
                var result = _manager.UserGet((ulong)loginId, user);

                if (result != MTRetCode.MT_RET_OK)
                {
                    user.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(request.Name))
                    user.Name(request.Name);
                if (!string.IsNullOrEmpty(request.Phone))
                    user.Phone(request.Phone);
                if (!string.IsNullOrEmpty(request.Country))
                    user.Country(request.Country);
                if (!string.IsNullOrEmpty(request.City))
                    user.City(request.City);
                if (!string.IsNullOrEmpty(request.State))
                    user.State(request.State);
                if (!string.IsNullOrEmpty(request.Address))
                    user.Address(request.Address);
                if (!string.IsNullOrEmpty(request.Comment))
                    user.Comment(request.Comment);
                if (request.Color.HasValue)
                    user.Color(request.Color.Value);
                if (request.Leverage.HasValue)
                    user.Leverage(request.Leverage.Value);
                if (request.Agent.HasValue)
                    user.Agent(request.Agent.Value);

                var updateResult = _manager.UserUpdate(user);
                user.Release();

                if (updateResult == MTRetCode.MT_RET_OK)
                {
                    return new BaseResponse<object>().WithSuccess(null, "User updated successfully");
                }

                return new BaseResponse<object>().WithError(GetMT5ErrorMessage(updateResult), 400);
            });
        }

        /// <summary>
        /// Get users by group
        /// </summary>
        [HttpGet]
        [Route("group/{groupName}")]
        public IHttpActionResult GetUsersByGroup(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var result = _manager.UserAccountRequestArray(groupName, accountArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var users = new List<object>();
                uint total = accountArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        var user = _manager.UserCreate();
                        if (_manager.UserGet(account.Login(), user) == MTRetCode.MT_RET_OK)
                        {
                            users.Add(new
                            {
                                Login = user.Login(),
                                Group = user.Group(),
                                Name = user.Name(),
                                Balance = account.Balance(),
                                Credit = account.Credit(),
                                Leverage = user.Leverage(),
                                Registration = DateTimeOffset.FromUnixTimeSeconds(user.Registration()).LocalDateTime
                            });
                        }
                        user.Release();
                    }
                }

                accountArray.Release();
                return new BaseResponse<object>().WithSuccess(users, $"Retrieved {users.Count} users from group '{groupName}'");
            });
        }

        /// <summary>
        /// Check if login exists
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}/exists")]
        public IHttpActionResult CheckLoginExists(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var user = _manager.UserCreate();
                var result = _manager.UserGet((ulong)loginId, user);
                user.Release();

                var exists = result == MTRetCode.MT_RET_OK;
                return new BaseResponse<object>().WithSuccess(new { Exists = exists }, exists ? "Login exists" : "Login does not exist");
            });
        }

        /// <summary>
        /// Get user logins by name (search)
        /// </summary>
        [HttpGet]
        [Route("search")]
        public IHttpActionResult SearchUsers(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter is required");

            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var result = _manager.UserAccountRequestArray("*", accountArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var users = new List<object>();
                uint total = accountArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        var user = _manager.UserCreate();
                        if (_manager.UserGet(account.Login(), user) == MTRetCode.MT_RET_OK)
                        {
                            if (user.Name().IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                users.Add(new
                                {
                                    Login = user.Login(),
                                    Name = user.Name(),
                                    Group = user.Group()
                                });
                            }
                        }
                        user.Release();
                    }
                }

                accountArray.Release();
                return new BaseResponse<object>().WithSuccess(users, $"Found {users.Count} users matching '{name}'");
            });
        }
    }

    public class UserUpdateRequest
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public uint? Color { get; set; }
        public uint? Leverage { get; set; }
        public ulong? Agent { get; set; }
    }
}
