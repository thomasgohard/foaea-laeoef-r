using FOAEA3.Model;
using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class LicenceDenialsAPI : BaseAPI
    {
        public LicenceDenialApplicationData GetApplication(string enfSrv, string ctrlCd)
        {
            var data = GetDataAsync<LicenceDenialApplicationData>($"api/v1/licenceDenials/{enfSrv}-{ctrlCd}", APIroot_LicenceDenial).Result;
            if (data.Messages is null)
                data.Messages = new MessageDataList();
            return data;
        }

        public LicenceDenialApplicationData CreateApplication(LicenceDenialApplicationData licenceDenial)
        {
            var data = PostDataAsync<LicenceDenialApplicationData, LicenceDenialApplicationData>("api/v1/licenceDenials", licenceDenial, APIroot_LicenceDenial).Result;
            if (data.Messages is null)
                data.Messages = new MessageDataList();
            return data;
        }

        public LicenceDenialApplicationData UpdateApplication(LicenceDenialApplicationData licenceDenial)
        {
            var data = PutDataAsync<LicenceDenialApplicationData, LicenceDenialApplicationData>("api/v1/licenceDenials", licenceDenial, APIroot_LicenceDenial).Result;
            if (data.Messages is null)
                data.Messages = new MessageDataList();
            return data;
        }

        public object GetTraceResults(string enfSrv, string ctrlCd)
        {
            return GetDataAsync< DataList<TraceResponseData>>($"api/v1/licenceDenials/{enfSrv}-{ctrlCd}/LicenceDenialResults", APIroot_LicenceDenial).Result;
        }
    }
}
