using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IFoaEventsAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<FoaEventDataDictionary> GetFoaEventsAsync();
    }
}
