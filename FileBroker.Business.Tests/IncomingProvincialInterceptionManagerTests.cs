using FileBroker.Business.Tests.InMemory;
using FileBroker.Data;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FileBroker.Business.Tests
{
    public class IncomingProvincialInterceptionManagerTests
    {
        
        [Fact]
        public async Task GetAndValidateAppDataFromRequest_Test1()
        {
            // Arrange
            string enfService = "ON01";
            string controlCode = "A123";
            string submitterCode = "TEST";

            var (coreData, interceptionData, financialData, sourceSpecificData) = SetupGoodData(enfService, controlCode, submitterCode);

            var fileAuditData = new FileAuditData
            {
                Appl_EnfSrv_Cd = enfService,
                Appl_CtrlCd = controlCode,
                Appl_Source_RfrNr = "",
                ApplicationMessage = "",
                InboundFilename = "",
                Timestamp = DateTime.Now,
                IsCompleted = false,
            };
            int errorCount = 0;

            string fileName = "";

            var interceptionApplications = new InMemoryInterceptionApplicationAPIBroker();
            var applicationEvents = new InMemoryApplicationEventAPIBroker();
            var applications = new InMemoryApplicationAPIBroker();

            var apis = new APIBrokerList
            {
                InterceptionApplications = interceptionApplications,
                ApplicationEvents = applicationEvents,
                Applications = applications
            };

            var repositories = new RepositoryList
            {
                FlatFileSpecs = null,
                FileTable = null,
                FileAudit = null,
                ProcessParameterTable = null,
                OutboundAuditTable = null,
                ErrorTrackingTable = null,
                MailService = null,
                TranslationTable = null,
                RequestLogTable = null
            };

            var auditConfig = new ProvincialAuditFileConfig
            {
                AuditRootPath = null,
                AuditRecipients = null,
                FrenchAuditProvinceCodes = null
            };

            var interceptionManager = new IncomingProvincialInterceptionManager(fileName,
                                                                                apis,
                                                                                repositories,
                                                                                auditConfig);

            // Act
            InterceptionApplicationData applicationData;
            bool isValidData;
            int thisErrorCount = 0;

            (applicationData, thisErrorCount, isValidData) = await interceptionManager.GetAndValidateAppDataFromRequestAsync(coreData,
                                                                                       interceptionData,
                                                                                       financialData,
                                                                                       sourceSpecificData,
                                                                                       fileAuditData);
            errorCount += thisErrorCount;

            // Assert
            Assert.True(isValidData);
            Assert.Equal(0, errorCount);
            Assert.Equal(String.Empty, fileAuditData.ApplicationMessage);
            Assert.Equal(controlCode, applicationData.Appl_CtrlCd);
        }

        private static (MEPInterception_RecType10 coreData,
                        MEPInterception_RecType11 interceptionData,
                        MEPInterception_RecType12 financialData,
                        List<MEPInterception_RecType13> sourceSpecificData) SetupGoodData(string enfSrv, string controlCode,
                                                                                          string submitterCode)
        {
            var coreData = new MEPInterception_RecType10
            {
                RecType = "10",
                dat_Subm_SubmCd = submitterCode,
                dat_Appl_CtrlCd = controlCode,
                dat_Appl_Source_RfrNr = "",
                dat_Appl_EnfSrvCd = enfSrv,
                dat_Subm_Rcpt_SubmCd = "",
                dat_Appl_Lgl_Dte = DateTime.Now.ToString(),
                dat_Appl_Dbtr_SurNme = "",
                dat_Appl_Dbtr_FrstNme = "",
                dat_Appl_Dbtr_MddleNme = "",
                dat_Appl_Dbtr_Brth_Dte = DateTime.Now.ToString(),
                dat_Appl_Dbtr_Gendr_Cd = "",
                dat_Appl_Dbtr_Entrd_SIN = "",
                dat_Appl_Dbtr_Parent_SurNme_Birth = "",
                dat_Appl_CommSubm_Text = "",
                dat_Appl_Rcptfrm_dte = DateTime.Now.ToString(),
                dat_Appl_AppCtgy_Cd = "",
                dat_Appl_Group_Batch_Cd = "",
                dat_Appl_Medium_Cd = "",
                dat_Appl_Affdvt_Doc_TypCd = "",
                dat_Appl_Reas_Cd = "",
                dat_Appl_Reactv_Dte = null,
                dat_Appl_LiSt_Cd = "",
                Maintenance_ActionCd = "",
                dat_New_Owner_RcptSubmCd = "",
                dat_New_Owner_SubmCd = "",
                dat_Update_SubmCd = "",
            };
            var interceptionData = new MEPInterception_RecType11
            {
                RecType = "11",
                dat_Subm_SubmCd = submitterCode,
                dat_Appl_CtrlCd = controlCode,
                dat_Appl_Dbtr_LngCd = "",
                dat_Appl_Dbtr_Addr_Ln = "",
                dat_Appl_Dbtr_Addr_Ln1 = "",
                dat_Appl_Dbtr_Addr_CityNme = "",
                dat_Appl_Dbtr_Addr_CtryCd = "",
                dat_Appl_Dbtr_Addr_PCd = "",
                dat_Appl_Dbtr_Addr_PrvCd = "",
                dat_Appl_Crdtr_SurNme = "",
                dat_Appl_Crdtr_FrstNme = "",
                dat_Appl_Crdtr_MddleNme = "",
                dat_Appl_Crdtr_Brth_Dte = DateTime.Now.ToString(),
            };
            var financialData = new MEPInterception_RecType12
            {
                RecType = "12",
                dat_Subm_SubmCd = submitterCode,
                dat_Appl_CtrlCd = controlCode,
                dat_IntFinH_LmpSum_Money = "",
                dat_IntFinH_Perpym_Money = "",
                dat_PymPr_Cd = "",
                dat_IntFinH_CmlPrPym_Ind = "",
                dat_IntFinH_NextRecalc_Dte = "",
                dat_HldbCtg_Cd = "",
                dat_IntFinH_DfHldbPrcnt = "",
                dat_IntFinH_DefHldbAmn_Money = "",
                dat_IntFinH_DefHldbAmn_Period = "",
                dat_IntFinH_VarIss_Dte = DateTime.Now.ToString(),
            };
            var sourceSpecificData = new List<MEPInterception_RecType13>
            {
                new MEPInterception_RecType13{
                    RecType= "13",
                    dat_Subm_SubmCd= submitterCode,
                    dat_Appl_CtrlCd= controlCode,
                    dat_EnfSrv_Cd= "",
                    dat_HldbCtg_Cd= "",
                    dat_HldbCnd_SrcHldbPrcnt= "",
                    dat_HldbCnd_Hldb_Amn_Money= "",
                    dat_HldbCnd_MxmPerChq_Money= "",
                },
                new MEPInterception_RecType13{
                    RecType= "13",
                    dat_Subm_SubmCd= submitterCode,
                    dat_Appl_CtrlCd= controlCode,
                    dat_EnfSrv_Cd= "",
                    dat_HldbCtg_Cd= "",
                    dat_HldbCnd_SrcHldbPrcnt= "",
                    dat_HldbCnd_Hldb_Amn_Money= "",
                    dat_HldbCnd_MxmPerChq_Money= "",
                }
            };

            return (coreData, interceptionData, financialData, sourceSpecificData);
        }

    }
}
