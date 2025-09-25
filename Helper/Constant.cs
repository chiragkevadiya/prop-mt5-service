using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Helper
{
    public class Constant
    {

        // enum
        public enum RequestStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Approved")]
            Approved = 2,
            [Description("Declined")]
            Declined = 3,
            [Description("Not Applied")]
            NotApplied = 0
        }

        public enum DropDownType
        {
            Priority,
            TutorialType,
            WithdrawType,
            KYCDocumentType,
            RuleType,
            RuleMandatoryModules
        }
        public enum TradingAccountType
        {
            [Description("Live")]
            Live = 0,
            [Description("Demo")]
            Demo = 1
        }

        public enum InternalTransferType
        {
            [Description("ChallengePurchased")]
            ChallengePurchase = 1,
            [Description("UserProfitShare")]
            UserProfitShare = 2,
            [Description("AdminProfitShare")]
            AdminProfitShare = 3,
        }
        public enum RuleType
        {
            [Description("KYC")]
            KYC = 1,
            [Description("Deposit")]
            Deposit = 2,
            [Description("Withdraw")]
            Withdraw = 3,
            [Description("Challenge")]
            Challenge = 4,
            [Description("Account")]
            Account = 5,

        }
        public enum RuleLevel
        {
            [Description("System")]
            System = 1
        }
        public enum RuleMandatoryFor
        {
            [Description("Deposit")]
            Deposit = 1,

            [Description("Withdraw")]
            Withdraw = 2,

            [Description("Account")]
            Account = 3,
        }

        public enum TicketStatus
        {
            [Description("Open")]
            Open,
            [Description("InProgress")]
            InProgress,
            [Description("Closed")]
            Closed,
            [Description("Assigned")]
            Assigned
        }

        public enum LeadStatus
        {
            [Description("New")]
            New,
            [Description("Attended")]
            Attended,
            [Description("Progress")]
            Progress,
            [Description("Positive")]
            Positive,
            [Description("Won")]
            Won,
            [Description("Lost")]
            Lost
        }
        public enum BannerViewType
        {
            [Description("Web")]
            Web = 1,

            [Description("Mobile")]
            Mobile = 2
        }

        public enum BannerType
        {
            [Description("User")]
            User = 2
        }
        public enum WithdrawType
        {
            [Description("BankTransfer")]
            BankTransfer = 1,
            //[Description("UPI")]
            //UPI = 2,
            //[Description("PayPal")]
            //Paypal = 3,
            [Description("Crypto")]
            Crypto = 2
        }

        public enum Priority
        {

            [Description("Low")]
            Low = 1,
            [Description("High")]
            High = 2,
            [Description("Medium")]
            Medium = 3
        }
        public enum TutorialType
        {
            [Description("Dashboard")]
            Dashboard = 1,
            [Description("Inbox")]
            Inbox = 2,
            [Description("Accounts")]
            Accounts = 3,
            [Description("Deposit")]
            Deposit = 4,
            [Description("Withdraw")]
            Withdraw = 5,
            [Description("Transfer")]
            Transfer = 6,
            [Description("Transactions")]
            Transactions = 7,
            [Description("Tree")]
            Tree = 8,
            [Description("Support")]
            Support = 9,
            [Description("Trading")]
            Trading = 10,
            [Description("IBDashboard")]
            IBDashboard = 11,
        }
        public enum ReferralType
        {
            [Description("AFF001")]
            AffiliateCode,
            [Description("REF001")]
            ReferralCode
        }

        public enum RoleType
        {
            Admin = 1,      // Administrator with full access
            SubAdmin = 2,   // Sub administrator with limit access
            User = 3,       // Regular user/trader
            IBUser = 4      // Introducing Broker (IB) role
        }

        public enum PropAccountType
        {
            [Description("NormalAccountType")]
            NormalAccountType = 1,
            [Description("CoverageAccountType")]
            CoverageAccountType = 2
        }

        public enum KYCDocumentType
        {
            [Description("Passport")]
            Passport = 1,
            [Description("DrivingLicence")]
            DrivingLicence = 2,
            [Description("NationalId")]
            NationalId = 3,
            [Description("UtilityBill")]
            UtilityBill = 4,
            [Description("BankStatement")]
            BankStatement = 5
        }

        public enum KYCType
        {
            Manual,
            Automated
        }

        public enum ClientType
        {
            ClientType = 1,
            OfficeType = 2,
            GroupType = 3
        }

        public enum LoginType
        {
            [Description("Web")]
            Web = 1,
            [Description("Mobile")]
            Mobile = 2,
        }

        public enum TwoFactorStatus
        {
            [Description("Disabled")]
            Disabled = 0,
            [Description("Enabled")]
            Enabled = 1,
        }

        public enum TradingTerm
        {
            Buy = 1,
            Sell = -1
        }

        public enum MoneyTransctionType
        {
            [Description("Withdrawal")]
            Withdrawal = -1,
            [Description("Deposit")]
            Deposit = 1
        }

        public enum DepositType
        {
            [Description("CashDeposit")]
            CashDeposit = 1,
            [Description("Admin")]
            Admin = 2,
        }

        public enum PaymentRequestStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Approved")]
            Approved = 2,
            [Description("Rejected")]
            Rejected = 3,
        }
        public enum TransactionStatus
        {
            Success = 1,
            Failed = 2,
            Reject = 3
        }

        public enum PaymentGatewayStatus
        {
            Pending = 1,
            Processing = 2,
            Completed = 3, // SUCCESS
            Failed = 4,
            Expired = 5
        }

        public enum PaymentStatus
        {
            Success,
            Failed
        }
        public enum ChallengePhaseStatus
        {
            Pending = 0,
            Active = 1,
            Success = 2,
            Drawdown = 3,
        }

        public enum ChallengeStatus
        {
            Active = 0,
            Completed = 1,
            Pending = 2,
            Failed = 3
        }
        public static bool IsValidEnumValue<TEnum>(object value) where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
        public static string GenerateWalletAccount()
        {
            var random = new Random();
            return $"WAL{random.Next(100000000, 999999999)}{random.Next(10, 99)}";
        }
        public static string GenerateSecurePassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            var bytes = new byte[length];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            var password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[bytes[i] % validChars.Length];
            }

            return new string(password);
        }

        public static List<string> GetMultipleFileUrl(string baseUri, List<string> fileNames, string fileFolderName)
        {
            if (fileNames != null)
            {
                var baseUriGet = new Uri(baseUri);
                var fileUrls = new List<string>();

                foreach (var newFileName in fileNames)
                {
                    if (!string.IsNullOrWhiteSpace(newFileName))
                    {
                        var fileUri = new Uri(baseUriGet, $"{fileFolderName}/{newFileName.Trim()}");

                        string fileUrl = fileUri.ToString();

                        fileUrls.Add(fileUrl);
                    }
                }
                return fileUrls;
            }
            return null;
        }
    }

    public static class ResponseMessage
    {
        // Configuration
        public const string ConfigNotFound = "Configuration key '{0}' was not found.";

        // TimeZone
        public const string TimeZoneNotFound = "Time zone not found.";

        // Auth
        public const string EmailInvalid = "The provided email address is invalid.";
        public const string PasswordInvalid = "The password entered is incorrect.";
        public const string AccountNotActive = "Your account is not active. Please contact support.";
        public const string EmailNotVerify = "Your email address is not verified. Please check your inbox for the verification link.";
        public const string OTPGenerated = "OTP has been generated successfully.";
        public const string InvalidOtp = "The OTP is invalid or has expired.";
        public const string UserNotFound = "User not found.";
        public const string UserNotActive = "This user is currently inactive.";
        public const string IBUserNotFound = "IB user not found.";
        public const string PasswordChanged = "Your password has been changed successfully.";
        public const string LoginSuccessful = "Login was successful.";
        public const string Logout = "Logout was successful.";
        public const string OtpResentSuccessfully = "OTP has been resent successfully.";
        public const string ForgotPasswordOtpSent = "An OTP has been sent to your registered email address.";
        public const string ForgotPasswordOtpVerify = "OTP has been verified successfully.";
        public const string PasswordResetSuccessful = "Your password has been reset successfully.";
        public const string TwoFactorAuthenticationEnabled = "Two-factor authentication has been enabled.";
        public const string TwoFactorAuthenticationDisabled = "Two-factor authentication has been disabled.";
        public const string TwoFactorAuthenticationAlreadyEnabled = "Two-factor authentication is already enabled.";
        public const string LoginInvalid = "Invalid login credentials.";
        public const string InvalidReferralCode = "The referral code entered is invalid. Please check and try again.";

        // User Registration
        public const string InvalidCountry = "The selected country is invalid.";
        public const string EmailAndPhoneExists = "An account with this email and phone number already exists.";
        public const string EmailExists = "An account with this email already exists.";
        public const string PhoneExists = "An account with this phone number already exists.";
        public const string RegistrationSuccess = "Registration successful. Login details have been sent to your email.";
        public const string InvalidVerificationToken = "The verification token is invalid or has already been used.";
        public const string EmailAlreadyVerified = "This email address is already verified.";
        public const string VerificationLinkExpired = "The verification link has expired.";
        public static string ProfileUpdate = "Profile Update Successfully";

        // Tutorial
        public const string AddTutorial = "Tutorial added successfully.";
        public const string UpdateTutorial = "Tutorial updated successfully.";
        public const string DeleteTutorial = "Tutorial deleted successfully.";
        public const string ActiveTutorial = "Tutorial activated successfully.";
        public const string DeactiveTutorial = "Tutorial deactivated successfully.";
        public const string TutorialNotFound = "Tutorial not found.";
        public const string TutorialTitleAlready = "A tutorial with this title already exists.";
        public const string YoutubeURLAlready = "This YouTube URL already exists.";

        //Banner
        public const string UploadBanner = "Please upload a banner image to continue.";
        public const string BannerNotFound = "Banner not found.";
        public const string BannerSaved = "Banner uploaded and saved successfully.";
        public const string BannerDeleted = "Banner deleted successfully.";
        public const string BannerDeactivated = "The banner has been deactivated.";
        public const string BannerActivated = "The banner has been activated.";

        // Promotion Kit
        public const string AddPromotionKit = "Promotion kit added successfully.";
        public const string UpdatePromotionKit = "Promotion kit updated successfully.";
        public const string ActivePromotionKit = "Promotion kit activated successfully.";
        public const string DeactivePromotionKit = "Promotion kit deactivated successfully.";
        public const string PromotionKitNotFound = "Promotion kit not found.";
        public const string DeletePromotionKit = "Promotion kit deleted successfully.";

        // FAQs
        public const string AddFAQs = "FAQ added successfully.";
        public const string UpdateFAQs = "FAQ updated successfully.";
        public const string DuplicateFAQFound = "Duplicate FAQ questions are not allowed.";
        public const string FaqNotFound = "FAQ not found. Please ensure the FAQ ID is correct.";
        public const string DeleteFaq = "FAQ deleted successfully.";
        public const string ActiveFAQ = "FAQ activated successfully.";
        public const string InActiveFAQ = "FAQ deactivated successfully.";

        // Lead & Source
        public const string LeadSaved = "Lead saved successfully.";
        public const string LeadDeleted = "Lead deleted successfully.";
        public const string LeadNotFound = "Lead not found.";
        public const string InvalidStatus = "Invalid status value.";
        public const string LeadStatusUpdated = "Lead status updated successfully.";
        public const string SourceSaved = "Source saved successfully.";
        public const string SourceDeleted = "Source deleted successfully.";
        public const string SourceNotFound = "Source not found.";

        // Wallet
        public const string PendingRequestExists = "A pending deposit request already exists. Please wait for approval or rejection before creating a new request.";
        public const string ReferenceIdAlreadyExists = "This reference ID already exists.";
        public const string DepositRequest = "Deposit request submitted successfully.";
        public const string DepositRequestNotFound = "No pending deposit request found for approval.";
        public const string DepositRequestApproved = "Deposit request has already been approved.";
        public const string DepositRequestRejected = "Deposit request has already been rejected.";
        public const string RequestAccepted = "Deposit request has been approved.";
        public const string RequestRejected = "Deposit request has been rejected.";
        public const string WalletNotFound = "User wallet not found.";
        public const string WithdrawApproved = "Your Withdraw Request has been approved.";
        public const string RequestAlreadyProcessed = "User {0}'s withdrawal request has already been {1}.";

        // Payment
        public const string PaymentGatewayNotFound = "Payment gateway not found.";
        public const string PaymentGatewayUpdate = "Payment gateway information updated successfully.";
        public const string PaymentGatewayDepositProcessed = "Your payment is being processed. Please wait.";
        public const string CertificateTimeInvalid = "The server's security certificate is invalid. Please check your system's date and time settings or contact support.";
        public const string PaymentGatewayDepositError = "An error occurred while connecting to the payment gateway. Please try again later.";
        public const string DepositFailed = "Payment failed.";
        public const string DepositSuccess = "Payment deposited successfully.";
        public const string DepositExpired = "The payment link has expired.";
        public const string PaymentOrderIdNotFound = "Payment order ID not found.";
        public const string PaymentDepositPending = "Your payment is being processed. Please wait.";
        public const string ExistingRequestPending = "A withdrawal request is already pending. Please wait until it is processed before creating a new request.";
        public const string InsufficientBalance = "Insufficient wallet balance.";
        public const string InvalidWithdrawType = "Invalid withdrawal method selected. Please choose a valid option.";
        public const string CreateWithdrawRequest = "Withdraw request submitted successfully. You will receive your transfer within 24 hours.";
        public const string WithdrawalsDisabled = "Withdrawals are currently disabled. Please contact the administrator.";
        public const string PaymentGatewayDepositDisableAmt = "Please enter an amount within the allowed range.";
        public const string PaymentGatewayDepositRuleAmtLimit = "You have exceeded the minimum or maximum amount limit. Please contact the administrator.";
        public const string ApprovedRequest = "Withdrawal request has been Approved.";
        public const string RejectRequest = "Withdrawal request has been Rejected.";
        public const string RequestNotFound = "No pending withdrawal request found for approval.";
        public const string KycRequiredDeposit = "KYC is Required For Deposit";
        public const string KycRequiredWithdraw = "KYC is Required For Withdraw";
        public const string AmountLimit = "The amount must be between {0} and {1}.";

        // Group & Symbols
        public const string NoNewGroupFound = "No new groups found to sync.";
        public const string SyncGroup = "Groups synced successfully.";
        public const string NoNewSymbolsFound = "No new symbols found to sync.";
        public const string SyncSymbols = "Symbols synced successfully.";
        public const string SyncSymbolsGroup = "Group symbols synchronized successfully.";

        // Client
        public const string CreateClientError = "An error occurred while creating your Vertex account. Please try again later.";
        public const string ClientAlreadyExists = "Client already exists.";
        public const string ClientCreateSuccess = "Client created successfully.";
        public const string ClientNotFound = "Client not found.";
        public const string BackOfficeLoginFailed = "BackOffice login failed for TerminalID {0}: {1}";
        public const string SummaryFetchFailed = "Failed to fetch account summary for TerminalID {0}: {1}";
        public const string SummaryParseFailed = "Failed to parse account summary for TerminalID {0}.";
        public const string ExceptionMessage = "Exception for TerminalID {0}: {1}";
        public const string JobSummary = "{0} equity records inserted successfully.";
        public const string JobSummaryWithErrors = "{0} equity records inserted successfully. {1} errors: {2}";

        // Account
        public const string DemoAccountCreationSuccess = "Demo account created successfully.";
        public const string TradingDepositBalance = "Trading platform deposit balance updated successfully.";
        public const string VertexAccNotFound = "Vertex trading platform account not found.";
        public const string LiveAccountCreationSuccess = "Live account created successfully.";
        public const string InvalidAccountType = "Invalid account type.";
        public const string VertexNotRequest = "No request found on the Vertex platform. Please contact support.";
        public const string LiveAccountCreationPending = "Your request for a Vertex trading account has been received and is pending admin approval.";
        public const string InvalidTradingType = "Invalid trading account type selected.";
        public const string FailedToCreateAccount = "Failed to create the trading account. Please try again later.";
        public const string TransferFailed = "Balance transfer failed. Please check the details and try again.";
        public const string ApprovedByAdmin = "Request approved by the Admin.";
        public const string RejectedByAdmin = "Request rejected by the Admin.";
        public const string DepositByAdminSuccess = "{0}$ added successfully to {1}'s wallet by admin.";
        public const string DepositByAdminFailed = "Deposit failed.";
        public const string InvalidTransactionPassword = "Invalid transaction password.";
        public const string AccountDeleted = "Account Delete Successfully.";
        public const string MT5AccNotFound = "MT5 trading platform account not found.";

        //Bank Detail
        public static string AddBankDetail = "Bank details added successfully";
        public static string UpdateBankDetail = "Bank details updated successfully";
        public static string BankDetailNotFound = "Bank details could not be found";
        public static string ActiveBankDetail = "Bank detail active successfully.";
        public static string InActiveBankDetail = "Bank detail inactive successfully.";
        public static string DeleteBankDetail = "Bank detail delete successfully.";
        public static string DoNotDelete = "Cannot delete active bank details.";

        // AccountType
        public const string AccountTypeNotFound = "Account type not found.";
        public const string AccountTypeCreated = "Account type created successfully.";
        public const string AccountTypeUpdated = "Account type updated successfully.";
        public const string GroupNotFound = "Group not found.";
        public const string GroupExistWithAccountType = "An account type already exists for the selected group.";
        public const string DemoAccountTypeBasePrice = "Your account type is demo, so the basic price of {0} should be 0.";

        //Account

        public const string AccountNotFound = "Account not found";

        // Rule
        public const string CreateRule = "Rule created successfully.";
        public const string RuleExist = "A default rule of this type already exists.";
        public const string KYCisAuto = "IsAuto is required when RuleType is KYC.";
        public const string MinMaxLimit = "Min and Max Limit are required and must be greater than zero for Deposit and Withdrawal rules.";
        public const string MandotoryField = "At least one value must be selected in MandatoryModule.";
        public const string UpdateRule = "Rule Update Successfully";

        // Auth (Admin)
        public const string LoginByAdmin = "Login successful.";
        public const string TradingUnauthorized = "Unauthorized access. Please check your credentials.";
        public const string LogNotFound = "Logs Not Found";

        //Ticket
        public const string TicketCreate = "Ticket created successfully";
        public const string CloseTicket = "Ticket closed successfully";

        // Common
        public const string ErrorOccurred = "An error occurred while processing your request.";
        public const string DataRetrieved = "Data retrieved successfully.";
        public const string DataNotFound = "Data not found.";
        public const string DataFound = "Data found successfully.";
        public const string Unauthorized = "Unauthorized access.";
        public const string InternalServerError = "Internal server error.";
        public const string SelectFileOne = "Please select only one file.";
        public const string SelectFile = "Please select a file.";
        public const string ConnectionError = "Unable to establish a connection. Please try again.";

        //KYC
        public const string KycNotFound = "No KYC request found.";
        public const string KycAlreadyProcessed = "This KYC request has already been processed.";
        public const string KycStatus = "KYC request processed successfully. Status: {0}.";
        public const string AlreadySubmittedPending = "KYC request already submitted. Please wait for approval.";
        public const string AlreadySubmittedApproved = "KYC request already approved.";
        public const string InvalidFileFormat = "Invalid file format. Allowed formats: .jpg, .jpeg, .png, .pdf";
        public const string UploadedSuccessfully = "KYC documents uploaded successfully. Awaiting approval.";

        // Challenge Application (add these if not present)
        public const string PhaseNotFound = "Phase not found.";
        public const string BalanceConfigNotFound = "Balance configuration not found.";
        public const string ChallengeConfigNotFound = "Challenge configuration not found.";
        public const string InsufficientWalletBalance = "Insufficient balance in your wallet to purchase this challenge.";
        public const string ChallengePurchasedSuccessfully = "Challenge purchased successfully.";
        public const string ChallengeRequestSentToAdmin = "Request sent to admin successfully.";
        public const string PhaseConfigurationNotFound = "No configuration found for this challenge phase.";
        public const string ChallengeRequestNotFound = "Challenge request could not be found.";
        public const string ChallengeRequestAlreadyApproved = "This challenge request has already been approved.";
        public const string ChallengeRequestAlreadyRejected = "This challenge request has already been rejected.";
        public const string ChallengeRequestNotPending = "The challenge request is not in a pending state.";
        public const string ChallengeApproved = "Challenge approved successfully. A trading account has been created.";
        public const string ChallengeAutoApproved = "Challenge Auto Approved successfully.";
        public const string ChallengeRequestSent = "Challenge request submitted for admin approval.";
        public const string ChallengeRejected = "Challenge request has been rejected.";
        public const string InvalidRequestData = "The request contains invalid or missing data.";
        public const string PhaseAccountTypeNotExist = "Phase ID {0} does not exist for the selected account type.";
        public const string DuplicateRequestBalances = "Duplicate balance values found in the request: {0}.";
        public const string DuplicateDatabaseBalances = "The following balance values already exist in the system for this phase: {0}.";
        public const string ChallengeFetched = "Challenge configuration retrieved successfully.";
        public const string ChallengeAdded = "Challenge configuration created successfully.";
        public const string InvalidChallengeRequest = "Please provide a valid Account Type ID, Phase ID, and configuration details.";
        public const string PhaseDataNotFound = "No data found for Phase ID {0}.";
        public const string PhaseConfigNotFound = "No configuration found for Phase ID {0} with balance value {1}.";
        public const string PhaseRulesUpdated = "Phase rules updated successfully.";
        public const string PhaseDeleted = "Phase and all associated phases have been deleted.";
        public const string PhaseLimitExceeded = "The maximum number of allowed phases ({0}) for this account type has been exceeded.";
        public const string PhaseCreated = "Phases created successfully.";
        public const string PhaseUpdated = "Phases updated successfully.";
        public const string Balancetransferfailed = "Balance transfer failed.";
        public const string PhaseIdNotFound = "Phase ID {0} was not found.";
        public const string PhaseConfigurationsDeleted = "Deleted {0} configurations and {1} balance records for Account Type ID {2}, Phase ID {3}.";
        public const string BalanceConfigUpdated = "Balance configuration updated successfully!";


        //Sales
        public const string InvalidExcelNoWorksheet = "Invalid Excel file: No worksheet found.";
        public const string NoDataFound = "No data found in the file.";
        public const string InvalidHeaderFormat = "Invalid file format. Please ensure the headers match the template.";
        public const string RequiredFieldBlank = "Required field cannot be blank.";
        public const string InvalidMobileFormat = "Mobile number must contain only digits.";
        public const string DuplicateEmailInFile = "Duplicate email within the file.";
        public const string DuplicateMobileInFile = "Duplicate mobile number within the file.";
        public const string CountryInvalid = "Invalid country. Please ensure the country exists in the system.";
        public const string InvalidSource = "Invalid source name. Please ensure the source exists in the system.";
        public const string EmailAlreadyExists = "Email already exists in the system.";
        public const string MobileAlreadyExists = "Mobile number already exists in the system.";
        public const string UploadWithErrors = "File uploaded with errors.";
        public const string NoValidLeads = "No valid leads found in the file.";
        public const string UploadSuccess = "File uploaded successfully. {0} leads processed.";
        public const string SaveError = "An error occurred while saving leads. {0}";
        public const string PropTradingClientURL = "https://www.hybridsolutions.com/downloads/vertexfx.exe";


        // Utility methods
        public static string Create(string entity) => $"{entity} created successfully.";
        public static string Update(string entity) => $"{entity} updated successfully.";
        public static string Delete(string entity) => $"{entity} deleted successfully.";
        public static string Retrieve(string entity) => $"{entity} retrieved successfully.";
        public static string Error(string message) => $"Error: {message}";
        public static string ParamsError(List<string> missingFields) => $"Missing required fields: {string.Join(", ", missingFields)}";
        public static string Found(string entity) => $"{entity} retrieved successfully.";
        public static string NotFound(string entity) => $"{entity} not found.";
        public static string AlreadyExists(string entity) => $"{entity} already exists.";
        public static string Success() => "Success";
        public static string GetActivationMessage(string entity, bool isActive) => $"{entity} has been {(isActive ? "activated" : "deactivated")} successfully.";
    }
}
