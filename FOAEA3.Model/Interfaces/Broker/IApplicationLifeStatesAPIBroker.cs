using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationLifeStatesAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStatesAsync();
    }
}
