using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IEnfSrvRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<List<EnfSrvData>> GetEnfServiceAsync(string enforcementServiceCode = null, string enforcementServiceName = null, string enforcementServiceProvince = null, string enforcementServiceCategory = null);
    }
}