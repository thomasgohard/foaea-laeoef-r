using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBFailedSubmitAudit : DBbase, IFailedSubmitAuditRepository
    {
        public DBFailedSubmitAudit(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task AppendFiledSubmitAuditAsync(string subject_submitter, FailedSubmitActivityAreaType activityAreaType, string error)
        {
            var parameters = new Dictionary<string, object>
            {
                {"UserID", subject_submitter },
                {"ActivityAeraTypeCd", activityAreaType.ToString() },
                {"ErrorString", error }
            };

            await MainDB.ExecProcAsync("FailedSubmitDataAudit_Insert", parameters);
        }

    }
}
