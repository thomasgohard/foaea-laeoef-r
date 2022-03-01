using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Data.DB
{
    public class DBFailedSubmitAudit : DBbase, IFailedSubmitAuditRepository
    {
        public DBFailedSubmitAudit(IDBTools mainDB) : base(mainDB)
        {

        }

        public void AppendFiledSubmitAudit(string subject_submitter, FailedSubmitActivityAreaType activityAreaType, string error)
        {
            var parameters = new Dictionary<string, object>
            {
                {"UserID", subject_submitter },
                {"ActivityAeraTypeCd", activityAreaType.ToString() },
                {"ErrorString", error }
            };

            MainDB.ExecProc("FailedSubmitDataAudit_Insert", parameters);
        }

    }
}
