using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Web;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationEventAPIBroker : IApplicationEventAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationEventAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public void SaveEvent(ApplicationEventData activeTraceEvent)
        {
            string apiCall = $"api/v1/applicationEvents";
            _ = ApiHelper.PostDataAsync<ApplicationEventData, ApplicationEventData>(apiCall, activeTraceEvent).Result;
        }

        public void SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail)
        {
            string apiCall = $"api/v1/applicationEvents/Detail";
            _ = ApiHelper.PostDataAsync<ApplicationEventDetailData, ApplicationEventDetailData>(apiCall, activeTraceEventDetail).Result;
        }

        public void UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            string writtenFile = HttpUtility.UrlEncode(newFilePath);

            string apiCall = $"api/v1/applicationEvent/Detail?command=MarkOutboundProcessed&activeState={actvSt_Cd}" +
                             $"&applicationState={appLiSt_Cd}&enfSrvCode={enfSrv_Cd}&writtenFile={writtenFile}";
            _ = ApiHelper.PutDataAsync<ApplicationEventDetailData, List<int>>(apiCall, eventIds).Result;
        }
    }
}
