namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialTerminationApplicationAPIBroker
    {
        LicenceDenialApplicationData ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
