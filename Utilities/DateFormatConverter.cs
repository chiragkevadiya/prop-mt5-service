using PropMT5Service.Constants;
using System;
using System.Globalization;

namespace PropMT5Service.Utilities
{
    public static class DateFormatConverter
    {
        public static DateTimeOffset FormatDate(string date)
        {
            if (date == MT5Constants.DateFormats.NotAvailable || date == string.Empty)
                return DateTimeOffset.MinValue;

            if (DateTime.TryParseExact(date, MT5Constants.DateFormats.SupportedFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                return new DateTimeOffset(dateTime, TimeSpan.Zero);

            throw new FormatException($"Failed to parse the date. {date}");
        }
    }
}
