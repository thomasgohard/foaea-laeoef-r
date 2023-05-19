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
    }
}
