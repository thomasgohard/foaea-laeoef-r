using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace TestData.TestDB
{
    public class InMemoryEnfSrv : IEnfSrvRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public List<EnfSrvData> GetEnfService(string enforcementServiceCode = null, string enforcementServiceName = null, string enforcementServiceProvince = null, string enforcementServiceCategory = null)
        {
            throw new NotImplementedException();
        }
    }
}
