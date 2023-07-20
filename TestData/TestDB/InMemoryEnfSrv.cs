using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryEnfSrv : IEnfSrvRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<List<EnfSrvData>> GetEnfService(string enforcementServiceCode = null, string enforcementServiceName = null, string enforcementServiceProvince = null, string enforcementServiceCategory = null)
        {
            throw new NotImplementedException();
        }
    }
}
