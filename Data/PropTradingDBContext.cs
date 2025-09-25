using Microsoft.EntityFrameworkCore;
using PropMT5ConnectionService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Data
{
    public class PropTradingDBContext : DbContext
    {
        public PropTradingDBContext(DbContextOptions<PropTradingDBContext> options)
            : base(options)
        {

        }

        public DbSet<PropAccountMaster> PropAccountMaster { get; set; }
        public DbSet<UserMaster> UserMasters { get; set; }

/*        public DbSet<EmailTemplateAttachmentsMaster> EmailTemplateAttachmentsMaster { get; set; }
        public DbSet<EmailTemplatesDetailsMaster> EmailTemplatesDetailsMaster { get; set; }
        public DbSet<EmailTemplatesMaster> EmailTemplatesMaster { get; set; }
        public DbSet<EmailTemplatesStaticKeyValueMaster> EmailTemplatesStaticKeyValueMaster { get; set; }
        public DbSet<EmailVariablesMaster> EmailVariablesMaster { get; set; }
        public DbSet<MailRequestLog> MailRequeslog { get; set; }
        public DbSet<MT5GroupCommissionMaster> MT5GroupCommissionMaster { get; set; }*/

        // Challenge
        
         public DbSet<UserChallengeHistory> UserChallengeHistory { get; set; }
        public DbSet<UserChallengePhase> UserChallengePhase { get; set; }
     /*   public DbSet<UserChallengeDailyEquityHistory> UserChallengeDailyEquityHistory { get; set; }
        public DbSet<BankDetailsMaster> BankDetailsMaster { get; set; }*/

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes
            modelBuilder.Entity<UserMaster>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserMaster>()
                .HasIndex(u => u.AffiliateCode);

            modelBuilder.Entity<UserMaster>()
                .HasIndex(u => u.ReferralCode)
                .IsUnique();

            modelBuilder.Entity<ExceptionLog>()
                .HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_ExceptionLogs_Timestamp");

            modelBuilder.Entity<AuditLog>()
                .HasIndex(e => new { e.Timestamp, e.UserId })
                .HasDatabaseName("IX_AuditLogs_Timestamp_UserId");

            modelBuilder.Entity<KYCRequestMaster>()
                .HasIndex(r => r.UserId)
                .HasDatabaseName("IX_KYCRequestMaster_UserId");

            modelBuilder.Entity<KYCRequestMaster>()
                .HasIndex(r => new { r.Status, r.ActionDate })
                .HasDatabaseName("IX_KYCRequestMaster_Status_ActionDate");

            modelBuilder.Entity<KYCRequestHistory>()
                .HasIndex(h => h.KYCRequestId)
                .HasDatabaseName("IX_KYCHistory_KYCRequestId");

            modelBuilder.Entity<KYCRequestHistory>()
                .HasIndex(h => h.DeclinedAt)
                .HasDatabaseName("IX_KYCHistory_DeclinedAt");

            modelBuilder.Entity<EmailTemplatesMaster>()
                .Property(e => e.ETKey)
                .HasConversion<string>();

            modelBuilder.Entity<TicketQueryMaster>()
                .HasIndex(x => new { x.QueryName })
                .IsUnique();

            //modelBuilder.Entity<RuleMaster>().Property(r => r.MandatoryModule).HasConversion(
            //v => string.Join(",", v.Select(e => e.ToString())),
            //v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
            //      .Select(e => Enum.Parse<RuleMandatoryFor>(e))
            //      .ToList());

            modelBuilder.Entity<WalletWithdrawal>()
            .Property(w => w.WithdrawType)
            .HasConversion(
                v => v.ToString(),
                v => (WithdrawType)Enum.Parse(typeof(WithdrawType), v)
            );

            modelBuilder.Entity<WalletWithdrawal>()
                .Property(w => w.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionStatus)Enum.Parse(typeof(TransactionStatus), v)
                );

            modelBuilder.Entity<InternalTransferHistory>()
                .Property(w => w.TransferType)
                .HasConversion(
                    v => v.ToString(),
                    v => (InternalTransferType)Enum.Parse(typeof(InternalTransferType), v)
                );

            modelBuilder.Entity<UserChallengeDailyEquityHistory>()
                .Property(e => e.DailyProfitOrLoss)
                .HasComputedColumnSql("\"CurrentDayEquity\" - \"PreviousDayEquity\"", stored: true);

            #region // Seed default admin entry

            // Seed TicketQueryMaster data based on the image
            var seedDate = new DateTime(2025, 6, 26, 7, 10, 1, 89, DateTimeKind.Utc); // 2024-06-20 12:40:01.089 IST to UTC

            modelBuilder.Entity<TicketQueryMaster>().HasData(
                new TicketQueryMaster { TicketQueryId = 1, QueryName = "Record Transaction", Description = "Transaction Record", IsIBRelated = true, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 2, QueryName = "Withdrawal", Description = "Withdrawal Request", IsIBRelated = false, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 3, QueryName = "Deposit", Description = "Open", IsIBRelated = false, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 4, QueryName = "Account", Description = "Account Management", IsIBRelated = false, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 5, QueryName = "IB Commission Role Back", Description = "IB Commission Rollback", IsIBRelated = true, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 6, QueryName = "Calculate IB Commission", Description = "Calculate IB Commission", IsIBRelated = true, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 7, QueryName = "IB Commission", Description = "IB Commission Payment", IsIBRelated = true, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 8, QueryName = "Internal Transfer", Description = "Internal Fund Transfer", IsIBRelated = false, CreatedDate = seedDate, CreatedBy = 1L },
                new TicketQueryMaster { TicketQueryId = 9, QueryName = "Other", Description = "Other", IsIBRelated = false, CreatedDate = seedDate, CreatedBy = 1L }
            );

            // Seed Categories
            modelBuilder.Entity<FAQsCategoryMaster>().HasData(
                new FAQsCategoryMaster { FAQCategoryId = 1, CategoryName = "Account", CategoryDescription = "Account-related questions", CreatedDate = seedDate },
                new FAQsCategoryMaster { FAQCategoryId = 2, CategoryName = "Deposit", CategoryDescription = "Deposit-related questions", CreatedDate = seedDate },
                new FAQsCategoryMaster { FAQCategoryId = 3, CategoryName = "Client", CategoryDescription = "Client-related questions", CreatedDate = seedDate },
                new FAQsCategoryMaster { FAQCategoryId = 4, CategoryName = "Prop Trading", CategoryDescription = "Prop Trading support", CreatedDate = seedDate },
                new FAQsCategoryMaster { FAQCategoryId = 5, CategoryName = "Withdrawal", CategoryDescription = "Withdrawals and issues", CreatedDate = seedDate },
                new FAQsCategoryMaster { FAQCategoryId = 6, CategoryName = "Internal Transfer", CategoryDescription = "Transfers within the platform", CreatedDate = seedDate }
            );



            modelBuilder.Entity<PaymentGatways>().HasData(
            new PaymentGatways
            {
                Id = 1,
                Name = "ZaroPay",
                Currency = "USD",
                Fees = 2.0m,
                ProcessingTime = "24 H",
                ClientId = "6861647269",
                ClientSecretkey = " ",
                ApiURLName = "https://staging.zaropay.com/api/v1",
                ApiKeyName = "z-api-key",
                ApiKeyValue = "4Ths8S8JM-Spomko2s-sHntUcNM-S0nBawLP",
                BackButtonRedirectUrl = " ",
                CallBackUrl = "/api/WalletMaster/ZaropayTransactionUpdate",
                IsEnabled = true,
                CreatedDate = DateTime.UtcNow,  // ⚠️ will be fixed at migration time
                FilePath = "https://mt5.neptunefxcrm.com/PaymentLogoImage/zaropay.png",
                FileName = null,
                ContryId = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246", // full country list
                IsPaymentGatway = true
            },
            new PaymentGatways
            {
                Id = 2,
                Name = "CashDeposit",
                Currency = "USD",
                Fees = 0m,
                ProcessingTime = "24 H",
                ClientId = "0",
                ClientSecretkey = null,
                ApiURLName = null,
                ApiKeyName = null,
                ApiKeyValue = null,
                BackButtonRedirectUrl = null,
                CallBackUrl = null,
                IsEnabled = true,
                CreatedDate = new DateTime(2025, 07, 25, 05, 41, 38, DateTimeKind.Utc),
                FilePath = "https://mt5.neptunefxcrm.com/PaymentLogoImage/bank.png",
                FileName = null,
                ContryId = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246",
                IsPaymentGatway = true
            }
            );


            modelBuilder.Entity<FAQsQuestionAnswerMaster>().HasData(
                     // Account (1)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 1, Question = "How to register an account?", Answer = "Use the Sign Up form on the homepage.", FAQCategoryId = 1, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 2, Question = "Can I change my email?", Answer = "Yes, via account settings.", FAQCategoryId = 1, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 3, Question = "How to update my profile?", Answer = "Go to the Profile section.", FAQCategoryId = 1, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 4, Question = "What if I forget my password?", Answer = "Use the Forgot Password link.", FAQCategoryId = 1, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 5, Question = "How to delete my account?", Answer = "Contact support for deletion requests.", FAQCategoryId = 1, IsActive = true, IsDeleted = false, CreatedDate = seedDate },

                     // Deposit (2)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 6, Question = "How to make a deposit?", Answer = "Go to Deposit section under Finance.", FAQCategoryId = 2, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 7, Question = "Are there deposit fees?", Answer = "No fees for standard deposits.", FAQCategoryId = 2, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 8, Question = "Is there a minimum deposit?", Answer = "Yes, $50 minimum.", FAQCategoryId = 2, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 9, Question = "How long does a deposit take?", Answer = "Usually within 1 hour.", FAQCategoryId = 2, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 10, Question = "Can I use a credit card?", Answer = "Yes, via the Payment Gateway.", FAQCategoryId = 2, IsActive = true, IsDeleted = false, CreatedDate = seedDate },

                     // Client (3)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 11, Question = "What is a client profile?", Answer = "It stores your basic info and trading preferences.", FAQCategoryId = 3, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 12, Question = "How to verify my client profile?", Answer = "Upload documents under Verification.", FAQCategoryId = 3, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 13, Question = "Can I have multiple profiles?", Answer = "No, one profile per client is allowed.", FAQCategoryId = 3, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 14, Question = "Where can I update contact info?", Answer = "Under Profile Settings.", FAQCategoryId = 3, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 15, Question = "Why was my profile rejected?", Answer = "Check your documents and re-upload.", FAQCategoryId = 3, IsActive = true, IsDeleted = false, CreatedDate = seedDate },

                     // Prop Trading (4)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 16, Question = "How do I set up my Prop Trading account?", Answer = "After completing your purchase or challenge registration, your Prop Trading account details will be emailed to you.", FAQCategoryId = 4, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 17, Question = "Can I trade on mobile devices?", Answer = "Yes, Prop Trading supports both iOS and Android apps. You can download them from the App Store or Google Play.", FAQCategoryId = 4, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 18, Question = "Where can I find my MT4/MT5 login credentials?", Answer = "Your MT4/MT5 login credentials are provided in your welcome email and are also visible in your Trader's Room.", FAQCategoryId = 4, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 19, Question = "How can I reset my Prop Trading platform password?", Answer = "You can reset your password directly from your Trader's Room or by contacting our support team.", FAQCategoryId = 4, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 20, Question = "What should I do if I can't connect to the Prop Trading server?", Answer = "Please check your internet connection, verify your login details, and ensure you're selecting the correct server. If issues persist, contact support.", FAQCategoryId = 4, IsActive = true, IsDeleted = false, CreatedDate = seedDate },

                     // Withdrawal (5)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 21, Question = "How to request a withdrawal?", Answer = "Go to Withdraw section under Finance.", FAQCategoryId = 5, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 22, Question = "Withdrawal fees?", Answer = "Some methods may incur fees.", FAQCategoryId = 5, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 23, Question = "Withdrawal time?", Answer = "Typically 1–3 business days.", FAQCategoryId = 5, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 24, Question = "Minimum withdrawal?", Answer = "$20 minimum.", FAQCategoryId = 5, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 25, Question = "Why was my withdrawal declined?", Answer = "Ensure sufficient balance and valid method.", FAQCategoryId = 5, IsActive = true, IsDeleted = false, CreatedDate = seedDate },

                     // Internal Transfer (6)
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 26, Question = "What is internal transfer?", Answer = "Funds moved between your own accounts.", FAQCategoryId = 6, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 27, Question = "Is there a fee?", Answer = "Internal transfers are free of charge.", FAQCategoryId = 6, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 28, Question = "How to initiate a transfer?", Answer = "Use the Transfer Funds option.", FAQCategoryId = 6, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 29, Question = "Transfer not reflected?", Answer = "Wait up to 15 minutes, then check.", FAQCategoryId = 6, IsActive = true, IsDeleted = false, CreatedDate = seedDate },
                     new FAQsQuestionAnswerMaster { FAQQuestionAnswerId = 30, Question = "Limits on internal transfers?", Answer = "No daily limit currently applied.", FAQCategoryId = 6, IsActive = true, IsDeleted = false, CreatedDate = seedDate }
                 );


            // Seeding CountryMaster data
            modelBuilder.Entity<CountryMaster>().HasData(GetCountries());

            // Seed UserMaster, UserDetailsMaster, and WalletMaster
            var (users, wallets, userDetails) = GetUsers();
            modelBuilder.Entity<UserMaster>().HasData(users);
            modelBuilder.Entity<WalletMaster>().HasData(wallets);
            modelBuilder.Entity<UserDetailsMaster>().HasData(userDetails);
            //modelBuilder.Entity<EmailTemplateMaster>().HasData(EmailTemplateMasters());

            #endregion
        }

        // data seeding
        private CountryMaster[] GetCountries()
        {
            return new[]
            {
             new CountryMaster { Id = 1, CountryName = "Afghanistan", CountryCode2 = "AF" },
        new CountryMaster { Id = 2, CountryName = "Albania", CountryCode2 = "AL" },
        new CountryMaster { Id = 3, CountryName = "Algeria", CountryCode2 = "DZ" },
        new CountryMaster { Id = 4, CountryName = "Andorra", CountryCode2 = "AD" },
        new CountryMaster { Id = 5, CountryName = "Angola", CountryCode2 = "AO" },
        new CountryMaster { Id = 6, CountryName = "Antigua and Barbuda", CountryCode2 = "AG" },
        new CountryMaster { Id = 7, CountryName = "Argentina", CountryCode2 = "AR" },
        new CountryMaster { Id = 8, CountryName = "Armenia", CountryCode2 = "AM" },
        new CountryMaster { Id = 9, CountryName = "Australia", CountryCode2 = "AU" },
        new CountryMaster { Id = 10, CountryName = "Austria", CountryCode2 = "AT" },
        new CountryMaster { Id = 11, CountryName = "Azerbaijan", CountryCode2 = "AZ" },
        new CountryMaster { Id = 12, CountryName = "Bahamas", CountryCode2 = "BS" },
        new CountryMaster { Id = 13, CountryName = "Bahrain", CountryCode2 = "BH" },
        new CountryMaster { Id = 14, CountryName = "Bangladesh", CountryCode2 = "BD" },
        new CountryMaster { Id = 15, CountryName = "Barbados", CountryCode2 = "BB" },
        new CountryMaster { Id = 16, CountryName = "Belarus", CountryCode2 = "BY" },
        new CountryMaster { Id = 17, CountryName = "Belgium", CountryCode2 = "BE" },
        new CountryMaster { Id = 18, CountryName = "Belize", CountryCode2 = "BZ" },
        new CountryMaster { Id = 19, CountryName = "Benin", CountryCode2 = "BJ" },
        new CountryMaster { Id = 20, CountryName = "Bhutan", CountryCode2 = "BT" },
        new CountryMaster { Id = 21, CountryName = "Bolivia", CountryCode2 = "BO" },
        new CountryMaster { Id = 22, CountryName = "Bosnia and Herzegovina", CountryCode2 = "BA" },
        new CountryMaster { Id = 23, CountryName = "Botswana", CountryCode2 = "BW" },
        new CountryMaster { Id = 24, CountryName = "Brazil", CountryCode2 = "BR" },
        new CountryMaster { Id = 25, CountryName = "Brunei Darussalam", CountryCode2 = "BN" },
        new CountryMaster { Id = 26, CountryName = "Bulgaria", CountryCode2 = "BG" },
        new CountryMaster { Id = 27, CountryName = "Burkina Faso", CountryCode2 = "BF" },
        new CountryMaster { Id = 28, CountryName = "Burundi", CountryCode2 = "BI" },
        new CountryMaster { Id = 29, CountryName = "Cabo Verde", CountryCode2 = "CV" },
        new CountryMaster { Id = 30, CountryName = "Cambodia", CountryCode2 = "KH" },
        new CountryMaster { Id = 31, CountryName = "Cameroon", CountryCode2 = "CM" },
        new CountryMaster { Id = 32, CountryName = "Canada", CountryCode2 = "CA" },
        new CountryMaster { Id = 33, CountryName = "Central African Republic", CountryCode2 = "CF" },
        new CountryMaster { Id = 34, CountryName = "Chad", CountryCode2 = "TD" },
        new CountryMaster { Id = 35, CountryName = "Chile", CountryCode2 = "CL" },
        new CountryMaster { Id = 36, CountryName = "China", CountryCode2 = "CN" },
        new CountryMaster { Id = 37, CountryName = "Colombia", CountryCode2 = "CO" },
        new CountryMaster { Id = 38, CountryName = "Comoros", CountryCode2 = "KM" },
        new CountryMaster { Id = 39, CountryName = "Congo Democratic Republic of the Congo", CountryCode2 = "CD" },
        new CountryMaster { Id = 40, CountryName = "Congo", CountryCode2 = "CG" },
        new CountryMaster { Id = 41, CountryName = "Costa Rica", CountryCode2 = "CR" },
        new CountryMaster { Id = 42, CountryName = "Croatia", CountryCode2 = "HR" },
        new CountryMaster { Id = 43, CountryName = "Cuba", CountryCode2 = "CU" },
        new CountryMaster { Id = 44, CountryName = "Cyprus", CountryCode2 = "CY" },
        new CountryMaster { Id = 45, CountryName = "Czech Republic", CountryCode2 = "CZ" },
        new CountryMaster { Id = 46, CountryName = "Denmark", CountryCode2 = "DK" },
        new CountryMaster { Id = 47, CountryName = "Djibouti", CountryCode2 = "DJ" },
        new CountryMaster { Id = 48, CountryName = "Dominica", CountryCode2 = "DM" },
        new CountryMaster { Id = 49, CountryName = "Dominican Republic", CountryCode2 = "DO" },
        new CountryMaster { Id = 50, CountryName = "Ecuador", CountryCode2 = "EC" },
        new CountryMaster { Id = 51, CountryName = "Egypt", CountryCode2 = "EG" },
        new CountryMaster { Id = 52, CountryName = "El Salvador", CountryCode2 = "SV" },
        new CountryMaster { Id = 53, CountryName = "Equatorial Guinea", CountryCode2 = "GQ" },
        new CountryMaster { Id = 54, CountryName = "Eritrea", CountryCode2 = "ER" },
        new CountryMaster { Id = 55, CountryName = "Estonia", CountryCode2 = "EE" },
        new CountryMaster { Id = 56, CountryName = "Eswatini", CountryCode2 = "SZ" },
        new CountryMaster { Id = 57, CountryName = "Ethiopia", CountryCode2 = "ET" },
        new CountryMaster { Id = 58, CountryName = "Fiji", CountryCode2 = "FJ" },
        new CountryMaster { Id = 59, CountryName = "Finland", CountryCode2 = "FI" },
        new CountryMaster { Id = 60, CountryName = "France", CountryCode2 = "FR" },
        new CountryMaster { Id = 61, CountryName = "Gabon", CountryCode2 = "GA" },
        new CountryMaster { Id = 62, CountryName = "Gambia", CountryCode2 = "GM" },
        new CountryMaster { Id = 63, CountryName = "Georgia", CountryCode2 = "GE" },
        new CountryMaster { Id = 64, CountryName = "Germany", CountryCode2 = "DE" },
        new CountryMaster { Id = 65, CountryName = "Ghana", CountryCode2 = "GH" },
        new CountryMaster { Id = 66, CountryName = "Greece", CountryCode2 = "GR" },
        new CountryMaster { Id = 67, CountryName = "Greenland", CountryCode2 = "GL" },
        new CountryMaster { Id = 68, CountryName = "Grenada", CountryCode2 = "GD" },
        new CountryMaster { Id = 69, CountryName = "Guatemala", CountryCode2 = "GT" },
        new CountryMaster { Id = 70, CountryName = "Guinea", CountryCode2 = "GN" },
        new CountryMaster { Id = 71, CountryName = "Guinea-Bissau", CountryCode2 = "GW" },
        new CountryMaster { Id = 72, CountryName = "Guyana", CountryCode2 = "GY" },
        new CountryMaster { Id = 73, CountryName = "Haiti", CountryCode2 = "HT" },
        new CountryMaster { Id = 74, CountryName = "Honduras", CountryCode2 = "HN" },
        new CountryMaster { Id = 75, CountryName = "Hungary", CountryCode2 = "HU" },
        new CountryMaster { Id = 76, CountryName = "Iceland", CountryCode2 = "IS" },
        new CountryMaster { Id = 77, CountryName = "India", CountryCode2 = "IN" },
        new CountryMaster { Id = 78, CountryName = "Indonesia", CountryCode2 = "ID" },
        new CountryMaster { Id = 79, CountryName = "Iran", CountryCode2 = "IR" },
        new CountryMaster { Id = 80, CountryName = "Iraq", CountryCode2 = "IQ" },
        new CountryMaster { Id = 81, CountryName = "Ireland", CountryCode2 = "IE" },
        new CountryMaster { Id = 82, CountryName = "Israel", CountryCode2 = "IL" },
        new CountryMaster { Id = 83, CountryName = "Italy", CountryCode2 = "IT" },
        new CountryMaster { Id = 84, CountryName = "Jamaica", CountryCode2 = "JM" },
        new CountryMaster { Id = 85, CountryName = "Japan", CountryCode2 = "JP" },
        new CountryMaster { Id = 86, CountryName = "Jordan", CountryCode2 = "JO" },
        new CountryMaster { Id = 87, CountryName = "Kazakhstan", CountryCode2 = "KZ" },
        new CountryMaster { Id = 88, CountryName = "Kenya", CountryCode2 = "KE" },
        new CountryMaster { Id = 89, CountryName = "Kiribati", CountryCode2 = "KI" },
        new CountryMaster { Id = 90, CountryName = "North Korea (Democratic People's Republic of Korea)", CountryCode2 = "KP" },
        new CountryMaster { Id = 91, CountryName = "South Korea (Republic of Korea)", CountryCode2 = "KR" },
        new CountryMaster { Id = 92, CountryName = "Kuwait", CountryCode2 = "KW" },
        new CountryMaster { Id = 93, CountryName = "Kyrgyzstan", CountryCode2 = "KG" },
        new CountryMaster { Id = 94, CountryName = "Lao People's Democratic Republic", CountryCode2 = "LA" },
        new CountryMaster { Id = 95, CountryName = "Latvia", CountryCode2 = "LV" },
        new CountryMaster { Id = 96, CountryName = "Lebanon", CountryCode2 = "LB" },
        new CountryMaster { Id = 97, CountryName = "Lesotho", CountryCode2 = "LS" },
        new CountryMaster { Id = 98, CountryName = "Liberia", CountryCode2 = "LR" },
        new CountryMaster { Id = 99, CountryName = "Libya", CountryCode2 = "LY" },
        new CountryMaster { Id = 100, CountryName = "Liechtenstein", CountryCode2 = "LI" },
        new CountryMaster { Id = 101, CountryName = "Lithuania", CountryCode2 = "LT" },
        new CountryMaster { Id = 102, CountryName = "Luxembourg", CountryCode2 = "LU" },
        new CountryMaster { Id = 103, CountryName = "Madagascar", CountryCode2 = "MG" },
        new CountryMaster { Id = 104, CountryName = "Malawi", CountryCode2 = "MW" },
        new CountryMaster { Id = 105, CountryName = "Malaysia", CountryCode2 = "MY" },
        new CountryMaster { Id = 106, CountryName = "Maldives", CountryCode2 = "MV" },
        new CountryMaster { Id = 107, CountryName = "Mali", CountryCode2 = "ML" },
        new CountryMaster { Id = 108, CountryName = "Malta", CountryCode2 = "MT" },
        new CountryMaster { Id = 109, CountryName = "Marshall Islands", CountryCode2 = "MH" },
        new CountryMaster { Id = 110, CountryName = "Mauritania", CountryCode2 = "MR" },
        new CountryMaster { Id = 111, CountryName = "Mauritius", CountryCode2 = "MU" },
        new CountryMaster { Id = 112, CountryName = "Mexico", CountryCode2 = "MX" },
        new CountryMaster { Id = 113, CountryName = "Micronesia (Federated States of Micronesia)", CountryCode2 = "FM" },
        new CountryMaster { Id = 114, CountryName = "Moldova (Republic of Moldova)", CountryCode2 = "MD" },
        new CountryMaster { Id = 115, CountryName = "Monaco", CountryCode2 = "MC" },
        new CountryMaster { Id = 116, CountryName = "Mongolia", CountryCode2 = "MN" },
        new CountryMaster { Id = 117, CountryName = "Montenegro", CountryCode2 = "ME" },
        new CountryMaster { Id = 118, CountryName = "Morocco", CountryCode2 = "MA" },
        new CountryMaster { Id = 119, CountryName = "Mozambique", CountryCode2 = "MZ" },
        new CountryMaster { Id = 120, CountryName = "Myanmar", CountryCode2 = "MM" },
        new CountryMaster { Id = 121, CountryName = "Namibia", CountryCode2 = "NA" },
        new CountryMaster { Id = 122, CountryName = "Nauru", CountryCode2 = "NR" },
        new CountryMaster { Id = 123, CountryName = "Nepal", CountryCode2 = "NP" },
        new CountryMaster { Id = 124, CountryName = "Netherlands", CountryCode2 = "NL" },
        new CountryMaster { Id = 125, CountryName = "New Zealand", CountryCode2 = "NZ" },
        new CountryMaster { Id = 126, CountryName = "Nicaragua", CountryCode2 = "NI" },
        new CountryMaster { Id = 127, CountryName = "Niger", CountryCode2 = "NE" },
        new CountryMaster { Id = 128, CountryName = "Nigeria", CountryCode2 = "NG" },
        new CountryMaster { Id = 129, CountryName = "Norway", CountryCode2 = "NO" },
        new CountryMaster { Id = 130, CountryName = "Oman", CountryCode2 = "OM" },
        new CountryMaster { Id = 131, CountryName = "Pakistan", CountryCode2 = "PK" },
        new CountryMaster { Id = 132, CountryName = "Palau", CountryCode2 = "PW" },
        new CountryMaster { Id = 133, CountryName = "Panama", CountryCode2 = "PA" },
        new CountryMaster { Id = 134, CountryName = "Papua New Guinea", CountryCode2 = "PG" },
        new CountryMaster { Id = 135, CountryName = "Paraguay", CountryCode2 = "PY" },
        new CountryMaster { Id = 136, CountryName = "Peru", CountryCode2 = "PE" },
        new CountryMaster { Id = 137, CountryName = "Philippines", CountryCode2 = "PH" },
        new CountryMaster { Id = 138, CountryName = "Poland", CountryCode2 = "PL" },
        new CountryMaster { Id = 139, CountryName = "Portugal", CountryCode2 = "PT" },
        new CountryMaster { Id = 140, CountryName = "Qatar", CountryCode2 = "QA" },
        new CountryMaster { Id = 141, CountryName = "Romania", CountryCode2 = "RO" },
        new CountryMaster { Id = 142, CountryName = "Russian Federation", CountryCode2 = "RU" },
        new CountryMaster { Id = 143, CountryName = "Rwanda", CountryCode2 = "RW" },
        new CountryMaster { Id = 144, CountryName = "Saint Kitts and Nevis", CountryCode2 = "KN" },
        new CountryMaster { Id = 145, CountryName = "Saint Lucia", CountryCode2 = "LC" },
        new CountryMaster { Id = 146, CountryName = "Saint Vincent and the Grenadines", CountryCode2 = "VC" },
        new CountryMaster { Id = 147, CountryName = "Samoa", CountryCode2 = "WS" },
        new CountryMaster { Id = 148, CountryName = "San Marino", CountryCode2 = "SM" },
        new CountryMaster { Id = 149, CountryName = "Sao Tome and Principe", CountryCode2 = "ST" },
        new CountryMaster { Id = 150, CountryName = "Saudi Arabia", CountryCode2 = "SA" },
        new CountryMaster { Id = 151, CountryName = "Senegal", CountryCode2 = "SN" },
        new CountryMaster { Id = 152, CountryName = "Serbia", CountryCode2 = "RS" },
        new CountryMaster { Id = 153, CountryName = "Seychelles", CountryCode2 = "SC" },
        new CountryMaster { Id = 154, CountryName = "Sierra Leone", CountryCode2 = "SL" },
        new CountryMaster { Id = 155, CountryName = "Singapore", CountryCode2 = "SG" },
        new CountryMaster { Id = 156, CountryName = "Slovakia", CountryCode2 = "SK" },
        new CountryMaster { Id = 157, CountryName = "Slovenia", CountryCode2 = "SI" },
        new CountryMaster { Id = 158, CountryName = "Solomon Islands", CountryCode2 = "SB" },
        new CountryMaster { Id = 159, CountryName = "Somalia", CountryCode2 = "SO" },
        new CountryMaster { Id = 160, CountryName = "South Africa", CountryCode2 = "ZA" },
        new CountryMaster { Id = 161, CountryName = "South Sudan", CountryCode2 = "SS" },
        new CountryMaster { Id = 162, CountryName = "Spain", CountryCode2 = "ES" },
        new CountryMaster { Id = 163, CountryName = "Sri Lanka", CountryCode2 = "LK" },
        new CountryMaster { Id = 164, CountryName = "Sudan", CountryCode2 = "SD" },
        new CountryMaster { Id = 165, CountryName = "Suriname", CountryCode2 = "SR" },
        new CountryMaster { Id = 166, CountryName = "Sweden", CountryCode2 = "SE" },
        new CountryMaster { Id = 167, CountryName = "Switzerland", CountryCode2 = "CH" },
        new CountryMaster { Id = 168, CountryName = "Syrian Arab Republic", CountryCode2 = "SY" },
        new CountryMaster { Id = 169, CountryName = "Tajikistan", CountryCode2 = "TJ" },
        new CountryMaster { Id = 170, CountryName = "Tanzania, United Republic of Tanzania", CountryCode2 = "TZ" },
        new CountryMaster { Id = 171, CountryName = "Thailand", CountryCode2 = "TH" },
        new CountryMaster { Id = 172, CountryName = "Timor-Leste", CountryCode2 = "TL" },
        new CountryMaster { Id = 173, CountryName = "Togo", CountryCode2 = "TG" },
        new CountryMaster { Id = 174, CountryName = "Tonga", CountryCode2 = "TO" },
        new CountryMaster { Id = 175, CountryName = "Trinidad and Tobago", CountryCode2 = "TT" },
        new CountryMaster { Id = 176, CountryName = "Tunisia", CountryCode2 = "TN" },
        new CountryMaster { Id = 177, CountryName = "Turkey", CountryCode2 = "TR" },
        new CountryMaster { Id = 178, CountryName = "Turkmenistan", CountryCode2 = "TM" },
        new CountryMaster { Id = 179, CountryName = "Tuvalu", CountryCode2 = "TV" },
        new CountryMaster { Id = 180, CountryName = "Uganda", CountryCode2 = "UG" },
        new CountryMaster { Id = 181, CountryName = "Ukraine", CountryCode2 = "UA" },
        new CountryMaster { Id = 182, CountryName = "United Arab Emirates", CountryCode2 = "AE" },
        new CountryMaster { Id = 183, CountryName = "United Kingdom of Great Britain and Northern Ireland", CountryCode2 = "GB" },
        new CountryMaster { Id = 184, CountryName = "United States of America", CountryCode2 = "US" },
        new CountryMaster { Id = 185, CountryName = "Uruguay", CountryCode2 = "UY" },
        new CountryMaster { Id = 186, CountryName = "Uzbekistan", CountryCode2 = "UZ" },
        new CountryMaster { Id = 187, CountryName = "Vanuatu", CountryCode2 = "VU" },
        new CountryMaster { Id = 188, CountryName = "Venezuela (Bolivarian Republic of)", CountryCode2 = "VE" },
        new CountryMaster { Id = 189, CountryName = "Viet Nam", CountryCode2 = "VN" },
        new CountryMaster { Id = 190, CountryName = "Yemen", CountryCode2 = "YE" },
        new CountryMaster { Id = 191, CountryName = "Zambia", CountryCode2 = "ZM" },
        new CountryMaster { Id = 192, CountryName = "Zimbabwe", CountryCode2 = "ZW" },
        new CountryMaster { Id = 193, CountryName = "Abkhazia", CountryCode2 = "AB" },
        new CountryMaster { Id = 194, CountryName = "Cook Islands", CountryCode2 = "CK" },
        new CountryMaster { Id = 195, CountryName = "Western Sahara", CountryCode2 = "EH" },
        new CountryMaster { Id = 196, CountryName = "French Guiana", CountryCode2 = "GF" },
        new CountryMaster { Id = 197, CountryName = "Guadeloupe", CountryCode2 = "GP" },
        new CountryMaster { Id = 198, CountryName = "Guam", CountryCode2 = "GU" },
        new CountryMaster { Id = 199, CountryName = "Hong Kong", CountryCode2 = "HK" },
        new CountryMaster { Id = 200, CountryName = "Jersey", CountryCode2 = "JE" },
        new CountryMaster { Id = 201, CountryName = "North Macedonia", CountryCode2 = "MK" },
        new CountryMaster { Id = 202, CountryName = "Martinique", CountryCode2 = "MQ" },
        new CountryMaster { Id = 203, CountryName = "Niue", CountryCode2 = "NU" },
        new CountryMaster { Id = 204, CountryName = "New Caledonia", CountryCode2 = "NC" },
        new CountryMaster { Id = 205, CountryName = "French Polynesia", CountryCode2 = "PF" },
        new CountryMaster { Id = 206, CountryName = "Puerto Rico", CountryCode2 = "PR" },
        new CountryMaster { Id = 207, CountryName = "Réunion", CountryCode2 = "RE" },
        new CountryMaster { Id = 208, CountryName = "Saint Barthélemy", CountryCode2 = "BL" },
        new CountryMaster { Id = 209, CountryName = "Saint Martin", CountryCode2 = "MF" },
        new CountryMaster { Id = 210, CountryName = "South Georgia and the South Sandwich Islands", CountryCode2 = "GS" },
        new CountryMaster { Id = 211, CountryName = "Svalbard and Jan Mayen", CountryCode2 = "SJ" },
        new CountryMaster { Id = 212, CountryName = "Taiwan", CountryCode2 = "TW" },
        new CountryMaster { Id = 213, CountryName = "Tokelau", CountryCode2 = "TK" },
        new CountryMaster { Id = 214, CountryName = "Wallis and Futuna", CountryCode2 = "WF" },
        new CountryMaster { Id = 215, CountryName = "United States Virgin Islands", CountryCode2 = "VI" },
        new CountryMaster { Id = 216, CountryName = "British Virgin Islands", CountryCode2 = "VG" },
        new CountryMaster { Id = 217, CountryName = "Antarctica", CountryCode2 = "AQ" },
        new CountryMaster { Id = 218, CountryName = "Aruba", CountryCode2 = "AW" },
        new CountryMaster { Id = 219, CountryName = "Bermuda", CountryCode2 = "BM" },
        new CountryMaster { Id = 220, CountryName = "Bouvet Island", CountryCode2 = "BV" },
        new CountryMaster { Id = 221, CountryName = "British Indian Ocean Territory", CountryCode2 = "IO" },
        new CountryMaster { Id = 222, CountryName = "Cayman Islands", CountryCode2 = "KY" },
        new CountryMaster { Id = 223, CountryName = "Christmas Island", CountryCode2 = "CX" },
        new CountryMaster { Id = 224, CountryName = "Cocos (Keeling) Islands", CountryCode2 = "CC" },
        new CountryMaster { Id = 225, CountryName = "Curacao", CountryCode2 = "CW" },
        new CountryMaster { Id = 226, CountryName = "Faroe Islands", CountryCode2 = "FO" },
        new CountryMaster { Id = 227, CountryName = "Falkland Islands (Malvinas)", CountryCode2 = "FK" },
        new CountryMaster { Id = 228, CountryName = "Gibraltar", CountryCode2 = "GI" },
        new CountryMaster { Id = 229, CountryName = "Greenland", CountryCode2 = "GL" },
        new CountryMaster { Id = 230, CountryName = "Guernsey", CountryCode2 = "GG" },
        new CountryMaster { Id = 231, CountryName = "Heard Island and McDonald Islands", CountryCode2 = "HM" },
        new CountryMaster { Id = 232, CountryName = "Isle of Man", CountryCode2 = "IM" },
        new CountryMaster { Id = 233, CountryName = "Montserrat", CountryCode2 = "MS" },
        new CountryMaster { Id = 234, CountryName = "Norfolk Island", CountryCode2 = "NF" },
        new CountryMaster { Id = 235, CountryName = "Pitcairn", CountryCode2 = "PN" },
        new CountryMaster { Id = 236, CountryName = "Saint Helena, Ascension and Tristan da Cunha", CountryCode2 = "SH" },
        new CountryMaster { Id = 237, CountryName = "Saint Pierre and Miquelon", CountryCode2 = "PM" },
        new CountryMaster { Id = 238, CountryName = "Sint Maarten", CountryCode2 = "SX" },
        new CountryMaster { Id = 239, CountryName = "Tokelau", CountryCode2 = "TK" },
        new CountryMaster { Id = 240, CountryName = "Tristan da Cunha", CountryCode2 = "TA" },
        new CountryMaster { Id = 241, CountryName = "Turks and Caicos Islands", CountryCode2 = "TC" },
        new CountryMaster { Id = 242, CountryName = "Saint Martin (Dutch part)", CountryCode2 = "MF" },
        new CountryMaster { Id = 243, CountryName = "United States Minor Outlying Islands", CountryCode2 = "UM" },
        new CountryMaster { Id = 244, CountryName = "Vatican City", CountryCode2 = "VA" },
        new CountryMaster { Id = 245, CountryName = "Montserrat", CountryCode2 = "MS" },
        new CountryMaster { Id = 246, CountryName = "Bonaire", CountryCode2 = "BO" },
            };
        }
        private static (UserMaster[], WalletMaster[], UserDetailsMaster[]) GetUsers()
        {
            var seedDate = new DateTime(2025, 6, 26, 7, 10, 1, 89, DateTimeKind.Utc); // 2024-06-20 12:40:01.089 IST to UTC

            var user1Id = 1;

            var users = new[]
            {
                new UserMaster
                {
                    UserId = user1Id,
                    FirstName = "Prop",
                    LastName = "Admin",
                    FullName = "Prop Admin",
                    Email = "propadmin@yopmail.com",
                    PhoneNumber = "1234567896",
                    Password = "Admin@123",
                    RoleId = RoleType.Admin,
                    KycStatus =RequestStatus.Approved,
                    IBStatus =RequestStatus.Approved,
                    ReferralCode = ReferralType.ReferralCode.GetDescription(),
                    AffiliateCode =ReferralType.AffiliateCode.GetDescription(),
                    ParentId = 0,
                    IsAdmin = true,
                    CreatedDate = seedDate,
                    IsEmailVerified = true,
                    IsActive = true,
                    IsDelete = false,
                    CurrencyCode = "USD"
                }};

            var wallets = new[]
            {
        new WalletMaster
        {
            UserId = user1Id,
            WalletAccount = "WALLET123",
            Balance = 5000m,
            CreatedAt = seedDate
        }};

            var userDetails = new[]
            {
        new UserDetailsMaster
        {
            UserId = user1Id,
            CountryId = 1,
            CountryOfBirthId = 1,
            CountryOfCitizenshipId = 1,
            DateOfBirth = new DateOnly(1990, 1, 1),
            ProfileCompletionPercentage = 50.0,
            CreatedDate = seedDate
        } };

            return (users, wallets, userDetails);
        }*/
        //private static EmailTemplateMaster[] EmailTemplateMasters()
        //{
        //    var seedDate = new DateTime(2025, 6, 26, 7, 10, 1, 89, DateTimeKind.Utc); // 2024-06-20 12:40:01.089 IST to UTC

        //    return new[]
        //    {
        //new EmailTemplateMaster
        //{
        //    Id = 1,
        //    EmailTemplateKey = EmailTemplateKey.LoginUser,
        //    Name = "LoginUser",
        //    Subject = "Login Notification",
        //    HtmlContent = EmailTemplate.LoginUser,
        //    EmailVariable = "FullName,LoginTime,OTPCode,ExpiryOtp",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 2,
        //    EmailTemplateKey = EmailTemplateKey.LoginAdmin,
        //    Name = "LoginAdmin",
        //    Subject = "Admin Login Notification",
        //    HtmlContent =EmailTemplate.LoginAdmin ,
        //    EmailVariable = "FullName,LoginTime,OTPCode,ExpiryOtp",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 3,
        //    EmailTemplateKey = EmailTemplateKey.ResendOTP,
        //    Name = "ResendOTP",
        //    Subject = "Your OTP Code",
        //    HtmlContent = EmailTemplate.ResendOTP,
        //    EmailVariable = "FullName,LoginTime,OTPCode,ExpiryOtp",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 4,
        //    EmailTemplateKey = EmailTemplateKey.ForgotPasswordOTP,
        //    Name = "ForgotPasswordOTP",
        //    Subject = "Password Reset OTP",
        //    HtmlContent = EmailTemplate.ForgotPasswordOTP,
        //    EmailVariable = "FullName,OTPCode,ExpiryOtp",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 5,
        //    EmailTemplateKey = EmailTemplateKey.VerifyEmailUser,
        //    Name = "VerifyEmailUser",
        //    Subject = "Verify Your Email Address",
        //    HtmlContent = EmailTemplate.VerifyEmailUser,
        //    EmailVariable = "FullName,VerificationLink",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 6,
        //    EmailTemplateKey = EmailTemplateKey.SendVerificationEmail,
        //    Name = "SendVerificationEmail",
        //    Subject = "Email Verification Request",
        //    HtmlContent = EmailTemplate.SendVerificationEmail,
        //    EmailVariable = "FullName,VerificationLink",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 7,
        //    EmailTemplateKey = EmailTemplateKey.TwoFactorOTP,
        //    Name = "TwoFactorOTP",
        //    Subject = "Two-Factor Authentication Code",
        //    HtmlContent = EmailTemplate.TwoFactorOTP,
        //    EmailVariable = "FullName,OTPCode,ExpiryOtp",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 8,
        //    EmailTemplateKey = EmailTemplateKey.ChangePassword,
        //    Name = "ChangePassword",
        //    Subject = "Password Changed Successfully",
        //    HtmlContent = EmailTemplate.ChangePassword,
        //    EmailVariable = "FullName,Password",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 9,
        //    EmailTemplateKey = EmailTemplateKey.ForgotPassword,
        //    Name = "ForgotPassword",
        //    Subject = "Reset Your Password",
        //    HtmlContent = EmailTemplate.ForgotPassword,
        //    EmailVariable = "FullName,Password",
        //    CreatedAt = seedDate,
        //},
        //new EmailTemplateMaster
        //{
        //    Id = 10,
        //    EmailTemplateKey = EmailTemplateKey.UserRegister,
        //    Name = "UserRegister",
        //    Subject = "User Register Notification",
        //    HtmlContent = EmailTemplate.UserRegister,
        //    EmailVariable = "FullName,Email,WalletAccNo,TemporaryPassword,VerificationLink,ExpiryMinutes",
        //    CreatedAt = seedDate,
        //}
        //};
        //}
    }
}
