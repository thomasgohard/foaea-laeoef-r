using FOAEA3.Model;
using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class ApplicationsAPI : BaseAPI
    {
        public ApplicationData GetApplication(string enfSrv, string ctrlCd)
        {
            var data = GetDataAsync<ApplicationData>($"api/v1/applications/{enfSrv}-{ctrlCd}").Result;
            if (data.Messages is null)
                data.Messages = new MessageDataList();
            return data;
        }

        public List<ApplicationEventData> GetEvents(string key)
        {
            return GetDataAsync<List<ApplicationEventData>>($"api/v1/applicationEvents/{key}").Result;
        }

        public object GetSINResults(string key)
        {
            return GetDataAsync< DataList<SINResultData>>($"api/v1/applications/{key}/SINresults").Result;
        }

        public object GetSINResultsWithHistory(string key)
        {
            return GetDataAsync< DataList<SINResultWithHistoryData>>($"api/v1/applications/{key}/SINresultsWithHistory").Result;
        }

    }
}
