using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace FOAEA3.Common.Filters
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

                var verbMethod = context.HttpContext.Request.HttpContext.Request.Method;
                var actionName = context.RouteData.Values["action"];
                var controllerName = context.RouteData.Values["controller"];

                string info = $"({verbMethod}) controller {controllerName} action {actionName}";
                log.Information(info);

                string infoColour = $"([magenta]{verbMethod}[/magenta]) controller [cyan]{controllerName}[/cyan] action [cyan]{actionName}[/cyan]";
                ColourConsole.WriteEmbeddedColorLine(infoColour);
            }
            catch (Exception e)
            {
                ColourConsole.WriteLine("Error writing OnActionExecuting() to log: " + e.Message, ConsoleColor.Red);
            }

        }
    }
}
