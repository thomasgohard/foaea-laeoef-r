using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;

namespace FOAEA3.Business.Utilities
{
    internal class ProductionAuditManager
    {
        private IRepositories Repositories { get; }

        public ProductionAuditManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        public void Insert(string processName, string description, string audience, DateTime? completedDate = null)
        {
            Repositories.ProductionAuditRepository.Insert(processName, description, audience, completedDate);
        }

        public void Insert(ProductionAuditData productionAuditData)
        {
            Repositories.ProductionAuditRepository.Insert(productionAuditData);
        }
    }
}
