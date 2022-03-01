using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class ActiveStatusesAPI : BaseAPI
    {
        internal Dictionary<string, ActiveStatusData> GetActiveStatuses()
        {
            var result = new Dictionary<string, ActiveStatusData>();

            var data = GetDataAsync< DataList<ActiveStatusData>>($"api/v1/activeStatuses").Result;

            if (data.Messages.Count > 0)
                ReferenceData.Instance().Messages.AddRange(data.Messages);

            foreach (var activeStatus in data.Items)
                result.Add(activeStatus.ActvSt_Cd, activeStatus);

            return result;
        }
    }
}
