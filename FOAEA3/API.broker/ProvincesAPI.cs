using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class ProvincesAPI : BaseAPI
    {
        public List<ProvinceData> GetProvinces()
        {
            return GetDataAsync<List<ProvinceData>>($"api/v1/provinces").Result;
        }
    }
}
