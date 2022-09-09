using FOAEA3.Common.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Common.Helpers
{
    [ModelBinder(BinderType = typeof(ApplKeyModelBinder))]
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
                    EnfSrv = values[0]?.ToUpper()?.Trim();
                    CtrlCd = values[1]?.ToUpper()?.Trim();
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
