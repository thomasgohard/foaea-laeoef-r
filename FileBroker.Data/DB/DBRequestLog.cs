using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FileBroker.Data.DB
{
    public class DBRequestLog : IRequestLogRepository
    {
        private IDBTools MainDB { get; }

        public DBRequestLog(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public int Add(RequestLogData requestLogData)
        {
            return MainDB.CreateData<RequestLogData, int>("RequestLog", requestLogData, "RequestLogId", SetParametersForData, default);
        }

        private void SetParametersForData(RequestLogData data, Dictionary<string, object> parameters)
        {
            parameters.Add("Appl_CtrlCd", data.Appl_CtrlCd);
            parameters.Add("Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd);
            parameters.Add("MaintenanceAction", data.MaintenanceAction);
            parameters.Add("MaintenanceLifeState", data.MaintenanceLifeState);
            parameters.Add("LoadedDateTime", data.LoadedDateTime);
        }

        public List<RequestLogData> GetAll()
        {
            return MainDB.GetAllData<RequestLogData>("RequestLog", FillRequestLogDataFromReader);
        }

        public void DeleteAll()
        {
            MainDB.ExecProc("RequestLog_DeleteAll");
        }

        private void FillRequestLogDataFromReader(IDBHelperReader rdr, RequestLogData data)
        {
            data.RequestLogId = (int)rdr["RequestLogId"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.MaintenanceAction = rdr["MaintenanceAction"] as string;
            data.MaintenanceLifeState = rdr["MaintenanceLifeState"] as string;
            data.LoadedDateTime = (DateTime)rdr["LoadedDateTime"];
        }

    }
}
