using PropMT5ConnectionService.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Models
{
    public class UserMaster : BaseEntityCreatedModifiedDeleted
    {
        [Key]
        public long UserId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public long ParentId { get; set; }
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public RoleType RoleId { get; set; }
        public string AffiliateCode { get; set; }
        public string ReferralCode { get; set; }
        public RequestStatus KycStatus { get; set; } = RequestStatus.NotApplied;
        public RequestStatus IBStatus { get; set; } = 0;

        [Required, StringLength(3), RegularExpression(@"^[A-Z]{3}$")]
        public string CurrencyCode { get; set; } = "USD";
        public TwoFactorStatus TwoFactorStatus { get; set; } = TwoFactorStatus.Disabled;
        public string TwoFactorOtp { get; set; } = string.Empty;
        public DateTime? TwoFactorOtpCreatedAt { get; set; }
        public DateTime? TwoFactorOtpExpiresAt { get; set; }
        public bool FirstTimeDeposit { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public bool IsMobileVisible { get; set; } = false;
        public string Otp { get; set; } = string.Empty;
        public DateTime? OtpCreatedAt { get; set; }
        public DateTime? OtpExpiresAt { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string EmailVerificationToken { get; set; } = string.Empty;
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public bool ClientVerify { get; set; }

    }
}
