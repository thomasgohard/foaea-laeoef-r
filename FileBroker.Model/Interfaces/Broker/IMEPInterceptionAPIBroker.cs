using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IMEPInterceptionAPIBroker
    {
        Task<string> GetLatestProvincialFileAsync(string partnerId);
    }
}
