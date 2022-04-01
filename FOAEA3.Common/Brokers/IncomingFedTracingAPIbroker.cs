using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Net.Http;

namespace FOAEA3.Common.Brokers
{
    public class IncomingFedTracingAPIbroker
    {
        private IAPIBrokerHelper ApiHelper { get; }
        private ApiConfig ApiFilesConfig { get; }

        public IncomingFedTracingAPIbroker(IAPIBrokerHelper apiHelper, ApiConfig apiConfig)
        {
            ApiHelper = apiHelper;
            ApiFilesConfig = apiConfig;
        }

        public HttpResponseMessage ProcessFlatFile(string fileNameNoPath, string flatFile)
        {
            string apiCall = $"api/v1/FederalTracingFiles?fileName={fileNameNoPath}";
            return ApiHelper.PostFlatFile(apiCall, flatFile, rootAPI: ApiFilesConfig.FileBrokerFederalTracingRootAPI);
        }
    }
}
