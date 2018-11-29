using GT.BizTalk.Framework.Core.Tracing;
using System;

namespace GT.BizTalk.Framework.Core.Utilities
{
    public class DateUtil
    {
        /// <summary>
        /// parse DB2 dates using specific format string and return an XSD compatible format
        /// </summary>
        /// <param name="datetimeInput">DB2 dateTime string</param>
        /// <returns></returns>
        public static string Parse_DB2Date_And_FormatAs_XSDDate(string datetimeInput)
        {
            string dateTimeFormat = "yyyy-MM-dd h:mm:ss tt";
            return ParseAndFormatAsXSDDate(datetimeInput, dateTimeFormat);
        }

        /// <summary>
        /// parse datetime string using specific format and return an XSD compatible format
        /// </summary>
        /// <param name="datetimeInput"></param>
        /// <param name="dateTimeFormat"></param>
        /// <returns></returns>
        public static string ParseAndFormatAsXSDDate(string datetimeInput, string dateTimeFormat)
        {
            DateTime parsedDatetime;
            var isParseable = false;
            if (string.IsNullOrEmpty(dateTimeFormat))
                isParseable = DateTime.TryParse(datetimeInput
                , System.Globalization.CultureInfo.CurrentCulture
                , System.Globalization.DateTimeStyles.None
                , out parsedDatetime);
            else
            {
                isParseable = DateTime.TryParseExact(datetimeInput
                , dateTimeFormat
                , System.Globalization.CultureInfo.InvariantCulture
                , System.Globalization.DateTimeStyles.NoCurrentDateDefault
                , out parsedDatetime);
            }
            if (isParseable)
            {
                return parsedDatetime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            }
            else
            {
                TraceProvider.Logger.TraceError(string.Format("Input Date [{0}] is not parsable using format string [{1}]. Returning original input.", datetimeInput, dateTimeFormat));
                return datetimeInput;
            }
        }

        public static string FormatDate(DateTime date, string dateTimeFormat)
        {
            return date.ToString(dateTimeFormat);
        }

        public static string GetCurrentDate(string dateTimeFormat)
        {
            var date = DateTime.Today;
            return date.ToString(dateTimeFormat);
        }

        public static string GetNextWeekDay(string dateTimeFormat)
        {
            return GetNextWeekDay().ToString(dateTimeFormat);
        }

        public static DateTime GetNextWeekDay()
        {
            var date = DateTime.Today.AddDays(1);

            while (true)
            {
                var dateofweek = (int)date.DayOfWeek;
                if (1 <= dateofweek && dateofweek <= 5)
                    return date;
                else
                    date = date.AddDays(1);
            }
        }
    }
}