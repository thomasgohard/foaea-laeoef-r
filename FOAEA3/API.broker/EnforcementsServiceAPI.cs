using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class EnforcementsServiceAPI : BaseAPI
    {
        internal EnfSrvData GetEnforcementService(string enfServiceCode)
        {
            return GetDataAsync<EnfSrvData>($"api/v1/enfServices/{enfServiceCode}").Result;
        }
        internal List<EnfSrvData> GetEnforcementServices(string enforcementServiceCode = null, string enforcementServiceName = null,
                                  string enforcementServiceProvince = null, string enforcementServiceCategory = null)
        {
            return GetDataAsync<List<EnfSrvData>>($"api/v1/enfServices?enforcementServiceCode={enforcementServiceCode}&enforcementServiceName={enforcementServiceName}&"+
                                                  $"enforcementServiceProvince={enforcementServiceProvince}&enforcementServiceCategory={enforcementServiceCategory}").Result;
        }
    }
}
