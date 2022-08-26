using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<List<ApplicationEventData>> GetRequestedSINEventDataForFileAsync(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventsForFile?fileName={fileName}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventData>>(apiCall);
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventDetailsForFile?fileName={fileName}";
            return await ApiHelper.GetDataAsync<List<ApplicationEventDetailData>>(apiCall);
        }

        public async Task<List<SinInboundToApplData>> GetLatestSinEventDataSummaryAsync()
        {
            string apiCall = $"api/v1/applicationEvents/GetLatestSinEventDataSummary";
            return await ApiHelper.GetDataAsync<List<SinInboundToApplData>>(apiCall);
        }

        public async Task SaveEventAsync(ApplicationEventData eventData)
        {
            string apiCall = $"api/v1/applicationEvents";
            _ = await ApiHelper.PostDataAsync<ApplicationEventData, ApplicationEventData>(apiCall, eventData);
        }

        public async Task SaveEventDetailAsync(ApplicationEventDetailData eventDetail)
        {
            string apiCall = $"api/v1/applicationEventDetails";
            _ = await ApiHelper.PostDataAsync<ApplicationEventDetailData, ApplicationEventDetailData>(apiCall, eventDetail);
        }

        public async Task UpdateOutboundEventDetailAsync(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            string writtenFile = HttpUtility.UrlEncode(newFilePath);

            string apiCall = $"api/v1/applicationEventDetails?command=MarkOutboundProcessed&activeState={actvSt_Cd}" +
                             $"&applicationState={appLiSt_Cd}&enfSrvCode={enfSrv_Cd}&writtenFile={writtenFile}";
            _ = await ApiHelper.PutDataAsync<ApplicationEventDetailData, List<int>>(apiCall, eventIds);
        }

    }
}
