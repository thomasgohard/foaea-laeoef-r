using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionModel : FoaeaPageModel
{
    public List<MessageData> ErrorMessage { get; set; }
    public List<MessageData> WarningMessage { get; set; }
    public List<MessageData> InfoMessage { get; set; }

    [BindProperty]
    public InterceptionApplicationData InterceptionApplication { get; set; }

    public InterceptionModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
        {
            InterceptionApplication = new InterceptionApplicationData
            {
                Appl_EnfSrv_Cd = httpContextAccessor.HttpContext.Session.GetString(SessionValue.ENF_SERVICE),
                Subm_SubmCd = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER)
            };
        }
    }

    public void OnGet()
    {

    }

    public async Task OnPostSubmitNewApplication()
    {
        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var newApplication = await interceptionApi.CreateInterceptionApplicationAsync(InterceptionApplication);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            ErrorMessage = newApplication.Messages.GetMessagesForType(MessageType.Error);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Warning))
            WarningMessage = newApplication.Messages.GetMessagesForType(MessageType.Warning);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Information))
            InfoMessage = newApplication.Messages.GetMessagesForType(MessageType.Information);
    }

}
