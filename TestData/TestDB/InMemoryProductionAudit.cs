using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryProductionAudit : IProductionAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task Insert(string processName, string description, string audience, DateTime? completedDate = null)
        {
            InMemData.ProductionAuditData.Add($"{processName}({DateTime.Now}): {description}");

            return Task.CompletedTask;
        }

        public Task Insert(ProductionAuditData productionAuditData)
        {
            throw new NotImplementedException();
        }
    }
}
