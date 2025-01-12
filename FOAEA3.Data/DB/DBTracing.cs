﻿using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{

    internal class DBTracing : DBbase, ITracingRepository
    {
        public DBTracing(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<TracingApplicationData> GetTracingData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                        {"Appl_CtrlCd", appl_CtrlCd }
                    };

            var traceData = (await MainDB.GetDataFromStoredProcAsync<TracingApplicationData>("TrcApplDtlGetTrc", parameters, FillDataFromReader))
                                                .FirstOrDefault();

            var financialData = await MainDB.GetDataFromStoredProcAsync<TraceFinancialData>("TraceFin_SelectForAppl", parameters, FillTraceFinDataFromReader);

            if ((traceData is not null) && (financialData is not null))
            {
                foreach (var finData in financialData)
                {
                    var detailsParameters = new Dictionary<string, object> {
                        { "TraceFin_Id", finData.TraceFin_Id }
                    };

                    var detailsData = await MainDB.GetDataFromStoredProcAsync<TraceFinancialDetailData>("TraceFin_Dtl_SelectForTraceFin", detailsParameters, FillFinancialDetailDataFromReader);
                    var taxForms = detailsData.Select(d => d.TaxForm).ToList();

                    traceData.YearsAndTaxForms.Add(finData.FiscalYear, taxForms);
                }

            }

            return traceData;
        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityData(string enfSrv_Cd, string cycle)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EnfSrv_Cd", enfSrv_Cd},
                {"cycle", cycle}
            };

            return await MainDB.GetDataFromStoredProcAsync<TraceCycleQuantityData>("MessageBrokerRequestedTRCINCycleQuantityData", parameters, FillTraceCycleQuantityDataFromReader);
        }

        public async Task CreateTracingData(TracingApplicationData data)
        {
            await ChangeTracingData(data, "TrcApplDtlInsert");

            await CreateYearsAndTaxForms(data);
        }

        public async Task UpdateTracingData(TracingApplicationData data)
        {
            await ChangeTracingData(data, "TrcApplDtlUpdate");

            await DeleteFinancialDataForAppl(data);
            if ((data.YearsAndTaxForms is not null) && data.YearsAndTaxForms.Any())
                await CreateYearsAndTaxForms(data);
        }

        public async Task CreateESDCEventTraceData()
        {
            await MainDB.ExecProcAsync("CreateESDCEventTraceData");
        }

        private async Task ChangeTracingData(TracingApplicationData data, string procName)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                        {"Appl_CtrlCd", data.Appl_CtrlCd },
                        {"Trace_Child_Text", (object) data.Trace_Child_Text ?? DBNull.Value },
                        {"Trace_Breach_Text", (object) data.Trace_Breach_Text ?? DBNull.Value },
                        {"Trace_ReasGround_Text", (object) data.Trace_ReasGround_Text ?? DBNull.Value },
                        {"FamPro_Cd", data.FamPro_Cd },
                        {"Statute_Cd", string.IsNullOrEmpty(data.Statute_Cd) ? DBNull.Value : data.Statute_Cd},
                        {"Trace_Cycl_Qty", data.Trace_Cycl_Qty },
                        {"Trace_LstCyclStr_Dte", data.Trace_LstCyclStr_Dte == DateTime.MinValue ? DateTime.Now : data.Trace_LstCyclStr_Dte },
                        {"Trace_LstCyclCmp_Dte", data.Trace_LstCyclCmp_Dte.HasValue ? data.Trace_LstCyclCmp_Dte : DBNull.Value },
                        {"Trace_LiSt_Cd", data.Trace_LiSt_Cd },
                        {"S282_Ind",data.IncludeS282 },
                        {"S283_Ind",data.IncludeS283 },
                        {"Trace_ReasStepsParent_Text", (object) data.Trace_ReasStepsParent_Text ?? DBNull.Value },
                        {"Trace_ReasStepsChild_Text", (object) data.Trace_ReasStepsChild_Text ?? DBNull.Value },
                        {"Declaration_Ind", data.DeclarationIndicator },
                        {"Tracing_Information", data.TraceInformation },
                        {"Sin_Information", data.IncludeSinInformation },
                        {"Financial_Information", data.IncludeFinancialInformation }
                    };

            if (string.IsNullOrEmpty(data.InfoBank_Cd))
                parameters.Add("InfoBank_Cd", DBNull.Value);
            else
                parameters.Add("InfoBank_Cd", data.InfoBank_Cd);

            if (data.PhoneNumber is not null) parameters.Add("Phone_Number", data.PhoneNumber);
            if (data.EmailAddress is not null) parameters.Add("Email_Address", data.EmailAddress);

            _ = await MainDB.ExecProcAsync(procName, parameters);
        }

        public async Task<bool> TracingDataExists(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                        {"Appl_CtrlCd", appl_CtrlCd }
                    };

            int count = await MainDB.GetDataFromStoredProcAsync<int>("TrcApplRecordExistsInTrcTable", parameters);

            return count > 0;
        }

        public async Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavit()
        {
            var result = new DataList<TracingApplicationData>();

            var appDB = new DBApplication(MainDB);
            var data = await appDB.GetApplicationsWaitingForAffidavit("T01");

            string provinceCode = CurrentSubmitter[0..2];

            var tracingData = new List<TracingApplicationData>();
            foreach (var item in data.Items)
            {
                string firstTwoCharacters = item.Appl_EnfSrv_Cd[0..2].ToLower();

                if (firstTwoCharacters == provinceCode)
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

        public async Task<List<TraceToApplData>> GetTraceToApplData()
        {                                                       
            return await MainDB.GetDataFromStoredProcAsync<TraceToApplData>("MessageBrokerGetTRACEInboundToApplData", FillTraceToApplDataFromReader);
        }

        public async Task<List<TracingOutgoingFederalData>> GetFederalOutgoingData(int maxRecords,
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

            return await MainDB.GetRecordsFromStoredProcAsync<TracingOutgoingFederalData>("MessageBrokerGetTRCOUTOutboundData",
                                                                               parameters, FillTracingOutgoingFederalRecord);

        }

        private void FillTracingOutgoingFederalRecord(IDBHelperReader rdr, out TracingOutgoingFederalData data)
        {
            data = new TracingOutgoingFederalData(
                Event_dtl_Id: (int)rdr["Event_dtl_Id"],
                Event_Reas_Cd: (rdr["Event_Reas_Cd"] != null) ? (int)rdr["Event_Reas_Cd"] : default,
                Event_Reas_Text: (rdr["Event_Reas_Text"] != null) ? rdr["Event_Reas_Text"] as string : default,
                ActvSt_Cd: rdr["ActvSt_Cd"] as string,
                Recordtype: rdr["Recordtype"] as string,
                Appl_Dbtr_Cnfrmd_SIN: rdr["Val_1"] as string,
                Appl_EnfSrv_Cd: rdr["Val_2"] as string,
                Appl_CtrlCd: rdr["Val_3"] as string,
                ReturnType: (int)rdr["Val_4"]
            );
        }

        public async Task<TracingOutgoingProvincialData> GetProvincialOutgoingData(int maxRecords,
                                                                             string activeState,
                                                                             string recipientCode,
                                                                             bool isXML = true)
        {
            var paramTracing = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "chrRecptCd", recipientCode },
                { "isXML", isXML ? 1 : 0 }
            };

            var tracingData = await MainDB.GetDataFromStoredProcAsync<TracingOutgoingProvincialTracingData>("MessageBrokerGetTRCAPPOUTOutboundData",
                                                                                                            paramTracing, FillTracingOutgoingProvincialTracingData);

            var paramFinancials = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "chrRecptCd", recipientCode }
            };

            var financialData = await MainDB.GetDataFromStoredProcAsync<TracingOutgoingProvincialFinancialData>("TrcRspFin_SelectForOutboundData",
                                                                                                            paramFinancials, FillTracingOutgoingProvincialFinancialData);

            if (financialData is not null)
            {
                var dbTraceResponse = new DBTraceResponse(MainDB);
                foreach(var finData in financialData)
                {
                    var baseInfoList = await dbTraceResponse.GetActiveTraceResponseFinancialsForApplication(finData.Appl_EnfSrv_Cd, finData.Appl_CtrlCd);
                    foreach(var baseInfo in baseInfoList.Items)
                    {
                        int responseId = baseInfo.TrcRspFin_Id;
                        var detailsList = await dbTraceResponse.GetTraceResponseFinancialDetails(responseId);
                        finData.TraceFinancialDetails = detailsList.Items;

                        foreach(var details in finData.TraceFinancialDetails)
                        {
                            var values = await dbTraceResponse.GetTraceResponseFinancialDetailValues(details.TrcRspFin_Dtl_Id);
                            details.TraceDetailValues = values.Items;
                        }

                    }

                }
            }

            var data = new TracingOutgoingProvincialData
            {
                TracingData = tracingData,
                FinancialData = financialData
            };

            return data;
        }

        private async Task CreateYearsAndTaxForms(TracingApplicationData data)
        {
            if ((data.YearsAndTaxForms is not null) && data.YearsAndTaxForms.Any())
            {
                foreach (var yearData in data.YearsAndTaxForms)
                {
                    var parameters = new Dictionary<string, object> {
                        {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                        {"Appl_CtrlCd", data.Appl_CtrlCd },
                        {"FiscalYear",  yearData.Key}
                    };
                    int yearId = await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("TraceFin_Insert", parameters, "TraceFin_Id");

                    foreach (var taxForm in yearData.Value)
                    {
                        var taxFormParameters = new Dictionary<string, object> {
                            {"TraceFin_Id", yearId },
                            {"TaxForm",  taxForm}
                        };
                        _ = await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("TraceFin_Dtl_Insert", taxFormParameters, "TraceFin_Dtl_Id");
                    }
                }
            }
        }

        private async Task DeleteFinancialDataForAppl(TracingApplicationData data)
        {
            var parameters = new Dictionary<string, object> {
                    {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                    {"Appl_CtrlCd", data.Appl_CtrlCd }
                };

            var financialData = await MainDB.GetDataFromStoredProcAsync<TraceFinancialData>("TraceFin_SelectForAppl", parameters, FillTraceFinDataFromReader);

            if (financialData is not null)
            {
                foreach (var finData in financialData)
                {
                    var detailsParameters = new Dictionary<string, object> {
                            { "TraceFin_Id", finData.TraceFin_Id }
                        };

                    var detailsData = await MainDB.GetDataFromStoredProcAsync<TraceFinancialDetailData>("TraceFin_Dtl_SelectForTraceFin", detailsParameters, FillFinancialDetailDataFromReader);

                    foreach (var detail in detailsData)
                    {
                        var deleteDetailsParameters = new Dictionary<string, object>
                            {
                                { "TraceFin_Dtl_Id", detail.TraceFin_Dtl_Id }
                            };
                        await MainDB.ExecProcAsync("TraceFin_Dtl_Delete", deleteDetailsParameters);
                    }

                    await MainDB.ExecProcAsync("TraceFin_Delete", detailsParameters);
                }
            }
        }

        private void FillTracingOutgoingProvincialTracingData(IDBHelperReader rdr, TracingOutgoingProvincialTracingData data)
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
            switch (enfSrvCd?.Trim())
            {
                case "HR02": data.EnfSrv_Cd = "UI00"; break;
                case "EI02": data.EnfSrv_Cd = "EI00"; break;
                case "RC01": data.EnfSrv_Cd = "RC00"; break;
                default:
                    break;
            }
        }

        private void FillTracingOutgoingProvincialFinancialData(IDBHelperReader rdr, TracingOutgoingProvincialFinancialData data)
        {
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Recordtype = rdr["Recordtype"] as string;
            data.Appl_EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Appl_Source_RfrNr = rdr["Appl_Source_RfrNr"] as string;
            data.Subm_Recpt_SubmCd = rdr["Subm_Recpt_SubmCd"] as string;
            data.TrcRsp_Rcpt_Dte = (DateTime)rdr["TrcRsp_Rcpt_Dte"];
            data.TrcRsp_SeqNr = rdr["TrcRsp_SeqNr"] as string;
            data.TrcSt_Cd = rdr["TrcSt_Cd"] as string;
            data.Prcs_RecType = (int?)rdr["Prcs_RecType"] ?? 0;
            data.EnfSrv_Cd = "RC00";
        }

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
            try
            {
                data.Trace_Cycl_Qty = (int)rdr["Trace_Cycl_Qty"];
            }
            catch
            {
                data.Trace_Cycl_Qty = (short)rdr["Trace_Cycl_Qty"];
            }
        }

        private void FillDataFromReader(IDBHelperReader rdr, TracingApplicationData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Trace_Cycl_Qty = (int)rdr["Trace_Cycl_Qty"];
            data.Trace_LstCyclStr_Dte = (DateTime)rdr["Trace_LstCyclStr_Dte"];
            data.Trace_LstCyclCmp_Dte = rdr["Trace_LstCyclCmp_Dte"] as DateTime?; // can be null 
            data.Trace_LiSt_Cd = (short)rdr["Trace_LiSt_Cd"];

            if (rdr.ColumnExists("Trace_Child_Text")) data.Trace_Child_Text = rdr["Trace_Child_Text"] as string; // can be null 
            if (rdr.ColumnExists("Trace_Breach_Text")) data.Trace_Breach_Text = rdr["Trace_Breach_Text"] as string; // can be null 
            if (rdr.ColumnExists("Trace_ReasGround_Text")) data.Trace_ReasGround_Text = rdr["Trace_ReasGround_Text"] as string; // can be null 
            if (rdr.ColumnExists("FamPro_Cd")) data.FamPro_Cd = rdr["FamPro_Cd"] as string;
            if (rdr.ColumnExists("Statute_Cd")) data.Statute_Cd = rdr["Statute_Cd"] as string; // can be null 
            if (rdr.ColumnExists("InfoBank_Cd")) data.InfoBank_Cd = rdr["InfoBank_Cd"] as string; // can be null 

            if (rdr.ColumnExists("Phone_Number")) data.PhoneNumber = rdr["Phone_Number"] as string; // can be null 
            if (rdr.ColumnExists("Email_Address")) data.EmailAddress = rdr["Email_Address"] as string; // can be null 
            if (rdr.ColumnExists("S282_Ind")) data.IncludeS282 = (bool)rdr["S282_Ind"];
            if (rdr.ColumnExists("S283_Ind")) data.IncludeS283 = (bool)rdr["S283_Ind"];
            if (rdr.ColumnExists("Trace_ReasStepsParent_Text")) data.Trace_ReasStepsParent_Text = rdr["Trace_ReasStepsParent_Text"] as string;
            if (rdr.ColumnExists("Trace_ReasStepsChild_Text")) data.Trace_ReasStepsChild_Text = rdr["Trace_ReasStepsChild_Text"] as string;
            if (rdr.ColumnExists("Declaration_Ind")) data.DeclarationIndicator = (bool)rdr["Declaration_Ind"];

            if (rdr.ColumnExists("Tracing_Information")) data.TraceInformation = (short)rdr["Tracing_Information"];
            if (rdr.ColumnExists("Sin_Information")) data.IncludeSinInformation = (bool)rdr["Sin_Information"];
            if (rdr.ColumnExists("Financial_Information")) data.IncludeFinancialInformation = (bool)rdr["Financial_Information"];
        }

        private void FillTraceFinDataFromReader(IDBHelperReader rdr, TraceFinancialData data)
        {
            data.TraceFin_Id = (int)rdr["TraceFin_Id"];
            data.FiscalYear = (short)rdr["FiscalYear"];
        }

        private void FillFinancialDetailDataFromReader(IDBHelperReader rdr, TraceFinancialDetailData data)
        {
            data.TraceFin_Dtl_Id = (int)rdr["TraceFin_Dtl_Id"];
            data.TraceFin_Id = (int)rdr["TraceFin_Id"];
            data.TaxForm = rdr["TaxForm"] as string;
        }

    }
}
