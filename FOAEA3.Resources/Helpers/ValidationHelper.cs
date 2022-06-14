using System;
using System.Text.RegularExpressions;

namespace FOAEA3.Resources.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsBaseRegexValid(string textToValidate, string regex)
        {

            bool error = false;

            var regexObj = new Regex(regex);
            if (!regexObj.IsMatch(textToValidate))
                error = true;

            return !error;

        }

        public static bool IsValidEmail(string email, bool isOptional = true)
        {
            if (isOptional && email == string.Empty)
                return true;
            else
                // this is the standard regex based on the example regex found on https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
                return IsBaseRegexValid(email, @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
        }

        public static bool IsValidPhoneNumber(string number) => IsBaseRegexValid(number, @"^([0-9]{3})([-]{1})([0-9]{4})$");

        public static bool IsValidInteger(string number) => int.TryParse(number, out _);

        public static bool IsValidPostalCode(string postalCode)
        {
            return IsBaseRegexValid(postalCode, @"[A-Z]\d[A-Z][\s]?\d[A-Z]\d");
        }

        public static bool IsValidSinNumberMod10(string sin, bool allowEmpty = false)
        {

            sin = sin?.Trim();

            if (string.IsNullOrEmpty(sin) && allowEmpty)
                return true;

            if (string.IsNullOrEmpty(sin) || (sin.Length != 9))
                return false;

            if (!Int64.TryParse(sin, out _))
                return false;

            int valTotalDoubled;
            int valTotalSingle;
            int valGrandTotal;

            // Step 1: Multiply every digit in an even position by 2 (ie. starting with the second digit multiply every other digit by 2).
            int val1 = 2 * int.Parse(sin.Substring(1, 1));
            int val2 = 2 * int.Parse(sin.Substring(3, 1));
            int val3 = 2 * int.Parse(sin.Substring(5, 1));
            int val4 = 2 * int.Parse(sin.Substring(7, 1));

            // Step 2: Add together all of the digits of the numbers multiplied by 2.

            if (val1.ToString().Length > 1)
                val1 = int.Parse(val1.ToString().Substring(0, 1)) + int.Parse(val1.ToString().Substring(1, 1));

            if (val2.ToString().Length > 1)
                val2 = int.Parse(val2.ToString().Substring(0, 1)) + int.Parse(val2.ToString().Substring(1, 1));

            if (val3.ToString().Length > 1)
                val3 = int.Parse(val3.ToString().Substring(0, 1)) + int.Parse(val3.ToString().Substring(1, 1));

            if (val4.ToString().Length > 1)
                val4 = int.Parse(val4.ToString().Substring(0, 1)) + int.Parse(val4.ToString().Substring(1, 1));

            valTotalDoubled = val1 + val2 + val3 + val4;

            // Step 3: Add together all of the digits not multiplied by 2.

            valTotalSingle = int.Parse(sin.Substring(0, 1)) +
                             int.Parse(sin.Substring(2, 1)) +
                             int.Parse(sin.Substring(4, 1)) +
                             int.Parse(sin.Substring(6, 1)) +
                             int.Parse(sin.Substring(8, 1));

            // Step 4: Sum together the results from Step 2 and Step 3.

            valGrandTotal = valTotalDoubled + valTotalSingle;

            // Step 5: Check if the Modulo 10 equals zero

            if ((valGrandTotal % 10) == 0)
                return true;
            else
                return false;

        }

    }
}
