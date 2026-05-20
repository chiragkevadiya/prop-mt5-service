using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using PropMT5Service.Utilities;
using PropMT5Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace PropMT5Service.Services
{
    public interface IMT5AccountService
    {
        Mt5LiveAccountVM GetSingleAccount(ulong loginId);
        IEnumerable<Mt5LiveAccountVM> GetAllAccounts();
        BaseResponse<Mt5AccountCreatedVM> CreateAccount(UserIdModel model, MT5Constants.AccountCreationConfig config);
        BaseResponse ChangePassword(ulong loginId, string newPassword, CIMTUser.EnUsersPasswords passwordType);
        BaseResponse SetAccountStatus(Mt5AccountStatusVM model);
    }

    public class MT5AccountService : IMT5AccountService
    {
        private readonly CIMTManagerAPI _manager;
        private readonly MT5Constants.AccountCreationConfig _config;

        public MT5AccountService(CIMTManagerAPI manager, MT5Constants.AccountCreationConfig config = null)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _config = config ?? new MT5Constants.AccountCreationConfig();
        }

        public Mt5LiveAccountVM GetSingleAccount(ulong loginId)
        {
            return MT5AccountOperations.GetSingleAccount(_manager, loginId);
        }

        public IEnumerable<Mt5LiveAccountVM> GetAllAccounts()
        {
            return MT5AccountOperations.GetAllAccounts(_manager);
        }

        public BaseResponse<Mt5AccountCreatedVM> CreateAccount(UserIdModel model, MT5Constants.AccountCreationConfig config)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            config = config ?? _config;
            int attemptCount = 0;
            const int maxAttempts = MT5Constants.AccountOperations.MaxCreateAttempts;

            while (attemptCount < maxAttempts)
            {
                try
                {
                    // Generate login ID
                    ulong loginId = GenerateLoginId(config.LoginPrefix);
                    model.LoginId = loginId;

                    // Create user
                    var cIMTUser = CreateMT5User(model);

                    // Generate passwords
                    string masterPassword = PasswordGenerator.GenerateMasterPassword(config.MasterPasswordLength);
                    string investorPassword = PasswordGenerator.GenerateInvestorPassword(config.InvestorPasswordLength);

                    // Set user rights
                    cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_ENABLED |
                                   CIMTUser.EnUsersRights.USER_RIGHT_OTP_ENABLED |
                                   CIMTUser.EnUsersRights.USER_RIGHT_PASSWORD);

                    // Add user to MT5
                    MTRetCode retCode = _manager.UserAdd(cIMTUser, masterPassword, investorPassword);

                    // Handle retry logic for existing login
                    if (retCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                    {
                        attemptCount++;
                        continue;
                    }

                    // Get user logins for confirmation
                    ulong[] userLogins = _manager.UserLogins(model.GroupName, out MTRetCode _);

                    if (retCode == MTRetCode.MT_RET_OK)
                    {
                        var createdAccount = new Mt5AccountCreatedVM
                        {
                            UserId = model.UserId,
                            Login = model.LoginId.HasValue && model.LoginId.Value != 0 ? model.LoginId.Value : userLogins.Last(),
                            GroupName = model.GroupName,
                            MasterPassword = masterPassword,
                            InvestorPassword = investorPassword,
                            Leverage = model.Leverage,
                            ServerName = config.ServerName
                        };

                        // Log success
                        AccountLogHelper.LogSuccess(
                            model.UserId, model.GroupName, model.Leverage,
                            model.FirstName, model.LastName, model.EMail,
                            model.Phone, model.Address, model.Country,
                            createdAccount.Login, masterPassword, investorPassword, config.AccountType);

                        return new BaseResponse<Mt5AccountCreatedVM>
                        {
                            Success = true,
                            Message = "Account created successfully",
                            Data = createdAccount,
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        // Log failure
                        AccountLogHelper.LogFailed(
                            model.UserId, model.GroupName, model.Leverage,
                            model.FirstName, model.LastName, model.EMail,
                            model.Phone, model.Address, model.Country,
                            retCode, masterPassword, investorPassword, config.AccountType);

                        return new BaseResponse<Mt5AccountCreatedVM>
                        {
                            Success = false,
                            Message = GetErrorMessage(retCode),
                            StatusCode = 400
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new BaseResponse<Mt5AccountCreatedVM>
                    {
                        Success = false,
                        Message = $"Failed to create account: {ex.Message}",
                        StatusCode = 500
                    };
                }
            }

            return new BaseResponse<Mt5AccountCreatedVM>
            {
                Success = false,
                Message = $"Failed to create account after {maxAttempts} attempts",
                StatusCode = 400
            };
        }

        public BaseResponse ChangePassword(ulong loginId, string newPassword, CIMTUser.EnUsersPasswords passwordType)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty", nameof(newPassword));

            MTRetCode retCode = _manager.UserPasswordChange(passwordType, loginId, newPassword);

            if (retCode == MTRetCode.MT_RET_OK)
            {
                string passwordTypeName = passwordType == CIMTUser.EnUsersPasswords.USER_PASS_MAIN
                    ? "Trading"
                    : "Investor";

                return new BaseResponse
                {
                    Success = true,
                    Message = $"{passwordTypeName} password changed successfully",
                    StatusCode = 200
                };
            }
            else
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = GetErrorMessage(retCode),
                    StatusCode = 400
                };
            }
        }

        public BaseResponse SetAccountStatus(Mt5AccountStatusVM model)
        {
            var result = MT5AccountOperations.SetAccountActiveStatus(_manager, model);
            return new BaseResponse
            {
                Success = result.Success,
                Message = result.Message,
                StatusCode = result.StatusCode
            };
        }

        #region Private Helper Methods

        private CIMTUser CreateMT5User(UserIdModel model)
        {
            var user = _manager.UserCreate();
            user.Login(model.LoginId ?? 0);
            user.FirstName(model.FirstName ?? string.Empty);
            user.LastName(model.LastName ?? string.Empty);
            user.Leverage(model.Leverage);
            user.Group(model.GroupName);
            user.EMail(model.EMail ?? string.Empty);
            user.Phone(model.Phone ?? string.Empty);
            user.Address(model.Address ?? string.Empty);
            user.Country(model.Country ?? string.Empty);
            return user;
        }

        private ulong GenerateLoginId(string prefix)
        {
            byte[] bytes = new byte[4];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);

            uint randomNumber = BitConverter.ToUInt32(bytes, 0) % 1000;
            string loginString = prefix + randomNumber.ToString("D3");
            return ulong.Parse(loginString);
        }

        private string GetErrorMessage(MTRetCode retCode)
        {
            if (retCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                return "Login already exists";
            if (retCode == MTRetCode.MT_RET_USR_INVALID_PASSWORD)
                return "Invalid password format";
            if (retCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                return "User not found";
            if (retCode == MTRetCode.MT_RET_ERR_PARAMS)
                return "Invalid parameters provided";

            return $"Operation failed with code: {retCode}";
        }

        #endregion
    }


}
