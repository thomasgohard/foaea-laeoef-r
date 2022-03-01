using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IEnfSrvRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<EnfSrvData> GetEnfService(string enforcementServiceCode = null, string enforcementServiceName = null, string enforcementServiceProvince = null, string enforcementServiceCategory = null);
    }
}