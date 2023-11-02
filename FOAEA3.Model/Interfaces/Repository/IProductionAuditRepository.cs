using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IProductionAuditRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task Insert(string processName, string description, string audience, DateTime? completedDate = null);
        Task Insert(ProductionAuditData productionAuditData);
    }
}
