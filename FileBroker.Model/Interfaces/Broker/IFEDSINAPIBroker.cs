using FOAEA3.Model.Interfaces;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IFEDSINAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


    }
}
