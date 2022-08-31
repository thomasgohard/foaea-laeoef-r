using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IProductionAuditRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task InsertAsync(string processName, string description, string audience, DateTime? completedDate = null);
        Task InsertAsync(ProductionAuditData productionAuditData);
    }
}
