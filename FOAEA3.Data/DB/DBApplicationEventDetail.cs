using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace FOAEA3.Data.DB
{

    internal class DBApplicationEventDetail : DBbase, IApplicationEventDetailRepository
    {
        public DBApplicationEventDetail(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<ApplicationEventDetailData> GetApplicationEventDetails(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            List<ApplicationEventDetailData> data = null;

            switch (queue)
            {
                case EventQueue.EventBFN_dtl:
                    if (string.IsNullOrEmpty(activeState))
                        parameters.Add("ActvSt_Cd", "A");
                    else
                        parameters.Add("ActvSt_Cd", activeState);

                    data = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("GetEventBFNDTLforI01",
                                                                                    parameters, FillEventDetailDataFromReader);
                    foreach (var item in data)
                        item.Queue = EventQueue.EventSIN_dtl;

                    break;
                case EventQueue.EventSIN_dtl:
                    data = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("EvntSIN_dtl_SelectForApplication",
                                                                                    parameters, FillEventDetailDataFromReader);
                    foreach (var item in data)
                        item.Queue = EventQueue.EventSIN_dtl;

                    break;

                case EventQueue.EventTrace_dtl:
                    data = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("EvntTrace_dtl_SelectForApplication",
                                                                                    parameters, FillEventDetailDataFromReader);
                    foreach (var item in data)
                        item.Queue = EventQueue.EventTrace_dtl;

                    break;
            }

            // TODO: add other events associated to application?

            if (string.IsNullOrEmpty(activeState))
                return data;
            else
                return data?.FindAll(m => m.ActvSt_Cd == activeState);
        }

        public ApplicationEventDetailData GetEventSINDetailDataForEventID(int eventID)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Event_ID", eventID}
                };

            var data = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("EventSIN_dtl_SelectForEventSIN",
                                                                                parameters, FillEventDetailDataFromReader);

            foreach (var item in data)
                item.Queue = EventQueue.EventSIN_dtl;

            return data.FirstOrDefault(); // returns null if no data found
        }

        public List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle)
        {
            var parameters = new Dictionary<string, object>
            {
                { "EnfSrv_Cd", enfSrv_Cd },
                { "cycle", cycle }
            };

            var result = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("MessageBrokerRequestedTRCINEventDetailData", parameters,
                                                                                  FillEventDetailDataFromReader);

            foreach (var item in result)
                item.Queue = EventQueue.EventTrace_dtl;

            return result;
        }

        public DataList<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "EnfSrv_Cd", enfSrv_Cd },
                { "fileName", fileName }
            };

            var resultData = MainDB.GetDataFromStoredProc<ApplicationEventDetailData>("MessageBrokerRequestedSININEventDetailData", parameters,
                                                                                      FillEventDetailDataFromReader);

            foreach (var item in resultData)
                item.Queue = EventQueue.EventSIN_dtl;

            var result = new DataList<ApplicationEventDetailData>
            {
                Items = resultData
            };

            if (!string.IsNullOrEmpty(MainDB.LastError))
                result.Messages.AddSystemError(MainDB.LastError);

            return result;
        }


        public bool SaveEventDetail(ApplicationEventDetailData eventDetailData)
        {
            bool success = false;

            switch (eventDetailData.Queue)
            {
                case EventQueue.EventBFN_dtl:
                    if (eventDetailData.Event_dtl_Id == 0)
                        success = CreateEventDetail(eventDetailData, "EvntBFN");
                    else
                        success = UpdateBFNEventDetai(eventDetailData);
                    break;

                case EventQueue.EventTrace_dtl:
                    if (eventDetailData.Event_dtl_Id == 0)
                        success = CreateEventDetail(eventDetailData, "EvntTrace");
                    else
                        success = UpdateTracingEventDetai(eventDetailData);
                    break;

                case EventQueue.EventSIN_dtl:
                    if (eventDetailData.Event_dtl_Id == 0)
                        success = CreateEventDetail(eventDetailData, "EvntSIN");
                    else
                        success = UpdateSINEventDetail(eventDetailData);
                    break;

            }

            // TODO: other event details types

            return success;
        }

        public bool SaveEventDetails(List<ApplicationEventDetailData> eventDetailsData)
        {
            bool success = true;

            foreach (var eventDetail in eventDetailsData)
                if (!SaveEventDetail(eventDetail))
                    success = false;

            return success;
        }

        private bool CreateEventDetail(ApplicationEventDetailData eventDetailData, string tableName)
        {
            bool success = false;

            var parameters = new Dictionary<string, object>
                    {
                        { "rvchTableName", tableName},
                        { "rintEvent_id", eventDetailData.Event_Id},
                        { "rdtmTimeStamp", eventDetailData.Event_TimeStamp},
                        { "dchrPriority_Ind", eventDetailData.Event_Priority_Ind},
                        { "ddtmEffctv_Dte", eventDetailData.Event_Effctv_Dte},
                        { "dchrActvSt_Cd", eventDetailData.ActvSt_Cd},
                        { "dsntAppList_Cd", eventDetailData.AppLiSt_Cd}
                    };

            if (eventDetailData.Event_Compl_Dte.HasValue)
                parameters.Add("ddtmCompl_Dte", eventDetailData.Event_Compl_Dte.Value);

            if (eventDetailData.Event_Reas_Cd.HasValue)
                parameters.Add("dintReas_Cd", (int)eventDetailData.Event_Reas_Cd.Value);

            if (!string.IsNullOrWhiteSpace(eventDetailData.Event_Reas_Text))
                parameters.Add("dvchReas_Text", eventDetailData.Event_Reas_Text);

            MainDB.ExecProc("fp_sub_EvntdtlCreate", parameters);

            if (string.IsNullOrEmpty(MainDB.LastError))
                success = true;

            return success;
        }
                
        private bool UpdateBFNEventDetai(ApplicationEventDetailData eventDetailData)
        {
            bool success = false;

            var parameters = new Dictionary<string, object>
                    {
                        { "rintEvent_dtl_Id", eventDetailData.Event_dtl_Id},
                        { "dintReas_Cd", eventDetailData.Event_Reas_Cd},
                        { "dvchReas_Text", eventDetailData.Event_Reas_Text},
                        { "ddtmEvent_Effctv_Dte", eventDetailData.Event_Effctv_Dte},
                        { "dsmtAppLiSt_Cd", eventDetailData.ActvSt_Cd},
                        { "dchrActvSt_Cd", eventDetailData.AppLiSt_Cd}
                    };

            MainDB.ExecProc("fp_PrcsSub_EvntUpdate", parameters);

            if (string.IsNullOrEmpty(MainDB.LastError))
                success = true;

            return success;
        }
        
        private bool UpdateTracingEventDetai(ApplicationEventDetailData eventDetailData)
        {
            bool success = false;

            var parameters = new Dictionary<string, object>
                    {
                        { "Event_dtl_Id", eventDetailData.Event_dtl_Id},
                        { "EnfSrv_Cd", eventDetailData.EnfSrv_Cd},
                        { "Event_Id", eventDetailData.Event_Id},
                        { "Event_TimeStamp", eventDetailData.Event_TimeStamp},
                        { "Event_Compl_Dte", eventDetailData.Event_Compl_Dte},
                        { "Event_Reas_Cd", eventDetailData.Event_Reas_Cd},
                        { "Event_Reas_Text", eventDetailData.Event_Reas_Text},
                        { "Event_Priority_Ind", eventDetailData.Event_Priority_Ind},
                        { "Event_Effctv_Dte", eventDetailData.Event_Effctv_Dte},
                        { "AppLiSt_Cd", eventDetailData.ActvSt_Cd},
                        { "ActvSt_Cd", eventDetailData.AppLiSt_Cd}
                    };

            MainDB.ExecProc("MessageBrokerEventTraceDetailUpdate", parameters);

            if (string.IsNullOrEmpty(MainDB.LastError))
                success = true;

            return success;
        }

        private bool UpdateSINEventDetail(ApplicationEventDetailData eventData)
        {
            bool success = false;

            var parameters = new Dictionary<string, object>
            {
                { "Event_dtl_Id", eventData.Event_dtl_Id },
                { "EnfSrv_Cd", eventData.EnfSrv_Cd },
                { "Event_Id", eventData.Event_Id },
                { "Event_TimeStamp", eventData.Event_TimeStamp },
                { "Event_Priority_Ind", eventData.Event_Priority_Ind },
                { "Event_Effctv_Dte", eventData.Event_Effctv_Dte },
                { "ActvSt_Cd", eventData.ActvSt_Cd },
                { "AppLiSt_Cd", (int) eventData.AppLiSt_Cd }
            };

            if (eventData.Event_Compl_Dte.HasValue)
                parameters.Add("Event_Compl_Dte", eventData.Event_Compl_Dte.Value);

            if (eventData.Event_Reas_Cd.HasValue)
                parameters.Add("Event_Reas_Cd", (int)eventData.Event_Reas_Cd.Value);
            else
                parameters.Add("Event_Reas_Cd", DBNull.Value);

            if (!string.IsNullOrWhiteSpace(eventData.Event_Reas_Text))
                parameters.Add("Event_Reas_Text", eventData.Event_Reas_Text);
            else
                parameters.Add("Event_Reas_Text", DBNull.Value);

            _ = MainDB.ExecProc("MessageBrokerEventSINDetailUpdate", parameters);

            if (string.IsNullOrWhiteSpace(MainDB.LastError))
                success = true;

            return success;
        }

        public void UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode,
                                              string writtenFile, List<int> eventIds)
        {
            string xmlEventList = ConvertEventIdsListToXml(eventIds);

            var parameters = new Dictionary<string, object>
            {
                {"actStCode", activeState },
                {"appLiStCode", applicationState },
                {"enfSvrCode", enfSrvCode },
                {"sWrittenFile", writtenFile },
                {"Event_dtl_Ids", xmlEventList }
            };

            MainDB.ExecProc("MessageBrokerOutboundUpdateEventDetail", parameters);

        }

        private static string ConvertEventIdsListToXml(List<int> eventIds)
        {
            var x = new XElement("FTPEvents", eventIds.Select(eventId => new XElement("Event_dtl_Id", eventId)));

            return x.ToString();
        }

        private static void FillEventDetailDataFromReader(IDBHelperReader rdr, ApplicationEventDetailData data)
        {
            data.Event_dtl_Id = (int)rdr["Event_dtl_Id"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Event_Id = rdr["Event_Id"] as int?; // can be null 
            data.Event_TimeStamp = (DateTime)rdr["Event_TimeStamp"];
            data.Event_Compl_Dte = rdr["Event_Compl_Dte"] as DateTime?; // can be null 
            if (rdr["Event_Reas_Cd"] != null)  // can be null
            {
                int reasonCode = (int)rdr["Event_Reas_Cd"];
                data.Event_Reas_Cd = (EventCode)reasonCode;
            }
            data.Event_Reas_Text = rdr["Event_Reas_Text"] as string; // can be null 
            data.Event_Priority_Ind = rdr["Event_Priority_Ind"] as string; // can be null 
            data.Event_Effctv_Dte = rdr["Event_Effctv_Dte"] as DateTime?; // can be null 
            data.AppLiSt_Cd = (short)rdr["AppLiSt_Cd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
