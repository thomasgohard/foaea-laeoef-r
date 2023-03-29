using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Web.Helpers;
using FOAEA3.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications.Dashboard;

public class InterceptionDashboardModel : FoaeaPageModel
{
    public List<ApplicationSearchResultData> SearchResults { get; set; }
    public List<EnfOffData> EnfOffices { get; set; }
    public List<string> ValidSubmitterCodes { get; set; }

    public int SubmittedToday { get; set; }
    public int SinPendingsLast7days { get; set; }
    public int SinPendingsAll { get; set; }
    public int SinNotConfirmedAll { get; set; }
    public int ApprovedAll { get; set; }
    public int CancelledAll { get; set; }
    public int RejectedAll { get; set; }
    public int ReversedAll { get; set; }
    public int SubmittedVariationsAll { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData SearchCriteria { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

    [BindProperty]
    public List<string> SelectedMenuOption { get; set; } = new List<string>();

    [BindProperty]
    public ActionReasonData SuspendData { get; set; } = new ActionReasonData();
    
    [BindProperty]
    public ActionReasonData CancelData { get; set; } = new ActionReasonData();
    
    [BindProperty]
    public TransferData TransferData { get; set; } = new TransferData();

    public InterceptionDashboardModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        if (LifeStates is null)
        {
            var apiRefBroker = new ApplicationLifeStatesAPIBroker(BaseAPIs);
            LifeStates = apiRefBroker.GetApplicationLifeStatesAsync().Result;
        }

        if (EnfOffices is null)
        {
            var apiSubmitters = new SubmitterAPIBroker(BaseAPIs);
            var submitterInfo = apiSubmitters.GetSubmitterAsync(CurrentSubmitter).Result;
            var apiEnfOffices = new EnfOfficesAPIBroker(BaseAPIs);
            EnfOffices = apiEnfOffices.GetEnfOfficesForEnfServiceAsync(submitterInfo.EnfSrv_Cd).Result;
            EnfOffices = EnfOffices.OrderBy(m => m.EnfSrv_Nme).ToList();
        }

        if (SearchCriteria is null)
        {
            SearchCriteria = new ApplicationSearchCriteriaData
            {
                Category = "I01" // default
            };
        }
    }

    public async Task<JsonResult> OnGetSelectSubmitterForOffice(string service, string office)
    {
        var apiSubmitters = new SubmitterAPIBroker(BaseAPIs);

        var submitters = await apiSubmitters.GetSubmitterCodesForOffice(service, office);
        submitters.Sort();
        ValidSubmitterCodes = submitters;

        return new JsonResult(ValidSubmitterCodes);
    }

    public async Task OnGet()
    {
        await RefreshPanels();
    }

    private async Task RefreshPanels()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);

        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);
        var changesForSubmitterToday = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 0);
        var changesForSubmitterLast7days = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 7);

        var sinPendingsForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.SIN_CONFIRMATION_PENDING_3);
        var sinNotConfirmedForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.SIN_NOT_CONFIRMED_5);
        var applCancelledForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.MANUALLY_TERMINATED_14);
        var applRejectedForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.APPLICATION_REJECTED_9);
        var applSubmittedVariationsAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19);

        var applApprovedForSubmitterAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C50780_APPLICATION_ACCEPTED);

        var applAcceptedReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51106_APPLICATION_ACCEPTED_REVERSED);
        var applCancelReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51108_CANCELLATION_REVERSED);
        var applRejectionReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51107_REJECTION_REVERSED);
        var applVariationReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51109_VARIATION_ACCEPTED_IN_ERROR_ACCEPTANCE_REVERSED);

        SubmittedToday = changesForSubmitterToday.Where(m => m.Appl_Create_Usr == currentSubmitter &&
                                                             m.Appl_Create_Dte.Date == DateTime.Now.Date).Count();

        SinPendingsLast7days = changesForSubmitterLast7days.Where(m => m.AppLiSt_Cd == ApplicationState.SIN_CONFIRMATION_PENDING_3).Count();
        SinPendingsAll = sinPendingsForSubmitterAll.Count;
        SinNotConfirmedAll = sinNotConfirmedForSubmitterAll.Count;
        SubmittedVariationsAll = applSubmittedVariationsAll.Count;
        ApprovedAll = applApprovedForSubmitterAll.Count;
        CancelledAll = applCancelledForSubmitterAll.Count;
        RejectedAll = applRejectedForSubmitterAll.Count;
        ReversedAll = applAcceptedReversalAll.Count + applCancelReversalAll.Count + applRejectionReversalAll.Count + 
                      applVariationReversalAll.Count;
    }

    public IActionResult OnPostMenuSelect()
    {
        string itemSelected = SelectedMenuOption.Where(m => !m.EndsWith(MenuActionChoice.Menu.ToString())).FirstOrDefault();
        if (itemSelected is not null)
        {
            var actionInfo = ActionHelper.ExtractInfo(itemSelected);

            string applKey = $"{actionInfo.Appl_EnfSrv_Cd}-{actionInfo.Appl_CtrlCd}";

            switch (actionInfo.Action)
            {
                case MenuActionChoice.Notes:
                    break;

                case MenuActionChoice.ViewEvents:
                    return Redirect(string.Format(PageRoute.VIEW_EVENTS, applKey));

                case MenuActionChoice.View:
                    return Redirect(string.Format(PageRoute.INTERCEPTION_VIEW, applKey));

                case MenuActionChoice.Edit:
                    return Redirect(string.Format(PageRoute.INTERCEPTION_EDIT, applKey));

                case MenuActionChoice.LinkT01:
                    break;

                case MenuActionChoice.LinkI01:
                    break;

                case MenuActionChoice.LinkL01:
                    break;
            }
        }

        return RedirectToPage();
    }

    public async Task OnPostSearchSubmittedToday()
    {
        await BuildSearchResult(await GetCreatedToday());
    }

    public async Task OnPostSearchSinPendingsLast7days()
    {
        await BuildSearchResult(await GetSinPendingsLast7days());
    }

    public async Task OnPostSearchSinPendingsAll()
    {
        await BuildSearchResult(await GetSinPendingsAll());
    }

    public async Task OnPostSearchApprovedAll()
    {
        await BuildSearchResult(await GetApprovedAll());
    }

    public async Task OnPostSearchCancelledAll()
    {
        await BuildSearchResult(await GetCancelledAll());
    }

    public async Task OnPostSearchRejectedAll()
    {
        await BuildSearchResult(await GetRejectedAll());
    }
    
    public async Task OnPostSearchSubmittedVariationsAll()
    {
        await BuildSearchResult(await GetSubmittedVariationsAll());
    }
    
    public async Task OnPostSearchReverseAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);

        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var applAcceptedReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51106_APPLICATION_ACCEPTED_REVERSED);
        var applCancelReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51108_CANCELLATION_REVERSED);
        var applRejectionReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51107_REJECTION_REVERSED);
        var applVariationReversalAll = await apiSubmitterBroker.GetAllWithEvent(currentSubmitter, EventCode.C51109_VARIATION_ACCEPTED_IN_ERROR_ACCEPTANCE_REVERSED);

        var allReversal = applAcceptedReversalAll;
        allReversal.AddRange(applCancelReversalAll);
        allReversal.AddRange(applRejectionReversalAll);
        allReversal.AddRange(applVariationReversalAll);

        await BuildSearchResult(allReversal);
    }

    public async Task<IActionResult> OnPostSuspendApplication()
    {
        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var interception = await interceptionApi.GetApplicationAsync(SuspendData.Appl_EnfSrv_Cd, SuspendData.Appl_CtrlCd);
        interception = await interceptionApi.SuspendInterceptionApplicationAsync(interception);
        // to do: also store Suspend reason description...

        if (interception.Messages.ContainsMessagesOfType(MessageType.Error))
        {
            SetDisplayMessages(interception.Messages);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelApplication()
    {
        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var interception = await interceptionApi.GetApplicationAsync(CancelData.Appl_EnfSrv_Cd, CancelData.Appl_CtrlCd);
        interception = await interceptionApi.CancelInterceptionApplicationAsync(interception);
        // to do: also store Cancel reason description...

        if (interception.Messages.ContainsMessagesOfType(MessageType.Error))
        {
            SetDisplayMessages(interception.Messages);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTransferApplication()
    {
        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var interception = await interceptionApi.GetApplicationAsync(TransferData.Appl_EnfSrv_Cd, TransferData.Appl_CtrlCd);

        if (string.IsNullOrEmpty(TransferData.NewIssuingSubmitter))
            TransferData.NewIssuingSubmitter = TransferData.NewRecipientSubmitter;

        interception = await interceptionApi.TransferInterceptionApplicationAsync(interception, TransferData.NewRecipientSubmitter,
                                                                                  TransferData.NewIssuingSubmitter);

        if (interception.Messages.ContainsMessagesOfType(MessageType.Error))
        {
            SetDisplayMessages(interception.Messages);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAdvancedSearch()
    {
        if (!ModelState.IsValid)
        {
            SearchResults = await GetSearchResults(SearchCriteria);

            if (BaseAPIs.ErrorData.Any())
                ErrorMessage = BaseAPIs.ErrorData;

            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMySearch()
    {
        if (!ModelState.IsValid)
        {
            SearchResults = await GetSearchResults(MySearchCriteria);

            if (BaseAPIs.ErrorData.Any())
                ErrorMessage = BaseAPIs.ErrorData;

            return Page();
        }

        return RedirectToPage();
    }

    private async Task<List<ApplicationSearchResultData>> GetSearchResults(ApplicationSearchCriteriaData searchCriteria)
    {
        string enfService = HttpContext.Session.GetString(SessionValue.ENF_SERVICE);

        var quickSearch = new QuickSearchData
        {
            DebtorFirstName = searchCriteria.FirstName,
            DebtorSurname = searchCriteria.LastName,
            SIN = searchCriteria.SIN,
            State = searchCriteria.State,
            Status = searchCriteria.Status,
            Category = searchCriteria.Category,
            ControlCode = searchCriteria.ControlCode,
            EnforcementService = enfService,
            ReferenceNumber = searchCriteria.EnfSourceRefNumber,
            JusticeNumber = searchCriteria.JusticeNumber
        };

        var searchApi = new ApplicationSearchesAPIBroker(BaseAPIs);
        return await searchApi.SearchAsync(quickSearch);
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetCreatedToday()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterToday = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 0);
        return changesForSubmitterToday.Where(m => m.Appl_Create_Usr == currentSubmitter &&
                                                   m.Appl_Create_Dte.Date == DateTime.Now.Date).ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetSinPendingsLast7days()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterLast7days = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 7);
        return changesForSubmitterLast7days.Where(m => m.AppLiSt_Cd == ApplicationState.SIN_CONFIRMATION_PENDING_3).ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetSinPendingsAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.SIN_CONFIRMATION_PENDING_3);
        return changesForSubmitterAll.ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetApprovedAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.APPLICATION_ACCEPTED_10);
        return changesForSubmitterAll.ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetCancelledAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.MANUALLY_TERMINATED_14);
        return changesForSubmitterAll.ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetRejectedAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesForSubmitterAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.APPLICATION_REJECTED_9);
        return changesForSubmitterAll.ToList();
    }

    private async Task<List<ApplicationModificationActivitySummaryData>> GetSubmittedVariationsAll()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);
        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);

        var changesSubmittedVariationsAll = await apiSubmitterBroker.GetAllAtState(currentSubmitter, ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19);
        return changesSubmittedVariationsAll.ToList();
    }

    private async Task BuildSearchResult(List<ApplicationModificationActivitySummaryData> modifiedFiles)
    {
        var apiApplication = new ApplicationAPIBroker(BaseAPIs);
        var apiInterception = new InterceptionApplicationAPIBroker(InterceptionAPIs);

        var activeStatusesBroker = new ActiveStatusesAPIBroker(BaseAPIs);
        var activeStatuses = activeStatusesBroker.GetActiveStatusesAsync().Result;

        SearchResults = new List<ApplicationSearchResultData>();
        foreach (var sinPendingAppl in modifiedFiles)
        {
            var thisAppl = await apiApplication.GetApplicationAsync(sinPendingAppl.Appl_EnfSrv_Cd, sinPendingAppl.Appl_CtrlCd);
            var summonsSummary = await apiInterception.GetSummonsSummaryForApplication(sinPendingAppl.Appl_EnfSrv_Cd, sinPendingAppl.Appl_CtrlCd);

            SearchResults.Add(new ApplicationSearchResultData
            {
                AppCtgy_Cd = thisAppl.AppCtgy_Cd,
                Appl_EnfSrv_Cd = thisAppl.Appl_EnfSrv_Cd,
                Subm_SubmCd = thisAppl.Subm_SubmCd,
                Appl_CtrlCd = thisAppl.Appl_CtrlCd,
                Subm_Recpt_SubmCd = thisAppl.Subm_Recpt_SubmCd,
                Appl_Rcptfrm_Dte = thisAppl.Appl_Rcptfrm_Dte,
                Appl_Expiry_Dte = summonsSummary?.ActualEnd_Dte,
                Appl_Dbtr_SurNme = thisAppl.Appl_Dbtr_SurNme,
                Appl_Source_RfrNr = thisAppl.Appl_Source_RfrNr,
                Appl_JusticeNr = thisAppl.Appl_JusticeNr,
                ActvSt_Cd = activeStatuses.Where(m => m.ActvSt_Cd == thisAppl.ActvSt_Cd).FirstOrDefault()?.ActvSt_Txt_E,
                AppLiSt_Cd = thisAppl.AppLiSt_Cd
            });
        }
    }

}

