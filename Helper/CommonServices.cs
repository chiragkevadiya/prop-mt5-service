using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using static PropMT5ConnectionService.Helper.Constant;


namespace PropTradingMT5.Helpers
{
    public static class CommonServices
    {
        public static List<string> ToUrlList(this string commaSeparatedUrls)
        {
            return (commaSeparatedUrls ?? string.Empty)
                .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
        }
        public static string[] SplitImageUrls(string imageUrls)
        {
            return (imageUrls ?? string.Empty)
                .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                .Select(url => url.Trim())
                .ToArray(); // ✅ returns string[]
        }
        public static List<string> SplitFileNames(string fileNames)
        {
            if (string.IsNullOrWhiteSpace(fileNames))
                return new List<string>();

            return fileNames
                .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f))
                .ToList();
        }


        public static class FileSize
        {
            public const long MinFileSizeBytes = 1 * 1024;              // 1 KB
            public const long MaxFileSizeBytes = 28 * 1024 * 1024;       // 28 MB
            public const long MaxVideoSizeBytes = 104 * 1024 * 1024;    // 104 MB
        }
        public static class FileUpload
        {
            // Upload folder names (do not change these)
            public const string UploadFiles = "UploadFiles"; // Root folder
            public const string KYCDocument = "KYCDocument";
            public const string BannerImage = "BannerImage";
            public const string EmailAttachments = "EmailAttachment";
            public const string SupportTicket = "SupportTicket";
            public const string ProfileImage = "ProfileImage";
            public const string TutorialVideo = "TutorialVideo";
            public const string PromotionKitImage = "PromotionKitImage";
            public const string ChatDocument = "ChatDocument";
            public const string SourceImage = "SourceImage";
            public const string KYCPDFDocument = "KYCPDFDocument";
            public const string SignatureImage = "SignatureImage";
            public const string LeadDocument = "LeadDocument";
            public const string DepositDocument = "DepositDocument";

            // File extensions
            public static readonly string[] EmailExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            public static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
            public static readonly string[] ProfileImageExtensions = { ".jpg", ".jpeg", ".png" };
            public static readonly string[] KYCExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            public static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".webm" };
            public static readonly string[] PromotionKitExtensions = { ".jpg", ".jpeg", ".png" };
            public static readonly string[] BannerExtensions = { ".jpg", ".jpeg", ".png" };
            public static readonly string[] SourceImageExtensions = { ".jpg", ".jpeg", ".png" };
            public static readonly string[] LeadDocumentExtensions = { ".xls", ".xlsx" };
            public static readonly string[] SupportTicketExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
            public static readonly string[] DepositDocumentExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

        }
    }

    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault()?.Description ?? value.ToString();
        }
        public static List<EnumItem> GetEnumValues<T>() where T : Enum
        {
            // Get all enum values and convert them to a list of EnumItem
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new EnumItem
                       {
                           Name = e.ToString(),
                           Value = Convert.ToInt32(e)
                       }).ToList();
        }

        public static string GetDescriptionByValue<T>(int value) where T : Enum
        {
            var enumValue = Enum.GetValues(typeof(T))
                                .Cast<T>()
                                .FirstOrDefault(e => Convert.ToInt32(e) == value);

            return (enumValue as Enum)?.GetDescription() ?? value.ToString();
        }
    }
    public class EnumItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
