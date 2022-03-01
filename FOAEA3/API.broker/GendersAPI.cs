using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class GendersAPI : BaseAPI
    {
        internal Dictionary<string, GenderData> GetGenders()
        {
            var result = new Dictionary<string, GenderData>();

            var data = GetDataAsync< DataList<GenderData>>($"api/v1/genders").Result;

            if (data.Messages.Count > 0)
                ReferenceData.Instance().Messages.AddRange(data.Messages);
            
            data.Items.Sort();

            foreach (var gender in data.Items)
                result.Add(gender.Gender_Cd, gender);

            return result;
        }
    }
}
