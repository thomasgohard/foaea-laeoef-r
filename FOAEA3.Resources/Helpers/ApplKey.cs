namespace FOAEA3.Resources.Helpers
{
    public class ApplKey
    {
        public string EnfSrv { get; set; } = string.Empty;
        public string CtrlCd { get; set; } = string.Empty;

        public ApplKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                string[] values = key.Split("-");
                if (values.Length == 2)
                {
                    EnfSrv = values[0]?.ToUpper();
                    CtrlCd = values[1]?.ToUpper();
                }
            }
        }

        public override string ToString()
        {
            return MakeKey(EnfSrv, CtrlCd);
        }

        public static string MakeKey(string enfSrv, string ctrlCd)
        {
            return enfSrv.Trim() + "-" + ctrlCd.Trim();
        }
    }
}
