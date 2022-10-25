using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FOAEA3.Common.Filters;

public class ActionProcessHeadersFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // do nothing
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var repositories = context.HttpContext.RequestServices.GetService<IRepositories>();

        var user = context.HttpContext.User;

        ProcessRequestHeaders(context.HttpContext.Request.Headers, user.Claims, repositories);
        ProcessResponseHeaders(context.HttpContext.Response.Headers);
    }

    private static void ProcessRequestHeaders(IDictionary<string, StringValues> headers,
                                              IEnumerable<Claim> claims,
                                              IRepositories repositories)
    {
        const string ACCEPT_LANGUAGE = "Accept-Language";

        var userName = claims?.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
        var submitter = claims?.Where(m => m.Type == "Submitter").FirstOrDefault()?.Value;

        if (repositories is not null)
        {
            repositories.CurrentUser = userName;
            repositories.CurrentSubmitter = submitter;
        }

        if (headers.Keys.ContainsCaseInsensitive(ACCEPT_LANGUAGE))
        {
            string key = headers.Keys.First(m => m.Contains(ACCEPT_LANGUAGE, StringComparison.InvariantCultureIgnoreCase));
            string language = headers[key];

            LanguageHelper.SetLanguage(language);
        }
    }

    private static void ProcessResponseHeaders(IDictionary<string, StringValues> headers)
    {
        headers.Add("Content-Language", LanguageHelper.IsEnglish() ? "en" : "fr");
    }
}
