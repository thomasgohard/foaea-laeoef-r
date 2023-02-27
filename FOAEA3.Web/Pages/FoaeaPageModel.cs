using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages;

public class FoaeaPageModel : PageModel
{
    public string CurrentSubmitter;
    protected readonly ApiConfig ApiRoots;
    protected readonly APIBrokerHelper BaseAPIs;
    protected readonly APIBrokerHelper InterceptionAPIs;
    private readonly IHttpContextAccessor ContextAccessor;

    public List<GenderData> Genders { get; set; }
    public List<CountryData> Countries { get; set; }
    public FoaEventDataDictionary EventCodes { get; set; }
    public List<ApplicationLifeStateData> LifeStates { get; set; }
    public List<ProvinceData> ValidProvinces { get; set; }
    public List<PaymentPeriodData> PaymentPeriods { get; set; }
    public List<ProvinceData> AllProvinces { get; set; }

    public FoaEventDataDictionary FoaEvents { get; set; }

    public List<MessageData> ErrorMessage { get; set; } = new List<MessageData>();
    public List<MessageData> WarningMessage { get; set; } = new List<MessageData>();
    public List<MessageData> InfoMessage { get; set; } = new List<MessageData>();

    public FoaeaPageModel(IHttpContextAccessor httpContextAccessor, ApiConfig apiConfig)
    {
        ApiRoots = apiConfig;

        if ((httpContextAccessor is not null) && (httpContextAccessor.HttpContext is not null))
        {
            CurrentSubmitter = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER);
            string userName = httpContextAccessor.HttpContext.Session.GetString(SessionValue.USER_NAME);

            ContextAccessor = httpContextAccessor;

            BaseAPIs = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, CurrentSubmitter, userName,
                                       getToken: GetToken, getRefreshedToken: GetRefreshedToken);
            InterceptionAPIs = new APIBrokerHelper(ApiRoots.FoaeaInterceptionRootAPI, CurrentSubmitter, userName,
                                       getToken: GetToken, getRefreshedToken: GetRefreshedToken);

            if (!string.IsNullOrEmpty(CurrentSubmitter))
            {
                var apiFoaEventsBroker = new FoaEventsAPIBroker(BaseAPIs);
                FoaEvents = apiFoaEventsBroker.GetFoaEventsAsync().Result;
            }
        }
    }

    public string GetToken()
    {
        if ((ContextAccessor is not null) && (ContextAccessor.HttpContext is not null))
        {
            if (ContextAccessor.HttpContext.Session.Keys.Contains(SessionValue.TOKEN))
                return ContextAccessor.HttpContext.Session.GetString(SessionValue.TOKEN);
            else
                return null;
        }
        return null;
    }

    public async Task<string> GetRefreshedToken()
    {
        if ((ContextAccessor is not null) && (ContextAccessor.HttpContext is not null))
        {
            string currentToken = ContextAccessor.HttpContext.Session.GetString(SessionValue.TOKEN);
            string refreshToken = ContextAccessor.HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            string userName = TokenDataHelper.UserName(currentToken);
            string submitter = TokenDataHelper.SubmitterCode(currentToken);

            var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, submitter, userName);
            var apiBroker = new LoginsAPIBroker(apiHelper, currentToken);

            var result = await apiBroker.RefreshTokenAsync(currentToken, refreshToken);

            ContextAccessor.HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
            ContextAccessor.HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

            return result.Token;
        }
        else
            return "";
    }

    public void SetDisplayMessages(MessageDataList messages)
    {
        if (messages.ContainsMessagesOfType(MessageType.Error))
            ErrorMessage = messages.GetMessagesForType(MessageType.Error);

        if (messages.ContainsMessagesOfType(MessageType.Warning))
            WarningMessage = messages.GetMessagesForType(MessageType.Warning);

        if (messages.ContainsMessagesOfType(MessageType.Information))
            InfoMessage = messages.GetMessagesForType(MessageType.Information);
    }

    public string GetEventDescription(MessageData info)
    {
        string result = info.Description;

        if (info.Code != EventCode.UNDEFINED)
        {
            var foaEvent = FoaEvents[info.Code];
            if (foaEvent is not null)
            {
                string description = foaEvent.Description;
                if (!string.IsNullOrEmpty(info.Description))
                    description += $" ({info.Description})";

                result = description;
            }
        }

        if (!string.IsNullOrEmpty(info.Field))
            result += $" [{info.Field}]";

        return result;
    }

    protected void LoadReferenceData()
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

        var apiReasonBroker = new FoaEventsAPIBroker(BaseAPIs);
        EventCodes = apiReasonBroker.GetFoaEventsAsync().Result;
        if (apiReasonBroker.ApiHelper.ErrorData.Any()) ErrorMessage.AddRange(apiReasonBroker.ApiHelper.ErrorData);
    }
}
