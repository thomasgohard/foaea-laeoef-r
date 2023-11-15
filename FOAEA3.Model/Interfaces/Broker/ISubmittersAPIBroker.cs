using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISubmittersAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<string> GetFOAEAOfficersEmails();
    }
}
