using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ILicenceDenialApplicationAPIBroker
    {
        LicenceDenialApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        List<LicenceDenialOutgoingFederalData> GetOutgoingFederalLicenceDenialRequests(int maxRecords,
                                                                                  string activeState,
                                                                                  int lifeState,
                                                                                  string enfServiceCode);

        List<LicenceDenialToApplData> GetLicenceDenialToApplData(string fedSource);

        LicenceDenialApplicationData ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd);

    }
}
