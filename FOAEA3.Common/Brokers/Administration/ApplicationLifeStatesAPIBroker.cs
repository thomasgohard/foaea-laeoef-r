using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Common.Brokers.Administration
{

    public class ApplicationLifeStatesAPIBroker : IApplicationLifeStatesAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationLifeStatesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public DataList<ApplicationLifeStateData> GetApplicationLifeStates()
        {
            string apiCall = $"api/v1/ApplicationLifeStates";
            return ApiHelper.GetDataAsync<DataList<ApplicationLifeStateData>>(apiCall).Result;
        }

    }
}
