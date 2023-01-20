using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionModel : FoaeaPageModel
{
    public List<MessageData> ErrorMessage { get; set; }
    public List<MessageData> WarningMessage { get; set; }
    public List<MessageData> InfoMessage { get; set; }

    public string EnfServiceDescription { get; set; }

    [BindProperty]
    public InterceptionApplicationData InterceptionApplication { get; set; }

    public InterceptionModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
        {
            string submitter = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER);

            InterceptionApplication = new InterceptionApplicationData
            {
                Appl_EnfSrv_Cd = httpContextAccessor.HttpContext.Session.GetString(SessionValue.ENF_SERVICE),
                Subm_SubmCd = submitter
            };

            InterceptionApplication.HldbCnd.Add(new HoldbackConditionData());

            var submitterProfileApi = new SubmitterProfileAPIBroker(BaseAPIs);
            EnfServiceDescription = submitterProfileApi.GetSubmitterProfileAsync(submitter).Result.EnfSrv_Nme;
        }
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostSubmitNewApplication()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = GetValidationErrors();

            return Page();
        }

        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var newApplication = await interceptionApi.CreateInterceptionApplicationAsync(InterceptionApplication);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Error))
            ErrorMessage = newApplication.Messages.GetMessagesForType(MessageType.Error);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Warning))
            WarningMessage = newApplication.Messages.GetMessagesForType(MessageType.Warning);

        if (newApplication.Messages.ContainsMessagesOfType(MessageType.Information))
            InfoMessage = newApplication.Messages.GetMessagesForType(MessageType.Information);

        InterceptionApplication = newApplication;

        if (!InterceptionApplication.HldbCnd.Any())
            InterceptionApplication.HldbCnd.Add(new HoldbackConditionData());

        return Page();
    }

    private List<MessageData> GetValidationErrors()
    {
        var errorMessages = new List<MessageData>();
        foreach (var value in ModelState.Values)
        {
            if (value.Errors.Any())
                foreach (var error in value.Errors)
                    errorMessages.Add(new MessageData(EventCode.UNDEFINED, null, error.ErrorMessage, MessageType.Error));
        }

        return errorMessages;
    }
}
