using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DBHelper
{
    public static class SqlStyleExtensions
    {
        public static bool IsBetween(this DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt < end;
        }

        public static bool In<T>(this T source, params T[] items)
        {
            return items.Contains(source);
        }

        public static bool NotIn<T>(this T source, params T[] items)
        {
            return !items.Contains(source);
        }

        public static bool IsValidOrderByClause(this string orderByClause)
        {
            return Regex.IsMatch(orderByClause, @"^[a-zA-Z]+[a-zA-Z ,]*$");
        }

        public static bool IsValidSqlDatetime(this string dateTimeValue)
        {
            bool isValid = false;

            if (DateTime.TryParse(dateTimeValue, out DateTime testDate))
                isValid = testDate.IsValidSqlDatetime();

            return isValid;
        }

        public static bool IsValidSqlDatetime(this DateTime dateTimeValue)
        {
            bool isValid = false;

            var minDateTime = new DateTime(1753, 1, 1);
//            var maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);

            if (dateTimeValue >= minDateTime)
                isValid = true;

            return isValid;
        }
    }
}
