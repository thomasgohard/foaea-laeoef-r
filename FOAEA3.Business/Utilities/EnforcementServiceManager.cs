using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Business.Utilities
{
    internal class EnforcementServiceManager
    {
        private IRepositories Repositories { get; }

        public EnforcementServiceManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        public async Task<EnfSrvData> GetEnforcementServiceAsync(string enfSrvCd)
        {
            IEnfSrvRepository db = Repositories.EnfSrvRepository;
            var result = await db.GetEnfServiceAsync(enfSrvCd);
            if (result.Count == 1)
                return result[0];
            else
                return null;
        }

    }
}
