using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Business.Utilities
{
    internal class ProductionAuditManager
    {
        private IRepositories Repositories { get; }

        public ProductionAuditManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        public async Task InsertAsync(string processName, string description, string audience, DateTime? completedDate = null)
        {
            await Repositories.ProductionAuditRepository.InsertAsync(processName, description, audience, completedDate);
        }

        public async Task InsertAsync(ProductionAuditData productionAuditData)
        {
            await Repositories.ProductionAuditRepository.InsertAsync(productionAuditData);
        }
    }
}
