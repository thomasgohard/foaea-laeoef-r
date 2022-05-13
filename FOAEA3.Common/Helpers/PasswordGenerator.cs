using System.Security.Cryptography;

namespace FOAEA3.Common.Helpers
{
    using System;
    using System.Text.RegularExpressions;

    public partial class PasswordGenerator
    {

        /* TODO ERROR: Skipped RegionDirectiveTrivia */        // Define default min and max password lengths.
        private static readonly int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static readonly int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static readonly string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static readonly string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static readonly string PASSWORD_CHARS_NUMERIC = "23456789";
        private static readonly string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        /* TODO ERROR: Skipped EndRegionDirectiveTrivia *//* TODO ERROR: Skipped RegionDirectiveTrivia */

        public static bool IsEmail(string email)
        {
            // for exchange server, all these special characters are valid:  !#$%&'*+-/=?^_`{|}~
            // CR723 asking for allowing the apostrophe ' and other characters ??
            //string MatchEmailPattern = @"^(([\w-'!#$%&*-?^_~]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-'!#$%&*-?^_~]{2,}))@" + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?  " + @"[0-9]{1,2}|25[0-5]|2[0-4][0-9])\." + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?" + "[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|" + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            // this is the standard regex based on the example regex found on https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
            string regexEmailPattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";



            if (!((email ?? "") == (string.Empty ?? "")))
            {
                return Regex.IsMatch(email ?? "", regexEmailPattern);
            }
            else
            {
                return false;
            }
        }
       
        public static string GenerateGUID()
        {
            string GenerateGUIDRet = Guid.NewGuid().ToString();
            return GenerateGUIDRet;
        }
        
        public static string Generate()
        {
            string GenerateRet = Generate(DEFAULT_MIN_PASSWORD_LENGTH, DEFAULT_MAX_PASSWORD_LENGTH);
            return GenerateRet;
        }

        public static string Generate(int length)
        {
            string GenerateRet = Generate(length, length);
            return GenerateRet;
        }
        
        public static string Generate(int minLength, int maxLength)

        {

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            var charGroups = new char[][] { PASSWORD_CHARS_LCASE.ToCharArray(), PASSWORD_CHARS_UCASE.ToCharArray(), PASSWORD_CHARS_NUMERIC.ToCharArray(), PASSWORD_CHARS_SPECIAL.ToCharArray() };

            // Use this array to track the number of unused characters in each
            // character group.
            var charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            int I;
            var loopTo = charsLeftInGroup.Length - 1;
            for (I = 0; I <= loopTo; I++)
                charsLeftInGroup[I] = charGroups[I].Length;

            // Use this array to track (iterate through) unused character groups.
            var leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            var loopTo1 = leftGroupsOrder.Length - 1;
            for (I = 0; I <= loopTo1; I++)
                leftGroupsOrder[I] = I;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            var randomBytes = new byte[4];

            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = (randomBytes[0] & 0x7F) << 24 | randomBytes[1] << 16 | randomBytes[2] << 8 | randomBytes[3];

            // Now, this is real randomization.
            var random = new Random(seed);

            // This array will hold password characters.
            char[] password;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
            {
                password = new char[random.Next(minLength - 1, maxLength) + 1];
            }
            else
            {
                password = new char[minLength];
            }

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            var loopTo2 = password.Length - 1;
            for (I = 0; I <= loopTo2; I++)
            {

                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                {
                    nextLeftGroupsOrderIdx = 0;
                }
                else
                {
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);
                }

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                {
                    nextCharIdx = 0;
                }
                else
                {
                    nextCharIdx = random.Next(0, lastCharIdx + 1);
                }

                // Add this character to the password.
                password[I] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx] = charsLeftInGroup[nextGroupIdx] - 1;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
    
}
