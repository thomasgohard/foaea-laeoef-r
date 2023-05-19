using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IMEPInterceptionAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<string> GetLatestProvincialFileAsync(string partnerId);
    }
}
