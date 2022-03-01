using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;

namespace FOAEA3.Business.Utilities
{
    internal class EnforcementServiceManager
    {
        private IRepositories Repositories { get; }

        public EnforcementServiceManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        internal EnfSrvData GetEnforcementService(string enfSrvCd)
        {
            IEnfSrvRepository db = Repositories.EnfSrvRepository;
            List<EnfSrvData> result = db.GetEnfService(enfSrvCd);
            if (result.Count == 1)
                return result[0];
            else
                return null;
        }

    }
}
