using FOAEA3.API.broker;
using FOAEA3.Model;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Helpers
{
    internal static class SubmitterHelper
    {

        internal static IEnumerable<(string prvCd, string prvTxt)> GetProvinces(bool isEnglish)
        {
            var provincesAPI = new ProvincesAPI();

            List<ProvinceData> provinces = provincesAPI.GetProvinces().FindAll(m => m.PrvCtryCd == "CAN");

            var data = from province in provinces
                       select (province.PrvCd, isEnglish ? province.PrvTxtE : province.PrvTxtF);

            return data;
        }

        internal static IEnumerable<(string enfSrvCd, string cityLoc, string cityName, string offName, string activeCode)> GetEnfOffForProvince(string provCd)
        {

            var enfOfficesAPI = new EnforcementOfficesAPI();
            List<EnfOffData> enfOffs = enfOfficesAPI.GetEnfOff(province: provCd);

            var orderedEnfOffs = from enfOff in enfOffs
                                 orderby enfOff.EnfSrv_Cd, enfOff.EnfOff_City_LocCd
                                 select (enfOff.EnfSrv_Cd,
                                         enfOff.EnfOff_City_LocCd,
                                         enfOff.EnfOff_Addr_CityNme,
                                         enfOff.EnfOff_Nme,
                                         enfOff.ActvSt_Cd
                                 );

            return orderedEnfOffs;
        }

        internal static IEnumerable<(string enfSrv_Cd, string enfOff_City_LocCd)> GetEnfOffForService(string enfServiceCd)
        {
            var enfOfficesAPI = new EnforcementOfficesAPI();
            List<EnfOffData> enfOffs = enfOfficesAPI.GetEnfOff(enfServCode: enfServiceCd);

            var orderedEnfOffs = from enfOff in enfOffs
                                 where enfOff.ActvSt_Cd == "A"
                                 orderby enfOff.EnfSrv_Cd, enfOff.EnfOff_City_LocCd
                                 select (enfOff.EnfSrv_Cd, enfOff.EnfOff_City_LocCd);

            return orderedEnfOffs;
        }

        internal static (string enfOff_AbbrCd, string enfOff_Addr_CityNme) GetEnfOffInfo(string enfOffCd, string enfSrvCd)
        {

            var enfOfficesAPI = new EnforcementOfficesAPI();
            EnfOffData enfOff = enfOfficesAPI.GetEnfOff(enfOffCode: enfOffCd, enfServCode: enfSrvCd).FirstOrDefault();

            return (enfOff.EnfOff_AbbrCd, enfOff.EnfOff_Addr_CityNme);
        }

        internal static IEnumerable<(string enfSrv_Cd, string enfSrv_Nme)> GetEnfServicesForProvince(string provCd, bool isEnglish)
        {

            var enfServicesAPI = new EnforcementsServiceAPI();
            List<EnfSrvData> enfServices = enfServicesAPI.GetEnforcementServices(enforcementServiceProvince: provCd);
            var orderedEnfServices = from enfService in enfServices
                                     where enfService.ActvSt_Cd == "A"
                                     orderby enfService.EnfSrv_Cd
                                     select (enfService.EnfSrv_Cd, isEnglish ? enfService.EnfSrv_Nme : enfService.EnfSrv_Nme_F);

            return orderedEnfServices;
        }

        internal static IEnumerable<(string subjectName, bool allowedAccess)> GetSubjectsForSubmitter(string submCd)
        {
            var subjectsAPI = new SubjectsAPI();

            List<SubjectData> subjects = subjectsAPI.GetSubjectsForSubmitter(submCd);

            var data = from s in subjects
                       select (s.SubjectName, s.AllowedAccess);

            return data;

        }

    }

}
