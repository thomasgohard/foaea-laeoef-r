using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    public class InMemoryProductionAudit : IProductionAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void Insert(string processName, string description, string audience, DateTime? completedDate = null)
        {
            InMemData.ProductionAuditData.Add($"{processName}({DateTime.Now}): {description}");
        }

        public void Insert(ProductionAuditData productionAuditData)
        {
            throw new NotImplementedException();
        }
    }
}
