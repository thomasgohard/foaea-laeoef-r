using FOAEA3.Model;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Common.Brokers
{
    public class IncomingFedTracingAPIbroker
    {
        public IAPIBrokerHelper ApiHelper { get; }
        private ApiConfig ApiFilesConfig { get; }
        public string Token { get; set; }

        public IncomingFedTracingAPIbroker(IAPIBrokerHelper apiHelper, ApiConfig apiConfig, string token)
        {
            ApiHelper = apiHelper;
            ApiFilesConfig = apiConfig;
            Token = token;
        }

        public async Task<HttpResponseMessage> ProcessFlatFileAsync(string fileNameNoPath, string flatFile)
        {
            string apiCall = $"api/v1/FederalTracingFiles?fileName={fileNameNoPath}";
            return await ApiHelper.PostFlatFileAsync(apiCall, flatFile,
                                                     rootAPI: ApiFilesConfig.FileBrokerFederalTracingRootAPI,
                                                     token: Token);
        }
    }
}
