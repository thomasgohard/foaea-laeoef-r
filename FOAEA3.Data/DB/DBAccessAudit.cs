using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    public class DBAccessAudit : DBbase, IAccessAuditRepository
    {
        public DBAccessAudit(IDBTools mainDB) : base(mainDB)
        {

        }

        public int SaveDataPageInfo(AccessAuditPage auditPage, string subject_submitter)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Accesstype", (int) auditPage },
                {"UserID", subject_submitter }
            };

            decimal returnValue = MainDB.GetDataFromProcSingleValue<decimal>("AccessAuditDataHeader_Insert", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                LogError(MainDB.LastError);

            return (int) returnValue;
        }

        public void SaveDataValue(int pageId, string key, string value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ParentID", pageId },
                {"ElementName", key },
                {"ElementValue", value }
            };

            MainDB.ExecProc("AccessAuditDataElementValue_Insert", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                LogError(MainDB.LastError);
        }

        public List<AccessAuditElementTypeData> GetAllElementAccessType()
        {
            return MainDB.GetAllData<AccessAuditElementTypeData>("AccessAuditDataElementValueType", FillAccessAuditElementTypeData);
        }

        private void LogError(string errorMessage)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ErrorString", errorMessage }
            };

            MainDB.ExecProc("AccessAuditDataErrorLog_insert", parameters);
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
