namespace FOAEA3.Resources.Helpers
{
    public static class UserExtensions
    {         
        public static bool IsInternalUser(this string submCd)
        {
            if (submCd?.Length > 2)
                return (submCd[..2] == "FO");
            else
                return false;
        }
    }
}
