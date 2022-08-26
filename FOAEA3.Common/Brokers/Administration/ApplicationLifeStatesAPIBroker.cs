using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers.Administration
{

    public class ApplicationLifeStatesAPIBroker : IApplicationLifeStatesAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public ApplicationLifeStatesAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public async Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStatesAsync()
        {
            string apiCall = $"api/v1/ApplicationLifeStates";
            return await ApiHelper.GetDataAsync<DataList<ApplicationLifeStateData>>(apiCall);
        }

    }
}
