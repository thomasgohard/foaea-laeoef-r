using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System.Web;

namespace FOAEA3.Common.Brokers
{
    public class ApplicationEventAPIBroker : IApplicationEventAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public ApplicationEventAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<List<ApplicationEventData>> GetRequestedSINEventDataForFile(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventsForFile?fileName={fileName}";
            return await ApiHelper.GetData<List<ApplicationEventData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string fileName)
        {
            string apiCall = $"api/v1/applicationFederalSins/RequestedEventDetailsForFile?fileName={fileName}";
            return await ApiHelper.GetData<List<ApplicationEventDetailData>>(apiCall, token: Token);
        }

        public async Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary()
        {
            string apiCall = $"api/v1/applicationEvents/GetLatestSinEventDataSummary";
            return await ApiHelper.GetData<List<SinInboundToApplData>>(apiCall, token: Token);
        }

        public async Task<List<ApplicationEventData>> GetEvents(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string apiCall = $"api/v1/applicationEvents/{key}";
            return await ApiHelper.GetData<List<ApplicationEventData>>(apiCall, token: Token);
        }

        public async Task SaveEvent(ApplicationEventData eventData)
        {
            string apiCall = $"api/v1/applicationEvents";
            _ = await ApiHelper.PostData<ApplicationEventData, ApplicationEventData>(apiCall, eventData, token: Token);
        }

        public async Task SaveEventDetail(ApplicationEventDetailData eventDetail)
        {
            string apiCall = $"api/v1/applicationEventDetails";
            _ = await ApiHelper.PostData<ApplicationEventDetailData, ApplicationEventDetailData>(apiCall, eventDetail, token: Token);
        }

        public async Task UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            string writtenFile = HttpUtility.UrlEncode(newFilePath);

            string apiCall = $"api/v1/applicationEventDetails?command=MarkOutboundProcessed&activeState={actvSt_Cd}" +
                             $"&applicationState={appLiSt_Cd}&enfSrvCode={enfSrv_Cd}&writtenFile={writtenFile}";
            _ = await ApiHelper.PutData<ApplicationEventDetailData, List<int>>(apiCall, eventIds, token: Token);
        }

    }
}
