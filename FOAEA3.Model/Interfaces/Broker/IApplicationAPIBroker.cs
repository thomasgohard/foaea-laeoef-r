namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationAPIBroker
    {
        ApplicationData GetApplication(string appl_EnfSrvCd, string appl_CtrlCd);
    }
}
