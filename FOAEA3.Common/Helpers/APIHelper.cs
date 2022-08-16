using FOAEA3.Model;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.Common.Helpers;

public static class APIHelper
{
    public static bool ValidateApplication(ApplicationData application, ApplKey applKey, out string error)
    {
        error = string.Empty;

        if (application is null)
        {
            error = "Missing or invalid request body.";
            return false;
        }

        application.Appl_EnfSrv_Cd = application.Appl_EnfSrv_Cd.Trim();
        application.Appl_CtrlCd = application.Appl_CtrlCd.Trim();

        if (applKey is not null)
            if ((applKey.EnfSrv != application.Appl_EnfSrv_Cd) || (applKey.CtrlCd != application.Appl_CtrlCd))
            {
                error = "Key does not match body.";
                return false;
            }

        return true;
    }
}
