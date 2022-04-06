using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class SinAPIBroker : ISinAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public SinAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public void InsertBulkData(List<SINResultData> resultData)
        {
            _ = ApiHelper.PostDataAsync<SINResultData, List<SINResultData>>("api/v1/applicationSins/bulk",
                                                                             resultData).Result;
        }

        public List<SINOutgoingFederalData> GetOutgoingFederalSins(int maxRecords, string activeState,
                                                                   int lifeState, string enfServiceCode)
        {
            string baseCall = "api/v1/OutgoingFederalSins";
            string apiCall = $"{baseCall}?maxRecords={maxRecords}&activeState={activeState}" +
                             $"&lifeState={lifeState}&enfServiceCode={enfServiceCode}";
            return ApiHelper.GetDataAsync<List<SINOutgoingFederalData>>(apiCall).Result;
        }

        public ApplicationData SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            string key = ApplKey.MakeKey(appl_EnfSrvCd, appl_CtrlCd);
            string baseCall = "api/v1/ApplicationSins";
            string apiCall = $"{baseCall}/{key}/SinConfirmation";
            return ApiHelper.PutDataAsync<ApplicationData, SINConfirmationData>(apiCall, confirmationData).Result;
        }
    }
}
