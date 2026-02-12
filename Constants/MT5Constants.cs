namespace PropMT5ConnectionService.Constants
{
    /// <summary>
    /// Constants for MT5 operations
    /// </summary>
    public static class MT5Constants
    {
        /// <summary>
        /// Password configuration
        /// </summary>
        public static class PasswordConfig
        {
            public const int DefaultMasterPasswordLength = 11;
            public const int DefaultInvestorPasswordLength = 9;
            public const int MinPasswordLength = 8;
            public const int MaxPasswordLength = 20;
        }

        /// <summary>
        /// Account configuration
        /// </summary>
        public static class AccountConfig
        {
            public const string DefaultLiveLoginPrefix = "555";
            public const string DefaultDemoLoginPrefix = "999";
            public const int MaxAccountCreationAttempts = 100;
            public const string DefaultLiveServerName = "PropTradingMT5";
            public const string DefaultDemoServerName = "QuorionexMarketsTestOnly-Trade";
        }

        /// <summary>
        /// Password types
        /// </summary>
        public enum PasswordType
        {
            Master = 0,
            Investor = 1
        }

        /// <summary>
        /// Account types
        /// </summary>
        public static class AccountType
        {
            public const string Live = "Live";
            public const string Demo = "Demo";
        }

        /// <summary>
        /// User rights
        /// </summary>
        public static class UserRights
        {
            public const string Enabled = "Enabled";
            public const string OTPEnabled = "OTPEnabled";
            public const string Password = "Password";
            public const string TradingAllowed = "TradingAllowed";
        }

        /// <summary>
        /// API Response messages
        /// </summary>
        public static class ResponseMessages
        {
            // Success messages
            public const string AccountCreatedSuccess = "Account created successfully";
            public const string AccountRetrievedSuccess = "Account retrieved successfully";
            public const string AccountsRetrievedSuccess = "Accounts retrieved successfully";
            public const string PasswordChangedSuccess = "{0} password changed successfully";
            public const string StatusUpdatedSuccess = "Account status updated successfully";

            // Error messages
            public const string ModelCannotBeNull = "Model cannot be null";
            public const string InvalidModelState = "Invalid model state";
            public const string PasswordCannotBeEmpty = "Password cannot be empty";
            public const string InvalidPasswordType = "Password type must be 0 (Master) or 1 (Investor)";
            public const string AccountCreationFailed = "Failed to create account after {0} attempts";
            public const string UnexpectedError = "An unexpected error occurred: {0}";
        }

        /// <summary>
        /// HTTP Status codes
        /// </summary>
        public static class StatusCodes
        {
            public const int Success = 200;
            public const int BadRequest = 400;
            public const int NotFound = 404;
            public const int InternalServerError = 500;
        }
    }
}
