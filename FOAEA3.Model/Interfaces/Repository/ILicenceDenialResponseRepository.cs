using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILicenceDenialResponseRepository
    {
        //DataList<LicenceDenialResponseData> GetLicenceDenialResponseForApplication(string applEnfSrvCd, string applCtrlCd);
        LicenceDenialResponseData GetLastResponseData(string applEnfSrvCd, string applCtrlCd);
    }
}
