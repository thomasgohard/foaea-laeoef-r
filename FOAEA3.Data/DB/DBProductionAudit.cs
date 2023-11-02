using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBProductionAudit : DBbase, IProductionAuditRepository
    {
        public DBProductionAudit(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task Insert(string processName, string description, string audience, DateTime? completedDate = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Process_dte", DateTime.Now },
                {"Process_name", processName },
                {"Description_code", description },
                {"Audience", audience }
            };

            if (completedDate.HasValue)
                parameters.Add("Compl_dte", completedDate.Value);
            else
                parameters.Add("Compl_dte", DBNull.Value);

            _ = await MainDB.ExecProcAsync("ProductionAuditInsert", parameters);
        }

        public async Task Insert(ProductionAuditData productionAuditData)
        {
            await Insert(productionAuditData.Process_name, productionAuditData.Description, productionAuditData.Audience,
                              productionAuditData.Compl_dte);
        }
    }
}
