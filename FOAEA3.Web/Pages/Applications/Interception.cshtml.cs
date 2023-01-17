using FOAEA3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionModel : FoaeaPageModel
{
    public InterceptionModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
    }

    public void OnGet()
    {
    }
}
