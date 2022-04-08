namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialApplicationAPIBroker
    {
        LicenceDenialApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
    }
}
