using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages.Applications;

public class InterceptionModel : FoaeaPageModel
{
    public string EnfServiceDescription { get; set; }
    public List<GenderData> Genders { get; set; }
    public List<CountryData> Countries { get; set; }
    public List<ApplicationLifeStateData> LifeStates { get; set; }
    public List<ProvinceData> ValidProvinces { get; set; }

    public List<ProvinceData> AllProvinces { get; set; }

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

            var apiLifeStatesBroker = new ApplicationLifeStatesAPIBroker(BaseAPIs);
            LifeStates = apiLifeStatesBroker.GetApplicationLifeStatesAsync().Result;
            if (apiLifeStatesBroker.ApiHelper.ErrorData.Any())
                ErrorMessage.AddRange(apiLifeStatesBroker.ApiHelper.ErrorData);

            var apiGenderBroker = new GendersAPIBroker(BaseAPIs);
            Genders = apiGenderBroker.GetGendersAsync().Result;
            if (apiGenderBroker.ApiHelper.ErrorData.Any())
                ErrorMessage.AddRange(apiGenderBroker.ApiHelper.ErrorData);

            var apiCountryBroker = new CountriesAPIBroker(BaseAPIs);
            Countries = apiCountryBroker.GetCountriesAsync().Result;
            if (apiCountryBroker.ApiHelper.ErrorData.Any())
                ErrorMessage.AddRange(apiCountryBroker.ApiHelper.ErrorData);

            var apiProvinceBroker = new ProvincesAPIBroker(BaseAPIs);
            AllProvinces = apiProvinceBroker.GetProvincesAsync().Result;
            if (apiProvinceBroker.ApiHelper.ErrorData.Any())
                ErrorMessage.AddRange(apiProvinceBroker.ApiHelper.ErrorData);
        }
    }

    public void OnGet()
    {

    }

    public JsonResult OnGetSelectProvinceForCountry(string countryCode)
    {
        ValidProvinces = AllProvinces.Where(m => m.PrvCtryCd == countryCode).ToList();

        return new JsonResult(ValidProvinces);
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

        SetDisplayMessages(newApplication.Messages);

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
