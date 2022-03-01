using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class FamilyProvisionsAPI : BaseAPI
    {
        public List<FamilyProvisionData> GetFamilyProvisions()
        {
            return GetDataAsync<List<FamilyProvisionData>>($"api/v1/familyProvisions").Result;
        }
    }
}
