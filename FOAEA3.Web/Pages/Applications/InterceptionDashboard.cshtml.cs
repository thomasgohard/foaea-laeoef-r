using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FOAEA3.Model.Enums;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionDashboardModel : FoaeaPageModel
{
    public List<ApplicationLifeStateData> LifeStates { get; set; }
    public List<ApplicationSearchResultData> SearchResults { get; set; }
    public int SubmittedToday { get; set; }
    public int SinPendingsLast7days { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData SearchCriteria { get; set; }

    [BindProperty]
    public ApplicationSearchCriteriaData MySearchCriteria { get; set; }

    public async Task OnGet()
    {
        string currentSubmitter = HttpContext.Session.GetString(SessionValue.SUBMITTER);

        var apiSubmitterBroker = new SubmitterAPIBroker(BaseAPIs);
        var changesForSubmitterToday = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 0);
        var changesForSubmitterLast7days = await apiSubmitterBroker.GetRecentActivity(currentSubmitter, days: 7);

        SubmittedToday = changesForSubmitterToday.Where(m => m.Appl_Create_Usr == currentSubmitter &&
                                                             m.Appl_Create_Dte.Date == DateTime.Now.Date).Count();

        SinPendingsLast7days = changesForSubmitterLast7days.Where(m => m.AppLiSt_Cd == ApplicationState.SIN_CONFIRMATION_PENDING_3).Count();
    }

    public async Task OnPostSearchSubmittedToday()
    {
        await BuildSearchResult(await GetCreatedToday());
    }

    public async Task OnPostSearchSinPendingsLast7days()
    {
        await BuildSearchResult(await GetSinPendingsLast7days());
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
       
    public InterceptionDashboardModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        var apiRefBroker = new ApplicationLifeStatesAPIBroker(BaseAPIs);
        LifeStates = apiRefBroker.GetApplicationLifeStatesAsync().Result;

        SearchCriteria = new ApplicationSearchCriteriaData
        {
            Category = "I01" // default
        };
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

        return RedirectToPage("./Index");
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

        return RedirectToPage("./Index");
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

}

