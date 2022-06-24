using DBHelper;
using FileBroker.Model.Interfaces;
using System.Collections.Generic;

namespace FileBroker.Data.DB
{
    public class DBLoadInboundAudit : ILoadInboundAuditRepository
    {
        private IDBTools MainDB { get; }

        public DBLoadInboundAudit(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public void AddNew(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", applEnfSrvCd },
                {"Appl_CtrlCd", applCtrlCd },
                {"Appl_Source_RfrNr", sourceRefNumber },
                {"InboundFilename", inboundFileName }
            };

            MainDB.ExecProc("LoadInboundAuditInsert", parameters);
        }

        public bool AlreadyExists(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", applEnfSrvCd },
                {"Appl_CtrlCd", applCtrlCd },
                {"Appl_Source_RfrNr", sourceRefNumber },
                {"InboundFilename", inboundFileName }
            };

            int count = MainDB.GetDataFromStoredProcViaReturnParameter<int>("LoadInboundAuditSearch", parameters, "found");

            return (count > 0);
        }

        public void MarkAsCompleted(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", applEnfSrvCd },
                {"Appl_CtrlCd", applCtrlCd },
                {"Appl_Source_RfrNr", sourceRefNumber },
                {"InboundFilename", inboundFileName }
            };

            MainDB.ExecProc("LoadInboundAuditUpdate", parameters);
        }
    }
}
