using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBApplicationEvent : DBbase, IApplicationEventRepository
    {
        public DBApplicationEvent(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<ApplicationEventsList> GetApplicationEvents(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            List<ApplicationEventData> thisData = null;
            var data = new ApplicationEventsList();

            switch (queue)
            {
                case EventQueue.EventAM:
                case EventQueue.EventDbtr:
                case EventQueue.EventLicence:
                case EventQueue.EventSYS:
                case EventQueue.EventTrace_ESDC:
                    // TODO: add other events associated to application?
                    break;

                case EventQueue.EventBF:
                    thisData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("EvntBF_SelectForApplication",
                                                                              parameters, FillEventDataFromReader);

                    foreach (var item in thisData)
                    {
                        item.Queue = EventQueue.EventSubm;
                        if ((string.IsNullOrEmpty(activeState)) ||
                            (!string.IsNullOrEmpty(activeState) && (item.ActvSt_Cd == activeState)))
                            data.Add(item);
                    }

                    break;

                case EventQueue.EventBFN:
                    if (string.IsNullOrEmpty(activeState))
                        parameters.Add("ActvSt_Cd", "A");
                    else
                        parameters.Add("ActvSt_Cd", activeState);

                    thisData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("GetEventBFNforI01",
                                                                              parameters, FillEventDataFromReader);

                    foreach (var item in thisData)
                    {
                        item.Queue = EventQueue.EventBFN;
                        if ((string.IsNullOrEmpty(activeState)) ||
                            (!string.IsNullOrEmpty(activeState) && (item.ActvSt_Cd == activeState)))
                            data.Add(item);
                    }

                    break;

                case EventQueue.EventSIN:
                    thisData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("EvntSIN_SelectForApplication",
                                                                              parameters, FillEventDataFromReader);
                    foreach (var item in thisData)
                    {
                        item.Queue = EventQueue.EventSIN;
                        if ((string.IsNullOrEmpty(activeState)) ||
                            (!string.IsNullOrEmpty(activeState) && (item.ActvSt_Cd == activeState)))
                            data.Add(item);
                    }

                    break;

                case EventQueue.EventSubm:
                    thisData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("EvntSubm_SelectForApplication",
                                                                              parameters, FillEventDataFromReader);
                    foreach (var item in thisData)
                    {
                        item.Queue = EventQueue.EventSubm;
                        if ((string.IsNullOrEmpty(activeState)) ||
                            (!string.IsNullOrEmpty(activeState) && (item.ActvSt_Cd == activeState)))
                            data.Add(item);
                    }

                    break;

                case EventQueue.EventTrace:
                    thisData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("EvntTrace_SelectForApplication",
                                                                              parameters, FillEventDataFromReader);
                    foreach (var item in thisData)
                    {
                        item.Queue = EventQueue.EventTrace;
                        if ((string.IsNullOrEmpty(activeState)) ||
                            (!string.IsNullOrEmpty(activeState) && (item.ActvSt_Cd == activeState)))
                            data.Add(item);
                    }

                    break;

            }

            return data;
        }

        public async Task<ApplicationEventsList> GetEventBF(string subm_SubmCd, string appl_CtrlCd, EventCode eventCode, string activeState)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Subm_SubmCd", subm_SubmCd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"Event_Reas_Cd", eventCode },
                    {"ActvSt_Cd", activeState }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("GetEventBF", parameters, FillEventDataFromReader);

            var eventsData = new ApplicationEventsList();
            foreach (var item in data)
            {
                item.Queue = EventQueue.EventBF;
                eventsData.Add(item);
            }

            return eventsData; // returns null if no data found
        }

        public async Task<ApplicationEventsList> GetActiveEventBFs()
        {
            var data = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("GetActiveEvntBF", FillEventDataFromReader);
            var eventsData = new ApplicationEventsList();

            foreach (var item in data)
            {
                item.Queue = EventQueue.EventBF;
                eventsData.Add(item);
            }

            return eventsData; // returns null if no data found
        }

        public async Task<bool> SaveEvents(ApplicationEventsList events,
                                           ApplicationState applicationState = ApplicationState.UNDEFINED,
                                           string activeState = "")
        {
            bool success = true;

            foreach (var eventData in events)
            {
                success = await SaveEvent(eventData, applicationState, activeState);

                if (!success)
                    break;

            }

            return success;
        }

        public async Task<bool> SaveEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                              string activeState = "")
        {
            bool success;

            if (eventData.Event_Id == 0)
                success = await CreateEvent(eventData, applicationState, activeState);
            else
                success = await UpdateEvent(eventData, applicationState, activeState);

            return success;
        }

        private async Task<bool> CreateEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                                string activeState = "")
        {
            if (applicationState == ApplicationState.UNDEFINED)
                applicationState = eventData.AppLiSt_Cd;

            if (activeState == "")
                activeState = eventData.ActvSt_Cd;

            var parameters = new Dictionary<string, object>
            {
                { "rvchTableName", eventData.Queue.ToString().Replace("Event", "Evnt") },
                { "rchrSubm_SubmCd", eventData.Subm_SubmCd },
                { "rchrAppl_CtrlCd", eventData.Appl_CtrlCd },
                { "rdtmTimeStamp", eventData.Event_TimeStamp },
                { "rchrSubm_Recpt", eventData.Subm_Recpt_SubmCd },
                { "dintReas_Cd", (int) (eventData.Event_Reas_Cd ?? 0) },
                { "dchrPriority_Ind", eventData.Event_Priority_Ind },
                { "ddtmEffctv_Dte", eventData.Event_Effctv_Dte },
                { "dchrActvSt_Cd", activeState },
                { "dsntAppList_Cd", (int) applicationState },
                { "rchrSubm_Update", eventData.Subm_Update_SubmCd },
                { "chrEnfSrvCd", eventData.Appl_EnfSrv_Cd }
            };

            if (eventData.Event_Compl_Dte.HasValue)
                parameters.Add("ddtmCompl_Dte", eventData.Event_Compl_Dte.Value);

            if (!string.IsNullOrWhiteSpace(eventData.Event_Reas_Text))
                parameters.Add("dvchReas_Text", eventData.Event_Reas_Text);

            _ = await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("fp_sub_EvntCreate", parameters, "dintEvent_Id");
            if (!string.IsNullOrWhiteSpace(MainDB.LastError))
                return false;
            else
                return true;

        }

        private async Task<bool> UpdateEvent(ApplicationEventData eventData, ApplicationState applicationState = ApplicationState.UNDEFINED,
                                string activeState = "")
        {
            var parameters = new Dictionary<string, object>();

            switch (eventData.Queue)
            {
                case EventQueue.EventAM:
                case EventQueue.EventDbtr:
                case EventQueue.EventLicence:
                case EventQueue.EventSYS:
                case EventQueue.EventTrace_ESDC:
                case EventQueue.EventCR_PADR:
                    // TODO: other types of events
                    break;

                case EventQueue.EventBF:
                    parameters.Add("Event_Id", eventData.Event_Id);
                    parameters.Add("Event_Effctv_Dte", eventData.Event_Effctv_Dte);
                    parameters.Add("AppLiSt_Cd", applicationState == ApplicationState.UNDEFINED ? eventData.AppLiSt_Cd : applicationState);
                    parameters.Add("ActvSt_Cd", string.IsNullOrEmpty(activeState) ? eventData.ActvSt_Cd : activeState);

                    await MainDB.ExecProcAsync("EvntBF_Update", parameters);
                    break;

                case EventQueue.EventBFN:
                    parameters.Add("Event_Id", eventData.Event_Id);
                    parameters.Add("Appl_EnfSrv_Cd", eventData.Appl_EnfSrv_Cd);
                    parameters.Add("Appl_CtrlCd", eventData.Appl_CtrlCd);
                    parameters.Add("Subm_Recpt_SubmCd", eventData.Subm_Recpt_SubmCd);
                    parameters.Add("Event_TimeStamp", eventData.Event_TimeStamp);
                    parameters.Add("Event_Compl_Dte", eventData.Event_Compl_Dte);
                    parameters.Add("Event_Reas_Cd", eventData.Event_Reas_Cd);
                    parameters.Add("Event_Reas_Text", eventData.Event_Reas_Text);
                    parameters.Add("Event_Priority_Ind", eventData.Event_Priority_Ind);
                    parameters.Add("Event_Effctv_Dte", eventData.Event_Effctv_Dte);
                    parameters.Add("AppLiSt_Cd", eventData.AppLiSt_Cd);
                    parameters.Add("ActvSt_Cd", eventData.ActvSt_Cd);

                    await MainDB.ExecProcAsync("EvntBFN_Update", parameters);
                    break;

                case EventQueue.EventTrace:
                    parameters.Add("Event_Id", eventData.Event_Id);
                    parameters.Add("Appl_EnfSrv_Cd", eventData.Appl_EnfSrv_Cd);
                    parameters.Add("Appl_CtrlCd", eventData.Appl_CtrlCd);
                    parameters.Add("Subm_Recpt_SubmCd", eventData.Subm_Recpt_SubmCd);
                    parameters.Add("Event_TimeStamp", eventData.Event_TimeStamp);
                    parameters.Add("Event_Compl_Dte", eventData.Event_Compl_Dte);
                    parameters.Add("Event_Reas_Cd", eventData.Event_Reas_Cd);
                    parameters.Add("Event_Reas_Text", eventData.Event_Reas_Text);
                    parameters.Add("Event_Priority_Ind", eventData.Event_Priority_Ind);
                    parameters.Add("Event_Effctv_Dte", eventData.Event_Effctv_Dte);
                    parameters.Add("AppLiSt_Cd", eventData.AppLiSt_Cd);
                    parameters.Add("ActvSt_Cd", eventData.ActvSt_Cd);
                    parameters.Add("processId", 0); // not actually used by the proc?

                    await MainDB.ExecProcAsync("MessageBrokerEventTraceUpdate", parameters);
                    break;

                case EventQueue.EventSIN:
                    parameters.Add("Event_Id", eventData.Event_Id);
                    parameters.Add("Appl_CtrlCd", eventData.Appl_CtrlCd);
                    parameters.Add("Event_TimeStamp", eventData.Event_TimeStamp);
                    parameters.Add("Subm_Recpt_SubmCd", eventData.Subm_Recpt_SubmCd);
                    parameters.Add("Event_Reas_Cd", (int)(eventData.Event_Reas_Cd ?? 0));
                    parameters.Add("Event_Priority_Ind", eventData.Event_Priority_Ind);
                    parameters.Add("Event_Effctv_Dte", eventData.Event_Effctv_Dte);
                    parameters.Add("ActvSt_Cd", eventData.ActvSt_Cd);
                    parameters.Add("AppLiSt_Cd", (int)eventData.AppLiSt_Cd);
                    parameters.Add("Appl_EnfSrv_Cd", eventData.Appl_EnfSrv_Cd);
                    parameters.Add("Event_Reas_Text", eventData.Event_Reas_Text ?? string.Empty);

                    if (eventData.Event_Compl_Dte.HasValue)
                        parameters.Add("Event_Compl_Dte", eventData.Event_Compl_Dte.Value);

                    await MainDB.ExecProcAsync("MessageBrokerEventSINUpdate", parameters);
                    break;
            }

            if (string.IsNullOrEmpty(MainDB.LastError))
                return true;
            else
                return false;
        }

        public async Task CloseNETPTraceEvents()
        {
            await MainDB.ExecProcAsync("MessageBrokerCloseESDCTraceEvents");
        }

        public async Task<int> GetTraceEventCount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime receivedAffidavitDate,
                                      EventCode eventReasonCode, int eventId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                {"Appl_CtrlCd", appl_CtrlCd},
                {"Appl_RecvAffdvt_Dte", receivedAffidavitDate},
                {"Event_Reas_Cd", (int) eventReasonCode},
                {"EventId", eventId},
            };

            return await MainDB.GetDataFromProcSingleValueAsync<int>("EvntTraceGetEventCount", parameters);
        }

        public async Task<ApplicationEventsList> GetRequestedTRCINTracingEvents(string enfSrv_Cd, string cycle)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EnfSrv_Cd", enfSrv_Cd},
                {"cycle", cycle}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("MessageBrokerRequestedTRCINEventData", parameters,
                                                                            FillEventDataFromReader);
            var eventsData = new ApplicationEventsList();
            foreach (var item in result)
            {
                item.Queue = EventQueue.EventTrace;
                eventsData.Add(item);
            }

            return eventsData;
        }

        public async Task<ApplicationEventsList> GetRequestedLICINLicenceDenialEvents(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                               string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EnfSrv_Cd", enfSrv_Cd},
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                {"Appl_CtrlCd", appl_CtrlCd}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("MessageBrokerRequestedLICINEventData", parameters,
                                                                            FillEventDataFromReader);
            var eventsData = new ApplicationEventsList();

            foreach (var item in result)
            {
                item.Queue = EventQueue.EventLicence;
                eventsData.Add(item);
            }
            return eventsData;
        }

        public async Task<ApplicationEventsList> GetRequestedSINEventDataForFile(string enfSrv_Cd, string fileName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "EnfSrv_Cd", enfSrv_Cd },
                { "fileName", fileName }
            };

            var resultData = await MainDB.GetDataFromStoredProcAsync<ApplicationEventData>("MessageBrokerRequestedSININEventData", parameters,
                                                                                FillEventDataFromReader);
            var eventsData = new ApplicationEventsList();

            foreach (var item in resultData)
            {
                item.Queue = EventQueue.EventSIN;
                eventsData.Add(item);
            }

            return eventsData;
        }

        public async Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary()
        {
            return await MainDB.GetDataFromStoredProcAsync<SinInboundToApplData>("MessageBrokerGetSINInboundToApplData", FillLatestSinEventDataFromReader);
        }

        private void FillLatestSinEventDataFromReader(IDBHelperReader rdr, SinInboundToApplData data)
        {
            data.Event_Id = (int)rdr["Event_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Tot_Childs = (int)rdr["Tot_Childs"];
            data.Tot_Closed = (int)rdr["Tot_Closed"];
            data.Tot_Invalid = (int)rdr["Tot_Invalid"];
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_Dbtr_Cnfrmd_SIN = rdr["Appl_Dbtr_Cnfrmd_SIN"] as string;
            data.Appl_Dbtr_RtrndBySrc_SIN = rdr["Appl_Dbtr_RtrndBySrc_SIN"] as string;
            data.AppLiSt_Cd = (short)rdr["AppLiSt_Cd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.SVR_SIN = rdr["SVR_SIN"] as string;
            data.ValStat_Cd = (short)rdr["ValStat_Cd"];
        }

        public async Task DeleteBFEvent(string subm_SubmCd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Subm_SubmCd", subm_SubmCd },
                        {"Appl_CtrlCd", appl_CtrlCd }
                    };

            await MainDB.ExecProcAsync("EvntBF_DeleteForSubmitterCodeControlCode", parameters);

        }

        public string GetLastError()
        {
            return MainDB.LastError;
        }

        private static void FillEventDataFromReader(IDBHelperReader rdr, ApplicationEventData data)
        {
            data.Event_Id = (int)rdr["Event_Id"];
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string; // can be null 
            if (rdr["Event_Reas_Cd"] != null)  // can be null
            {
                int reasonCode = (int)rdr["Event_Reas_Cd"];
                data.Event_Reas_Cd = (EventCode)reasonCode;
            }
            data.Event_Effctv_Dte = (DateTime)rdr["Event_Effctv_Dte"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.AppLiSt_Cd = (ApplicationState)rdr["AppLiSt_Cd"];

            if (rdr.ColumnExists("Event_TimeStamp")) data.Event_TimeStamp = (DateTime)rdr["Event_TimeStamp"];
            if (rdr.ColumnExists("Event_Compl_Dte")) data.Event_Compl_Dte = rdr["Event_Compl_Dte"] as DateTime?; // can be null 
            if (rdr.ColumnExists("Event_Reas_Text")) data.Event_Reas_Text = rdr["Event_Reas_Text"] as string; // can be null 
            if (rdr.ColumnExists("Event_Priority_Ind")) data.Event_Priority_Ind = rdr["Event_Priority_Ind"] as string; // can be null 
            if (rdr.ColumnExists("Subm_SubmCd")) data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            if (rdr.ColumnExists("Subm_Recpt_SubmCd")) data.Subm_Recpt_SubmCd = rdr["Subm_Recpt_SubmCd"] as string;
            if (rdr.ColumnExists("Event_RecptSubm_ActvStCd")) data.Event_RecptSubm_ActvStCd = rdr["Event_RecptSubm_ActvStCd"] as string;
            if (rdr.ColumnExists("Subm_Update_SubmCd")) data.Subm_Update_SubmCd = rdr["Subm_Update_SubmCd"] as string; // can be null 
            if (rdr.ColumnExists("Appl_EnfSrv_Cd")) data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string; // can be null 
            if (rdr.ColumnExists("AppCtgy_Cd")) data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string;
            if (rdr.ColumnExists("Appl_Rcptfrm_Dte")) data.Appl_Rcptfrm_Dte = rdr["Appl_Rcptfrm_Dte"] as DateTime?;
        }

    }
}
