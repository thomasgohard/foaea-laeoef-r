using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;

namespace BackendProcess.API.Filters
{
    public class ActionAutoLoggerFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do nothing
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                ILogger log = Log.ForContext("APIpath", context.HttpContext.Request.Path.Value);

                //var context = HttpContext.ApplicationInstance.Context;
                var verbMethod = context.HttpContext.Request.HttpContext.Request.Method;
                var actionName = context.RouteData.Values["action"];

                log.Information($"({verbMethod}) method {actionName}()");
            }
            catch (Exception)
            {
                // do nothing -- logging is not critical
            }

        }
    }
}
