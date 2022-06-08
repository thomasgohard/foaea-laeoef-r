using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareEvents
    {
        public static List<DiffData> Run(string tableName, IRepositories repositories2, IRepositories repositories3,
                                         string enfSrv, string ctrlCd, EventQueue queue)
        {
            var diffs = new List<DiffData>();

            var evnt2 = repositories2.ApplicationEventRepository.GetApplicationEvents(enfSrv, ctrlCd, queue);
            var evnt3 = repositories3.ApplicationEventRepository.GetApplicationEvents(enfSrv, ctrlCd, queue);

            foreach (var evntItem2 in evnt2)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{evntItem2.ActvSt_Cd}]/[{evntItem2.Event_TimeStamp}]/[{(int)evntItem2.Event_Reas_Cd}][{(int)evntItem2.AppLiSt_Cd}]";
                var evntItem3 = evnt3.Where(m => ((m.Event_TimeStamp == evntItem2.Event_TimeStamp) && (m.Event_Reas_Cd == evntItem2.Event_Reas_Cd) && (m.AppLiSt_Cd == evntItem2.AppLiSt_Cd)) ||
                                                  ((m.Event_Compl_Dte is null) && (evntItem2.Event_Compl_Dte is null) && (m.Event_Reas_Cd == evntItem2.Event_Reas_Cd) && (m.AppLiSt_Cd == evntItem2.AppLiSt_Cd))).FirstOrDefault();
                if (evntItem3 is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "", badValue: "Missing in FOAEA 3!"));
                else
                    CompareData(tableName, evntItem2, evntItem3, ref diffs, key);
            }

            foreach (var evntItem3 in evnt3)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{evntItem3.ActvSt_Cd}]/[{evntItem3.Event_TimeStamp}]/[{(int)evntItem3.Event_Reas_Cd}][{(int)evntItem3.AppLiSt_Cd}]";
                var evntItem2 = evnt2.Where(m => ((m.Event_TimeStamp == evntItem3.Event_TimeStamp) && (m.Event_Reas_Cd == evntItem3.Event_Reas_Cd) && (m.AppLiSt_Cd == evntItem3.AppLiSt_Cd)) ||
                                                  ((m.Event_Compl_Dte is null) && (evntItem3.Event_Compl_Dte is null) && (m.Event_Reas_Cd == evntItem3.Event_Reas_Cd) && (m.AppLiSt_Cd == evntItem3.AppLiSt_Cd))).FirstOrDefault();
                if (evntItem2 is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "Missing in FOAEA 2!", badValue: ""));
            }

            return diffs;
        }

        private static void CompareData(string tableName, ApplicationEventData evnt2, ApplicationEventData evnt3, ref List<DiffData> diffs, string key)
        {
            if (evnt2.Queue != evnt3.Queue) diffs.Add(new DiffData(tableName, key: key, colName: "Queue", goodValue: evnt2.Queue, badValue: evnt3.Queue));
            if (evnt2.Event_Id!= evnt3.Event_Id) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Id", goodValue: evnt2.Event_Id, badValue: evnt3.Event_Id));
            if (evnt2.Appl_CtrlCd!= evnt3.Appl_CtrlCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_CtrlCd", goodValue: evnt2.Appl_CtrlCd, badValue: evnt3.Appl_CtrlCd));
            if (evnt2.Subm_SubmCd!= evnt3.Subm_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_SubmCd", goodValue: evnt2.Subm_SubmCd, badValue: evnt3.Subm_SubmCd));
            if (evnt2.Appl_EnfSrv_Cd!= evnt3.Appl_EnfSrv_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_EnfSrv_Cd", goodValue: evnt2.Appl_EnfSrv_Cd, badValue: evnt3.Appl_EnfSrv_Cd));
            if (evnt2.Subm_Recpt_SubmCd!= evnt3.Subm_Recpt_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_Recpt_SubmCd", goodValue: evnt2.Subm_Recpt_SubmCd, badValue: evnt3.Subm_Recpt_SubmCd));
            if (evnt2.Event_RecptSubm_ActvStCd!= evnt3.Event_RecptSubm_ActvStCd) diffs.Add(new DiffData(tableName, key: key, colName: "Event_RecptSubm_ActvStCd", goodValue: evnt2.Event_RecptSubm_ActvStCd, badValue: evnt3.Event_RecptSubm_ActvStCd));
            if (evnt2.Appl_Rcptfrm_Dte!= evnt3.Appl_Rcptfrm_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Rcptfrm_Dte", goodValue: evnt2.Appl_Rcptfrm_Dte, badValue: evnt3.Appl_Rcptfrm_Dte));
            if (evnt2.Subm_Update_SubmCd!= evnt3.Subm_Update_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_Update_SubmCd", goodValue: evnt2.Subm_Update_SubmCd, badValue: evnt3.Subm_Update_SubmCd));
            if (evnt2.Event_TimeStamp!= evnt3.Event_TimeStamp) diffs.Add(new DiffData(tableName, key: key, colName: "Event_TimeStamp", goodValue: evnt2.Event_TimeStamp, badValue: evnt3.Event_TimeStamp));
            if (evnt2.Event_Compl_Dte!= evnt3.Event_Compl_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Compl_Dte", goodValue: evnt2.Event_Compl_Dte, badValue: evnt3.Event_Compl_Dte));
            if (evnt2.Event_Reas_Cd!= evnt3.Event_Reas_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Reas_Cd", goodValue: evnt2.Event_Reas_Cd, badValue: evnt3.Event_Reas_Cd));
            if (evnt2.Event_Reas_Text!= evnt3.Event_Reas_Text) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Reas_Text", goodValue: evnt2.Event_Reas_Text, badValue: evnt3.Event_Reas_Text));
            if (evnt2.Event_Priority_Ind!= evnt3.Event_Priority_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Priority_Ind", goodValue: evnt2.Event_Priority_Ind, badValue: evnt3.Event_Priority_Ind));
            if (evnt2.Event_Effctv_Dte!= evnt3.Event_Effctv_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Event_Effctv_Dte", goodValue: evnt2.Event_Effctv_Dte, badValue: evnt3.Event_Effctv_Dte));
            if (evnt2.ActvSt_Cd!= evnt3.ActvSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "ActvSt_Cd", goodValue: evnt2.ActvSt_Cd, badValue: evnt3.ActvSt_Cd));
            if (evnt2.AppLiSt_Cd!= evnt3.AppLiSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "AppLiSt_Cd", goodValue: evnt2.AppLiSt_Cd, badValue: evnt3.AppLiSt_Cd));
            if (evnt2.AppCtgy_Cd != evnt3.AppCtgy_Cd ) diffs.Add(new DiffData(tableName, key: key, colName: "AppCtgy_Cd ", goodValue: evnt2.AppCtgy_Cd , badValue: evnt3.AppCtgy_Cd ));
        }
    }
}
