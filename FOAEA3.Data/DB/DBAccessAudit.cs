using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBAccessAudit : DBbase, IAccessAuditRepository
    {
        public DBAccessAudit(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<int> SaveDataPageInfo(AccessAuditPage auditPage, string subject_submitter)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Accesstype", (int) auditPage },
                {"UserID", subject_submitter }
            };

            decimal returnValue = await MainDB.GetDataFromProcSingleValueAsync<decimal>("AccessAuditDataHeader_Insert", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                await LogError(MainDB.LastError);

            return (int)returnValue;
        }

        public async Task SaveDataValue(int pageId, string key, string value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ParentID", pageId },
                {"ElementName", key },
                {"ElementValue", value }
            };

            await MainDB.ExecProcAsync("AccessAuditDataElementValue_Insert", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                await LogError(MainDB.LastError);
        }

        public async Task<List<AccessAuditElementTypeData>> GetAllElementAccessType()
        {
            return await MainDB.GetAllDataAsync<AccessAuditElementTypeData>("AccessAuditDataElementValueType", FillAccessAuditElementTypeData);
        }

        private async Task LogError(string errorMessage)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ErrorString", errorMessage }
            };

            await MainDB.ExecProcAsync("AccessAuditDataErrorLog_insert", parameters);
        }

        private void FillAccessAuditElementTypeData(IDBHelperReader rdr, AccessAuditElementTypeData data)
        {
            data.AccessAuditDataElementValueType_ID = (int)rdr["AccessAuditDataElementValueType_ID"];
            data.ElementName = rdr["ElementName"] as string; // can be null 
            data.F_Desc = rdr["F_Desc"] as string; // can be null 
            data.E_Desc = rdr["E_Desc"] as string; // can be null
        }

    }
}
