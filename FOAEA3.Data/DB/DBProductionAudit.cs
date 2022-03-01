using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBProductionAudit : DBbase, IProductionAuditRepository
    {
        public DBProductionAudit(IDBTools mainDB) : base(mainDB)
        {

        }

        public void Insert(string processName, string description, string audience, DateTime? completedDate = null)
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

            _ = MainDB.ExecProc("ProductionAuditInsert", parameters);
        }
    }
}
