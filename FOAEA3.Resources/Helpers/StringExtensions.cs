using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace FOAEA3.Resources.Helpers
{
    public static class StringExtensions
    {

        public static bool IsAlphanumeric(this string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]+$");
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string ShrinkInternalSpaces(this string value)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            return regex.Replace(value, " ");
        }

        public static int ConvertToInteger(this string valueFromLine)
        {
            if (!int.TryParse(valueFromLine, out int result))
                result = 0;

            return result;
        }

        public static bool ConvertToBool(this string valueFromLine)
        {
            if (valueFromLine == "1")
                return true;
            else
                return false;
        }

        public static short? ConvertToShortOrNull(this string value)
        {
            if (short.TryParse(value, out short result))
                return result;
            else
                return null;
        }

        public static short ConvertToShort(this string value)
        {
            if (short.TryParse(value, out short result))
                return result;
            else
                return 0;
        }

        public static int? ConvertToIntOrNull(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (int.TryParse(value, out int result))
                return result;
            else
                return null;
        }

        public static decimal? ConvertToDecimalOrNull(this string value)
        {
            if (decimal.TryParse(value, out decimal result))
                return result / 100M;
            else
                return null;
        }

        public static string AppendToPath(this string basePath, string additionalPath, bool isFileName = false)
        {
            string result = basePath;

            if (string.IsNullOrEmpty(result))
                result = additionalPath;
            else
            {
                if (!result.EndsWith("\\"))
                    result += "\\";

                result += additionalPath;
                if (!result.EndsWith("\\") && !isFileName)
                    result += "\\";
            }

            return result;
        }

        public static string ExtractSubfolder(this string basePath)
        {
            string result = basePath;

            if (result.EndsWith("\\"))
                result = result[..^1];

            int posLastBackslash = result.LastIndexOf('\\');
            if (posLastBackslash > -1)
                result = result[(posLastBackslash + 1)..];

            return result;
        }

        public static string ReplaceVariablesWithEnvironmentValues(this string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            data = data.Replace("%%", "%|||%"); // needed to handle two variables next to each other

            var result = data.GetEnvironmentVariablesAndValues();

            foreach (var (oldValue, newValue) in result)
                data = data.Replace($"%{oldValue}%", $"{newValue}");

            return data.Replace("|||", "");
        }


        private static Dictionary<string, string> GetEnvironmentVariablesAndValues(this string data)
        {
            var results = new Dictionary<string, string>();

            var tokens = data.Split("%", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 1)
            {
                for (int i = 1; i < tokens.Length; i += 2) // only do odd ones
                {
                    string variable = tokens[i];
                    string value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
                    if (string.IsNullOrEmpty(value))
                        value = Environment.GetEnvironmentVariable(variable); 
                    results.Add(variable, value);
                }
            }
            else if (tokens.Length == 1)
            {
                string variable = tokens[0];
                string value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(value))
                    value = Environment.GetEnvironmentVariable(variable);
                results.Add(variable, value);
            }

            return results;
        }

        public static string FixApostropheForSQL(this string value)
        {
            string result = value.Replace("''", "'");
            result = result.Replace("'", "''");

            return result;
        }

        public static string FixWildcardForSQL(this string value)
        {
            if (value.Contains('*'))
                return value.Replace("*", "%");
            else
                return value;
        }

        public static bool ContainsCaseInsensitive(this ICollection<string> keys, string value)
        {
            return keys.Any(m => m.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public static T Convert<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(input);
                }
                return default;
            }
            catch (Exception)
            {
                // log error?

                return default;
            }
        }

        public static DateTime? ConvertToDateTimeIgnoringTimeZone(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int pos = value.LastIndexOf("-");
                if (value.Contains('T') && (pos > 0))
                {
                    string valueWithoutTimezone = value[..pos];
                    if (DateTime.TryParse(valueWithoutTimezone, out DateTime result))
                        return result;
                }
                else if (DateTime.TryParse(value, out DateTime result))
                    return result;
            }

            return null;
        }

    }
}
