using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISubmitterAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<SubmitterData> GetSubmitterAsync(string submitterCode);
    }
}
