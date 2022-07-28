using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Common.Helpers
{
    public static class APIHelper
    {
        //public static void ApplyRequestHeaders(IRepositories repositories, IDictionary<string, StringValues> headers)
        //{
        //    const string CURRENT_SUBMITTER = "CurrentSubmitter";
        //    const string CURRENT_SUBJECT = "CurrentSubject";
        //    const string ACCEPT_LANGUAGE = "Accept-Language";

        //    if (headers.Keys.ContainsCaseInsensitive(CURRENT_SUBMITTER))
        //    {
        //        string key = headers.Keys.First(m => m.Contains(CURRENT_SUBMITTER, StringComparison.InvariantCultureIgnoreCase));
        //        repositories.CurrentSubmitter = headers[key];
        //    }

        //    if (headers.Keys.ContainsCaseInsensitive(CURRENT_SUBJECT))
        //    {
        //        string key = headers.Keys.First(m => m.Contains(CURRENT_SUBJECT, StringComparison.InvariantCultureIgnoreCase));
        //        repositories.CurrentUser = headers[key];
        //    }

        //    if (headers.Keys.ContainsCaseInsensitive(ACCEPT_LANGUAGE))
        //    {
        //        string key = headers.Keys.First(m => m.Contains(ACCEPT_LANGUAGE, StringComparison.InvariantCultureIgnoreCase));
        //        string language = headers[key];

        //        LanguageHelper.SetLanguage(language);
        //    }
        //}

        //public static void PrepareResponseHeaders(IDictionary<string, StringValues> headers)
        //{
        //    headers.Add("Content-Language", LanguageHelper.IsEnglish() ? "en" : "fr");
        //}

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
}
