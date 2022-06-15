using System;

namespace FileBroker.Model
{
    public class RequestLogData
    {
        public int RequestLogId { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string MaintenanceAction { get; set; }
        public string MaintenanceLifeState { get; set; }
        public DateTime LoadedDateTime { get; set; }
    }
}
