using FOAEA3.Model.Interfaces;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IFEDInterceptionAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


    }
}
