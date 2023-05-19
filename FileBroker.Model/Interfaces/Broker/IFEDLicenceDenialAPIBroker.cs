using FOAEA3.Model.Interfaces;

namespace FileBroker.Model.Interfaces.Broker
{
    public interface IFEDLicenceDenialAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


    }
}
