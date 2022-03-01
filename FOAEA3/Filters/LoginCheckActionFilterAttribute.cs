using FOAEA3.Model;
using FOAEA3.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FOAEA3.Filters
{

    public class LoginCheckActionFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var _currentSession = SessionHelper.Current;

            //bool isUserLoggedIn = _currentSession.Session.GetString(LoginData.FOAEA_User) != null;
            //string originalRoute = filterContext.HttpContext.Request.Path;

            //if (!isUserLoggedIn)
            //{


            //    if (originalRoute.ToUpper().Contains("CONFIRMRESET"))
            //    {
            //        originalRoute = originalRoute.Substring(0, originalRoute.LastIndexOf("/"));
            //    }
            //    if ((originalRoute != @"/")

            //        && (originalRoute != @"/Home/ChooseLanguage")
            //        && (originalRoute != @"/Home/SetLanguage")
            //        && (originalRoute != @"/Home/GetDBInfo")
            //        && (originalRoute != @"/Home/ResetPassword")
            //        && (originalRoute != @"/Home/ConfirmReset")
            //        && (originalRoute != @"/Home/ChangePasswordEntry")
            //        && (originalRoute != @"/Home/ChangePasswordVerification"))
            //    {

            //        filterContext.HttpContext.Response.Redirect("/");

            //    }

            //}
            //else
            //{
            //    //bool termsViewed = ((string)currentSession[LoginData.TERMS_VIEWED] == "TRUE");
            //    bool termsViewed = _currentSession.Session.GetString(LoginData.TERMS_VIEWED) == "TRUE";

            //    //_currentSession.Get<bool>(LoginData.TERMS_VIEWED)

            //    if (!termsViewed)
            //    {
            //        if (!originalRoute.Contains("TermsOfReference"))
            //        {
            //            filterContext.HttpContext.Response.Redirect(@"/Home/TermsOfReference");
            //        }
            //    }
            //    else
            //    {
            //        if (originalRoute.Contains("TermsOfReference"))
            //        {
            //            //string sendingPath = filterContext.HttpContext.Request.UrlReferrer.LocalPath;
            //            string sendingPath = filterContext.HttpContext.Request.Headers["Referer"].ToString();
            //            if (sendingPath.Contains("TermsOfReference"))
            //            {
            //                filterContext.HttpContext.Response.Redirect(@"/");

            //            }
            //        }
            //    }
            //}


           // base.OnActionExecuting(filterContext);

        }
    }

}

