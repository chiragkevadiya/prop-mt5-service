using System;
using System.Globalization;

namespace PropMT5ConnectionService.Utilities
{
    public static class DateFormatConverter
    {

        public static DateTimeOffset FormatDate(string date)
        {
            if (date != "NA" && date != string.Empty)
            {
                string dateString = date;
                CultureInfo provider = CultureInfo.InvariantCulture;

                // The format of the input date string
                //string[] formats = { "MMMM d, yyyy", "MMM d, yyyy", "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy", "MM/dd/yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt" };

                // The format of the input date string
                string[] formats = { "MMMM d, yyyy", "MMM d, yyyy", "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy", "MM/dd/yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "MM-dd-yyyy" };

                DateTime dateTime;
                if (DateTime.TryParseExact(dateString, formats, provider, DateTimeStyles.None, out dateTime))
                {
                    return new DateTimeOffset(dateTime, TimeSpan.Zero);
                }
                else
                {
                    throw new FormatException($"Failed to parse the date. {date}");
                }
            }
            else
            {
                return DateTimeOffset.MinValue;
            }
        }
    }

}
