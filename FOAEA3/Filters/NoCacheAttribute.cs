using Microsoft.AspNetCore.Mvc.Filters;

namespace FOAEA3.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            //filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            //filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            //filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //filterContext.HttpContext.Response.Cache.SetNoStore();

            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";


            base.OnResultExecuting(filterContext);
        }
    }
}
