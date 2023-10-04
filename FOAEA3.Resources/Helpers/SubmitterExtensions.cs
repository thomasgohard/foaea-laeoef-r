namespace FOAEA3.Resources.Helpers
{
    public static class SubmitterExtensions
    {         
        public static bool IsInternalAgentSubmitter(this string submCd)
        {
            if (submCd?.Length > 2)
                return (submCd.ToUpper()[..2] == "FO");
            else
                return false;
        }

        public static bool IsCourtSubmitter(this string submCd)
        {
            if (submCd?.Length > 3)
                return (submCd.ToUpper()[2] == 'C');
            else
                return false;
        }

        public static bool IsPeaceOfficerSubmitter(this string submCd)
        {
            if (submCd?.Length > 3)
                return (submCd.ToUpper()[2] == 'P');
            else
                return false;
        }

        public static bool IsProvincialChildSupportServicesSubmitter(this string submCd)
        {
            if (submCd?.Length > 3)
                return (submCd.ToUpper()[2] == 'S');
            else
                return false;
        }
    }
}
