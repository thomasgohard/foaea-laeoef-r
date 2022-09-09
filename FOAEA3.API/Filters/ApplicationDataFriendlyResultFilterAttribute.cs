using AutoMapper;
using FOAEA3.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FOAEA3.API.Filters
{
    public class ApplicationDataFriendlyResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context,
                                                    ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;

            if (resultFromAction is null || resultFromAction is { StatusCode: < 200 or >= 300 })
            {
                await next();
                return;
            }

            var actionPath = context.HttpContext.Request.Path;
            if (actionPath.HasValue && actionPath.Value.Contains("/friendly")) {
                var mapper = context.HttpContext.RequestServices.GetRequiredService<IMapper>();
                resultFromAction.Value = mapper.Map<ApplicationDataFriendly>(resultFromAction.Value);
            }

            await next();
        }
    }
}
