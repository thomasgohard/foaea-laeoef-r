using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialResponseAPIBroker : ILicenceDenialResponseAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialResponseAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }
        
        public void InsertBulkData(List<LicenceDenialResponseData> responseData)
        {
            _ = ApiHelper.PostDataAsync<LicenceDenialResponseData, List<LicenceDenialResponseData>>("api/v1/licenceDenialResponses/bulk",
                                                                                                    responseData).Result;
        }

        public void MarkTraceResultsAsViewed(string enfService)
        {
            ApiHelper.SendCommand("api/v1/licenceDenialResponses/MarkResultsAsViewed?enfService=" + enfService);
        }
    }
}
