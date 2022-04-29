using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Common.Brokers
{
    public class LicenceDenialTerminationApplicationAPIBroker : ILicenceDenialTerminationApplicationAPIBroker
    {
        private IAPIBrokerHelper ApiHelper { get; }

        public LicenceDenialTerminationApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public LicenceDenialApplicationData ProcessLicenceDenialResponse(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var appData = new LicenceDenialApplicationData
            {
                Appl_EnfSrv_Cd = appl_EnfSrv_Cd,
                Appl_CtrlCd = appl_CtrlCd,
                Subm_SubmCd = "MSGBRO"
            };
            string key = ApplKey.MakeKey(appl_EnfSrv_Cd, appl_CtrlCd);
            string apiCall = $"api/v1/licenceDenials/{key}/ProcessLicenceDenialTerminationResponse";
            return ApiHelper.PutDataAsync<LicenceDenialApplicationData, LicenceDenialApplicationData>(apiCall, appData).Result;
        }
    }
}
