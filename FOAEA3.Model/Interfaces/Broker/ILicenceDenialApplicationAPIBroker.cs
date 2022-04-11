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
    }
}
