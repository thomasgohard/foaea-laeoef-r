using System;

namespace FOAEA3.Resources.Helpers
{
    public static class DateTimeHelper
    {
        public const string MM_DD_YYYY_HH_MM_SS = "MM/dd/yyyy HH:mm:ss";
        public const string YYYY_MM_DD_HH_MM_SS = "yyyy/MM/dd HH:mm:ss";
        public const string GRID_DATAFORMATSTRING = "{0:yyyy/MM/dd}";
        public const string GRID_DATETIMEFORMATSTRING = "{0:yyyy/MM/dd HH:mm:ss}";
        public const string SQL_DATE = "MM/dd/yyyy";
        public const string SQL_DATETIME = "MM/dd/yyyy HH:mm:ss";
        public const string YYYY_MM_DD = "yyyy/MM/dd";

        public static string AsJulianString(this DateTime value)
        {
            string dayOfYear = value.DayOfYear.ToString("D3");
            return $"{value.Year}{dayOfYear}";
        }

        public static DateTime ConvertJulianDateStringToDateTime(string flatDate, ref string error)
        {
            if (flatDate.Length == 7)
            {
                try
                {
                    int year = int.Parse(flatDate.Substring(0, 4));
                    int day = int.Parse(flatDate.Substring(4, 3));

                    return new DateTime(year, 1, 1).AddDays(day - 1);
                }
                catch
                {
                    error = $"Invalid date passed: [{flatDate}]";
                    return new DateTime();
                }
            }
            else
                return new DateTime();
        }

        public static int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public static int GetQuarters(DateTime dt1, DateTime dt2)
        {
            double d1Quarter = GetQuarter(dt1.Month);
            double d2Quarter = GetQuarter(dt2.Month);
            double d1 = d2Quarter - d1Quarter;
            double d2 = (4 * (dt2.Year - dt1.Year));
            return (int)Round(d1 + d2);
        }

        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3) return 1;
            if (nMonth <= 6) return 2;
            if (nMonth <= 9) return 3;
            return 4;
        }

        public static DateTime AddQuarter(DateTime baseDateTime, int quarterCount)
        {
            return baseDateTime.AddMonths(quarterCount * 3);
        }

        public static int GetFiscalMonth(DateTime baseDateTime)
        {
            int fiscalMonth = baseDateTime.Month;

            if (fiscalMonth < 4)
                fiscalMonth += 9;
            else
                fiscalMonth -= 3;

            return fiscalMonth;
        }

        public static int GetFiscalYear(DateTime baseDateTime)
        {
            int fiscalYear = baseDateTime.Year;

            if (baseDateTime.Month < 4)
                fiscalYear--;

            return fiscalYear;
        }

        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

        public static bool AreDatesEqual(DateTime date1, DateTime date2)
        {
            // compare two dates but ignore milliseconds

            date1 = date1.AddMilliseconds(0 - date1.Millisecond);
            date2 = date2.AddMilliseconds(0 - date2.Millisecond);

            return date1 == date2;
        }

        public static bool AreDatesEqual(DateTime? date1, DateTime? date2)
        {
            // compare two dates but ignore milliseconds

            if (date1.HasValue)
                date1 = date1.Value.AddMilliseconds(0 - date1.Value.Millisecond);

            if (date2.HasValue)
                date2 = date2.Value.AddMilliseconds(0 - date2.Value.Millisecond);

            return date1 == date2;
        }

    }

}
