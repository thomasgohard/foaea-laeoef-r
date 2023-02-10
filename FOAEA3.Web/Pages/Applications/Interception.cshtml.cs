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
    public string EnfServiceDescription { get; set; }
    public List<GenderData> Genders { get; set; }
    public List<CountryData> Countries { get; set; }
    public List<ApplicationLifeStateData> LifeStates { get; set; }
    public List<ProvinceData> ValidProvinces { get; set; }
    public List<PaymentPeriodData> PaymentPeriods { get; set; }

    public List<ProvinceData> AllProvinces { get; set; }

    [BindProperty]
    public InterceptionApplicationData InterceptionApplication { get; set; }

    [BindProperty]
    public HoldbackConditionData NewCondition { get; set; }

    public InterceptionModel(IHttpContextAccessor httpContextAccessor, IOptions<ApiConfig> apiConfig) :
                                                                                                base(httpContextAccessor, apiConfig.Value)
    {
        if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
        {
            string submitter = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER);

            InterceptionApplication = new InterceptionApplicationData
            {
                Appl_EnfSrv_Cd = httpContextAccessor.HttpContext.Session.GetString(SessionValue.ENF_SERVICE),
                Subm_SubmCd = submitter,
                Appl_Dbtr_Addr_CtryCd = "CAN",
                Subm_Recpt_SubmCd = submitter,
                Medium_Cd = "ONL"                
            };

            var submitterProfileApi = new SubmitterProfileAPIBroker(BaseAPIs);
            EnfServiceDescription = submitterProfileApi.GetSubmitterProfileAsync(submitter).Result.EnfSrv_Nme;

            LoadReferenceData();
        }
    }

    public void OnGet()
    {

    }

    public JsonResult OnGetSelectProvinceForCountry(string countryCode)
    {
        ValidProvinces = AllProvinces.Where(m => m.PrvCtryCd == countryCode).OrderBy(m => m.PrvTxtE).ToList();

        return new JsonResult(ValidProvinces);
    }

    public async Task<IActionResult> OnPostSubmitNewApplication()
    {
        if (InterceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd == 0)
            InterceptionApplication.IntFinH.IntFinH_NextRecalcDate_Cd = null;

        if (InterceptionApplication.IntFinH.HldbCtg_Cd == "0")
            InterceptionApplication.IntFinH.HldbTyp_Cd = null;
        else if (InterceptionApplication.IntFinH.HldbCtg_Cd == "1")
            InterceptionApplication.IntFinH.HldbTyp_Cd = "T";
        else // "2"
            InterceptionApplication.IntFinH.HldbTyp_Cd = "P";

        if (InterceptionApplication.Appl_Dbtr_Brth_Dte == DateTime.MinValue)
            InterceptionApplication.Appl_Dbtr_Brth_Dte = null;

        if (!ModelState.IsValid)
        {
            ErrorMessage = GetValidationErrors();

            return Page();
        }

        var interceptionApi = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        var newApplication = await interceptionApi.CreateInterceptionApplicationAsync(InterceptionApplication);

        SetDisplayMessages(newApplication.Messages);

        InterceptionApplication = newApplication;

        if (!InterceptionApplication.HldbCnd.Any())
            InterceptionApplication.HldbCnd.Add(new HoldbackConditionData());

        return RedirectToPagePermanent("InterceptionDashboard");
    }

    public void OnPostAddEntry()
    {
        InterceptionApplication.HldbCnd.Add(new HoldbackConditionData()
        {
            Appl_EnfSrv_Cd = InterceptionApplication.Appl_EnfSrv_Cd,
            Appl_CtrlCd = InterceptionApplication.Appl_CtrlCd,
            HldbCnd_MxmPerChq_Money = NewCondition.HldbCnd_MxmPerChq_Money,
            HldbCnd_SrcHldbAmn_Money = NewCondition.HldbCnd_SrcHldbAmn_Money,
            HldbCnd_SrcHldbPrcnt = NewCondition.HldbCnd_SrcHldbPrcnt,
            HldbCtg_Cd = NewCondition.HldbCtg_Cd,
            EnfSrv_Cd = NewCondition.EnfSrv_Cd
        });
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

    private void LoadReferenceData()
    {
        var apiLifeStatesBroker = new ApplicationLifeStatesAPIBroker(BaseAPIs);
        LifeStates = apiLifeStatesBroker.GetApplicationLifeStatesAsync().Result;
        if (apiLifeStatesBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiLifeStatesBroker.ApiHelper.ErrorData);

        var apiGenderBroker = new GendersAPIBroker(BaseAPIs);
        Genders = apiGenderBroker.GetGendersAsync().Result;
        if (apiGenderBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiGenderBroker.ApiHelper.ErrorData);

        var apiCountryBroker = new CountriesAPIBroker(BaseAPIs);
        Countries = apiCountryBroker.GetCountriesAsync().Result;
        if (apiCountryBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiCountryBroker.ApiHelper.ErrorData);

        var apiProvinceBroker = new ProvincesAPIBroker(BaseAPIs);
        AllProvinces = apiProvinceBroker.GetProvincesAsync().Result;
        if (apiProvinceBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiProvinceBroker.ApiHelper.ErrorData);

        var apiInterceptioBroker = new InterceptionApplicationAPIBroker(InterceptionAPIs);
        PaymentPeriods = apiInterceptioBroker.GetPaymentPeriods().Result;
        if (apiInterceptioBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiInterceptioBroker.ApiHelper.ErrorData);
    }

}
