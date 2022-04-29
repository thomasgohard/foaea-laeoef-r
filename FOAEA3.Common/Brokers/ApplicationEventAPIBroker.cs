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

        public List<ApplicationEventData> GetRequestedSINEventDataForFile(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventsForFile?fileName={fileName}";
            return ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall).Result;
        }

        public List<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventDetailsForFile?fileName={fileName}";
            return ApiHelper.GetDataAsync<List<ApplicationEventDetailData>>(apiCall).Result;
        }

        public List<SinInboundToApplData> GetLatestSinEventDataSummary()
        {
            string apiCall = $"api/v1/applicationEvents/GetLatestSinEventDataSummary";
            return ApiHelper.GetDataAsync<List<SinInboundToApplData>>(apiCall).Result;
        }

        public void SaveEvent(ApplicationEventData eventData)
        {
            string apiCall = $"api/v1/applicationEvents";
            _ = ApiHelper.PostDataAsync<ApplicationEventData, ApplicationEventData>(apiCall, eventData).Result;
        }

        public void SaveEventDetail(ApplicationEventDetailData eventDetail)
        {
            string apiCall = $"api/v1/applicationEventDetails";
            _ = ApiHelper.PostDataAsync<ApplicationEventDetailData, ApplicationEventDetailData>(apiCall, eventDetail).Result;
        }

        public void UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            string writtenFile = HttpUtility.UrlEncode(newFilePath);

            string apiCall = $"api/v1/applicationEventDetails?command=MarkOutboundProcessed&activeState={actvSt_Cd}" +
                             $"&applicationState={appLiSt_Cd}&enfSrvCode={enfSrv_Cd}&writtenFile={writtenFile}";
            _ = ApiHelper.PutDataAsync<ApplicationEventDetailData, List<int>>(apiCall, eventIds).Result;
        }

    }
}
