using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FOAEA3.Model.Exceptions;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class GenderData : IComparable<GenderData>
    {
        public string Gender_Cd { get; set; }
        public string Gendr_Txt_E { get; set; }
        public string Gendr_Txt_F { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? Gendr_Txt_E : Gendr_Txt_F;
        }

        public int CompareTo([AllowNull] GenderData other)
        {
            Dictionary<string, int> sortOrder = new Dictionary<string, int>
                {
                    { "M", 1}, // male
                    { "F", 2}, // female
                    { "X", 3}, // another gender
                    { "I", 4}  // information not available
                };

            int result;

            if (!sortOrder.ContainsKey(Gender_Cd))
                throw new GenderException($"Invalid gender code: {Gender_Cd}");

            if (!sortOrder.ContainsKey(other.Gender_Cd))
                throw new GenderException($"Invalid gender code: {other.Gender_Cd}");

            if (Gender_Cd == other.Gender_Cd)
                result = 0;
            else if (sortOrder[Gender_Cd] < sortOrder[other.Gender_Cd])
                result = -1;
            else
                result = 1;

            return result;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GenderData other))
            {
                return false;
            }
            return this.CompareTo(other) == 0;
        }
        
        public static bool operator ==(GenderData left, GenderData right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static int Compare(GenderData left, GenderData right)
        {
            return left.CompareTo(right);
        }

        public static bool operator >(GenderData left, GenderData right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator >=(GenderData left, GenderData right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <(GenderData left, GenderData right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator <=(GenderData left, GenderData right)
        {
            return Compare(left, right) <= 0;
        }

        public static bool operator !=(GenderData left, GenderData right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Gender_Cd, Gendr_Txt_E, Gendr_Txt_F);
        }
    }

}
