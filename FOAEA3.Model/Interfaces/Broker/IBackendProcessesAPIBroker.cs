namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IBackendProcessesAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

    }
}
