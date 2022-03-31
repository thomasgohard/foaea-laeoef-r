using FileBroker.Data;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileBroker.Business
{
    public class OutgoingFederalSinManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList Repositories { get; }

        public OutgoingFederalSinManager(APIBrokerList apiBrokers, RepositoryList repositories)
        {
            APIs = apiBrokers;
            Repositories = repositories;
        }

        public string CreateOutputFile(string fileBaseName, out List<string> errors)
        {
            errors = new List<string>();

            string newFilePath = string.Empty;
            bool fileCreated = false;

            var fileTableData = Repositories.FileTable.GetFileTableDataForFileName(fileBaseName);

            try
            {
                var processCodes = Repositories.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);

                int cycleLength = 3;
                string newCycle = fileTableData.Cycle.ToString(new string('0', cycleLength));

                newFilePath = fileTableData.Path + fileBaseName + "." + newCycle;
                if (File.Exists(newFilePath))
                {
                    errors.Add("** Error: File Already Exists");
                    return "";
                }

                var data = GetOutgoingData(fileTableData, processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                           processCodes.EnfSrv_Cd);

                var eventIds = new List<int>();
                string fileContent = GenerateOutputFileContentFromData(data, newCycle, ref eventIds);

                File.WriteAllText(newFilePath, fileContent);
                fileCreated = true;

                Repositories.OutboundAuditDB.InsertIntoOutboundAudit(newFilePath, DateTime.Now, fileCreated,
                                                                     "Outbound File created successfully.");

                Repositories.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                APIs.ApplicationEventAPIBroker.UpdateOutboundEventDetail(processCodes.ActvSt_Cd, processCodes.AppLiSt_Cd,
                                                                         processCodes.EnfSrv_Cd,
                                                                         "OK: Written to " + newFilePath, eventIds);

                return newFilePath;

            }
            catch (Exception e)
            {
                string error = "Error Creating Outbound Data File: " + e.Message;
                errors.Add(error);

                Repositories.OutboundAuditDB.InsertIntoOutboundAudit(newFilePath, DateTime.Now, fileCreated, error);

                Repositories.ErrorTrackingDB.MessageBrokerError($"File Error: {fileTableData.PrcId} {fileBaseName}", "Error creating outbound file", e, true);

                return string.Empty;
            }
        }

        private List<SINOutgoingFederalData> GetOutgoingData(FileTableData fileTableData, string actvSt_Cd,
                                                             int appLiSt_Cd, string enfSrvCode)
        {
            var recMax = Repositories.ProcessParameterTable.GetValueForParameter(fileTableData.PrcId, "rec_max");
            int maxRecords = string.IsNullOrEmpty(recMax) ? 0 : int.Parse(recMax);

            var data = APIs.SinAPIBroker.GetOutgoingFederalSins(maxRecords, actvSt_Cd, appLiSt_Cd, enfSrvCode);
            return data;
        }

        private static string GenerateOutputFileContentFromData(List<SINOutgoingFederalData> data,
                                                                string newCycle, ref List<int> eventIds)
        {
            var result = new StringBuilder();

            result.AppendLine(GenerateHeaderLine(newCycle));
            foreach (var item in data)
            {
                result.AppendLine(GenerateDetailLine(item));
                eventIds.Add(item.Event_dtl_Id);
            }
            result.AppendLine(GenerateFooterLine(data.Count));

            return result.ToString();
        }

        private static string GenerateHeaderLine(string newCycle)
        {
            string julianDate = DateTime.Now.AsJulianString();

            return $"01{newCycle}{julianDate}";
        }

        private static string GenerateDetailLine(SINOutgoingFederalData item)
        {
            string result = $"02{item.Appl_EnfSrv_Cd:6}{item.Appl_CtrlCd:6}{item.Appl_Dbtr_Entrd_SIN:9}" +
                            $"{item.Appl_Dbtr_FrstNme:15}{item.Appl_Dbtr_MddleNme:15}{item.Appl_Dbtr_SurNme:25}" +
                            $"{item.Appl_Dbtr_Parent_SurNme:25}{item.Appl_Dbtr_Gendr_Cd:1}{item.Appl_Dbtr_Brth_Dte:8}";

            return result;
        }

        private static string GenerateFooterLine(int rowCount)
        {
            return $"99{rowCount:000000}";
        }
    }
}
