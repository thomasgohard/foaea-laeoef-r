namespace FOAEA3.Resources.Helpers
{
    public static class UserHelper
    {         
        public static bool IsInternalUser(this string submCd)
        {
            if (submCd.Length > 2)
                return (submCd.Substring(0, 2) == "FO");
            else
                return false;
        }
    }
}
