using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISubmitterProfileAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<SubmitterProfileData> GetSubmitterProfileAsync(string submitterCode);
    }
}
