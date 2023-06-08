using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSINChangeHistory : DBbase, ISINChangeHistoryRepository
    {
        public DBSINChangeHistory(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<bool> CreateSINChangeHistory(SINChangeHistoryData data)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                {"Appl_CtrlCd", data.Appl_CtrlCd },
                {"Appl_Dbtr_SurNme", data.Appl_Dbtr_SurNme },
                {"Appl_Dbtr_FrstNme", data.Appl_Dbtr_FrstNme },
                {"Appl_Dbtr_MddleNme", (data.Appl_Dbtr_MddleNme == null) ? DBNull.Value : data.Appl_Dbtr_MddleNme },
                {"Appl_Dbtr_Cnfrmd_SIN", data.Appl_Dbtr_Cnfrmd_SIN },
                {"AppLiSt_Cd", data.AppLiSt_Cd },
                {"SINChangeHistoryComment", data.SINChangeHistoryComment }
            };

            if (!string.IsNullOrEmpty(data.SINChangeHistoryUser))
                parameters.Add("SINChangeHistoryUser", data.SINChangeHistoryUser);

            await MainDB.ExecProcAsync("InsertSINChangeHistory", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
            {
                return false;
            }
            else
                return true;
        }
    }
}
