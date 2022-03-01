using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class ApplicationLifeStatesAPI : BaseAPI
    {
        internal Dictionary<ApplicationState, ApplicationLifeStateData> GetApplicationLifeStates()
        {
            var result = new Dictionary<ApplicationState, ApplicationLifeStateData>();

            var data = GetDataAsync< DataList<ApplicationLifeStateData>>($"api/v1/applicationLifeStates").Result;

            if (data.Messages.Count > 0)
                ReferenceData.Instance().Messages.AddRange(data.Messages);

            foreach (var applicationLifeState in data.Items)
                result.Add(applicationLifeState.AppLiSt_Cd, applicationLifeState);

            return result;
        }
    }
}
