using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;

namespace FOAEA3.Data.DB
{

    internal class DBTracing : DBbase, ITracingRepository
    {
        public DBTracing(IDBTools mainDB) : base(mainDB)
        {

        }

        public TracingApplicationData GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {

            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                        {"Appl_CtrlCd", appl_CtrlCd }
                    };

            List<TracingApplicationData> data = MainDB.GetDataFromStoredProc<TracingApplicationData>("TrcApplDtlGetTrc", parameters, FillDataFromReader);

            return data.FirstOrDefault(); // returns null if no data found

        }

        public List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EnfSrv_Cd", enfSrv_Cd},
                {"cycle", cycle}
            };

            return MainDB.GetDataFromStoredProc<TraceCycleQuantityData>("MessageBrokerRequestedTRCINCycleQuantityData", parameters,
                                                                        FillTraceCycleQuantityDataFromReader);
        }

        public void CreateTracingData(TracingApplicationData data)
        {
            ChangeTracingData(data, "TrcApplDtlInsert");
        }

        public void UpdateTracingData(TracingApplicationData data)
        {
            ChangeTracingData(data, "TrcApplDtlUpdate");
        }

        private void ChangeTracingData(TracingApplicationData data, string procName)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                        {"Appl_CtrlCd", data.Appl_CtrlCd },
                        {"Trace_Child_Text", (object) data.Trace_Child_Text ?? DBNull.Value },
                        {"Trace_Breach_Text", (object) data.Trace_Breach_Text ?? DBNull.Value },
                        {"Trace_ReasGround_Text", (object) data.Trace_ReasGround_Text ?? DBNull.Value },
                        {"FamPro_Cd", data.FamPro_Cd },
                        {"Statute_Cd", (object) data.Statute_Cd ?? DBNull.Value },
                        {"Trace_Cycl_Qty", data.Trace_Cycl_Qty },
                        {"Trace_LstCyclStr_Dte", data.Trace_LstCyclStr_Dte },
                        {"Trace_LstCyclCmp_Dte", data.Trace_LstCyclCmp_Dte.HasValue ? data.Trace_LstCyclCmp_Dte : DBNull.Value },
                        {"Trace_LiSt_Cd", data.Trace_LiSt_Cd },
                        {"InfoBank_Cd", (object) data.InfoBank_Cd ?? DBNull.Value }
                    };

            MainDB.ExecProc(procName, parameters);

        }

        public bool TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                        {"Appl_CtrlCd", appl_CtrlCd }
                    };

            int count = MainDB.GetDataFromStoredProc<int>("TrcApplRecordExistsInTrcTable", parameters);

            return count > 0;
        }

        public DataList<TracingApplicationData> GetApplicationsWaitingForAffidavit()
        {
            var result = new DataList<TracingApplicationData>();

            var appDB = new DBApplication(MainDB);
            var data = appDB.GetApplicationsWaitingForAffidavit("T01");

            var provinceCode = CurrentSubmitter.AsSpan(0, 2);
            //var provinceCode = CurrentSubmitter.Substring(0, 2).ToLower();

            var tracingData = new List<TracingApplicationData>();
            foreach (var item in data.Items)
            {
                var firstTwoCharacters = item.Appl_EnfSrv_Cd.AsSpan(0, 2);
                //var firstTwoCharacters = item.Appl_EnfSrv_Cd.Substring(0, 2).ToLower();

                //if (firstTwoCharacters == provinceCode) {
                if (firstTwoCharacters.CompareTo(provinceCode, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var tracing = new TracingApplicationData();
                    tracing.Merge(item);
                    tracingData.Add(tracing);
                }
            }
            result.Items = tracingData;
            result.Messages = data.Messages;

            return result;
        }

        public List<TraceToApplData> GetTraceToApplData()
        {
            return MainDB.GetAllData<TraceToApplData>("MessageBrokerGetTRACEInboundToApplData", FillTraceToApplDataFromReader);
        }

        public List<TracingOutgoingFederalData> GetFederalOutgoingData(int maxRecords,
                                                                       string activeState,
                                                                       ApplicationState lifeState,
                                                                       string enfServiceCode)
        {

            var parameters = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "sntAppLiSt_Cd", lifeState },
                { "chrEnfSrv_Cd", enfServiceCode }
            };

            return MainDB.GetRecordsFromStoredProc<TracingOutgoingFederalData>("MessageBrokerGetTRCOUTOutboundData",
                                                                               parameters, FillTracingOutgoingFederalRecord);

        }

        private void FillTracingOutgoingFederalRecord(IDBHelperReader rdr, out TracingOutgoingFederalData data)
        {
            data = new TracingOutgoingFederalData(
                Event_dtl_Id : (int)rdr["Event_dtl_Id"],
                Event_Reas_Cd : (rdr["Event_Reas_Cd"] != null) ? (int)rdr["Event_Reas_Cd"] : default,
                Event_Reas_Text : (rdr["Event_Reas_Text"] != null) ? rdr["Event_Reas_Text"] as string : default,
                ActvSt_Cd : rdr["ActvSt_Cd"] as string,
                Recordtype : rdr["Recordtype"] as string,
                Appl_Dbtr_Cnfrmd_SIN : rdr["Val_1"] as string,
                Appl_EnfSrv_Cd : rdr["Val_2"] as string,
                Appl_CtrlCd : rdr["Val_3"] as string,
                ReturnType : (int)rdr["Val_4"]
            );
        }

        public List<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords,
                                                                             string activeState,
                                                                             string recipientCode,
                                                                             bool isXML = true)
        {

            var parameters = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "chrRecptCd", recipientCode },
                { "isXML", isXML ? 1 : 0 }
            };

            var data = MainDB.GetDataFromStoredProc<TracingOutgoingProvincialData>("MessageBrokerGetTRCAPPOUTOutboundData",
                                                                               parameters, FillTracingOutgoingProvincialData);
            return data;

        }

        private void FillTracingOutgoingProvincialData(IDBHelperReader rdr, TracingOutgoingProvincialData data)
        {
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Recordtype = rdr["Recordtype"] as string;
            data.Appl_EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_CtrlCd = rdr["Val_3"] as string;
            data.Appl_Source_RfrNr = rdr["Val_4"] as string;
            data.Subm_Recpt_SubmCd = rdr["Val_5"] as string;
            data.TrcRsp_Rcpt_Dte = (DateTime)rdr["Val_6"];
            data.TrcRsp_SeqNr = rdr["Val_7"] as string;
            data.TrcSt_Cd = rdr["Val_8"] as string;
            data.AddrTyp_Cd = rdr["Val_9"] as string;
            data.TrcRsp_EmplNme = rdr["Val_10"] as string;
            data.TrcRsp_EmplNme1 = rdr["Val_11"] as string;
            data.TrcRsp_Addr_Ln = rdr["Val_12"] as string;
            data.TrcRsp_Addr_Ln1 = rdr["Val_13"] as string;
            data.TrcRsp_Addr_CityNme = rdr["Val_14"] as string;
            data.TrcRsp_Addr_PrvCd = rdr["Val_15"] as string;
            data.TrcRsp_Addr_CtryCd = rdr["Val_16"] as string;
            data.TrcRsp_Addr_PCd = rdr["Val_17"] as string;
            data.TrcRsp_Addr_LstUpdte = (DateTime)rdr["Val_18"];
            data.Prcs_RecType = (int)rdr["Val19"];

            var enfSrvCd = rdr["Val20"] as string;
            switch (enfSrvCd.Trim())
            {
                case "HR01": data.EnfSrv_Cd = "UI00"; break;
                case "EI02": data.EnfSrv_Cd = "EI00"; break;
                case "RC01": data.EnfSrv_Cd = "RC00"; break;
                default:
                    break;
            }
        }

        //private void FillTracingOutgoingFederalData(IDBHelperReader rdr, TracingOutgoingFederalData data)
        //{
        //    data.Event_dtl_Id = (int)rdr["Event_dtl_Id"];
        //    if (rdr["Event_Reas_Cd"] != null)
        //        data.Event_Reas_Cd = (int)rdr["Event_Reas_Cd"];
        //    if (rdr["Event_Reas_Text"] != null)
        //        data.Event_Reas_Text = rdr["Event_Reas_Text"] as string;
        //    data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        //    data.Recordtype = rdr["Recordtype"] as string;
        //    data.Appl_Dbtr_Cnfrmd_SIN = rdr["Val_1"] as string;
        //    data.Appl_EnfSrv_Cd = rdr["Val_2"] as string;
        //    data.Appl_CtrlCd = rdr["Val_3"] as string;
        //    data.ReturnType = (int)rdr["Val_4"];
        //}

        private void FillTraceToApplDataFromReader(IDBHelperReader rdr, TraceToApplData data)
        {
            data.Event_Id = (int)rdr["Event_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Tot_Childs = (int)rdr["Tot_Childs"];
            data.Tot_Closed = (int)rdr["Tot_Closed"];
            data.Tot_Invalid = (int)rdr["Tot_Invalid"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.AppLiSt_Cd = (ApplicationState)rdr["AppLiSt_Cd"];
        }

        private void FillTraceCycleQuantityDataFromReader(IDBHelperReader rdr, TraceCycleQuantityData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Trace_Cycl_Qty = (int)rdr["Trace_Cycl_Qty"];
        }

        private void FillDataFromReader(IDBHelperReader rdr, TracingApplicationData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Trace_Child_Text = rdr["Trace_Child_Text"] as string; // can be null 
            data.Trace_Breach_Text = rdr["Trace_Breach_Text"] as string; // can be null 
            data.Trace_ReasGround_Text = rdr["Trace_ReasGround_Text"] as string; // can be null 
            data.FamPro_Cd = rdr["FamPro_Cd"] as string;
            data.Statute_Cd = rdr["Statute_Cd"] as string; // can be null 
            data.Trace_Cycl_Qty = (int)rdr["Trace_Cycl_Qty"];
            data.Trace_LstCyclStr_Dte = (DateTime)rdr["Trace_LstCyclStr_Dte"];
            data.Trace_LstCyclCmp_Dte = rdr["Trace_LstCyclCmp_Dte"] as DateTime?; // can be null 
            data.Trace_LiSt_Cd = (short)rdr["Trace_LiSt_Cd"];
            data.InfoBank_Cd = rdr["InfoBank_Cd"] as string; // can be null 
        }

    }
}
