using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace FOAEA3.Filters
{
    public class SessionEndFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext.Session == null)// ||
            //     //!filterContext.HttpContext.Session.TryGetValue("ID", out byte[] val))
            //{
            //    filterContext.Result =
            //        new RedirectToRouteResult(new RouteValueDictionary(new
            //        {
            //            controller = "Home",
            //            action = "Login"
            //        }));
            //}
            //base.OnActionExecuting(filterContext);

            //HttpContext ctx = SessionHelper.Current;

            //if (ctx.Session.Id == null)
            //{
            //    //filterContext.HttpContext.Response.Redirect("/")
            //    filterContext.HttpContext.Response.Redirect(@"/Home/Index");
            //    return;
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}
