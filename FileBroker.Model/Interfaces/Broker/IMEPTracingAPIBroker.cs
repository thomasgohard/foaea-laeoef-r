using FOAEA3.Model.Interfaces;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IMEPTracingAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


    }
}
