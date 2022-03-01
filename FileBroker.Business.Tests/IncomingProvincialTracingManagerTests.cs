using DBHelper;
using FileBroker.Data;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Resources.Helpers;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using Xunit;

namespace FileBroker.Business.Tests
{
    public class IncomingProvincialTracingManagerTests
    {

        private string sourceTracingData;
        private IncomingProvincialTracingManager tracingManager;
        private InMemoryTracingApplicationAPIBroker tracingApplicationAPIs;
        private InMemoryFileAudit fileAuditDB;
        private InMemoryFileTable fileTableDB;

        private void SetupTestAndLoadFile(string fileName)
        {
            tracingApplicationAPIs = new InMemoryTracingApplicationAPIBroker();
            fileAuditDB = new InMemoryFileAudit();
            fileTableDB = new InMemoryFileTable();
            var auditConfig = new ProvincialAuditFileConfig
            {
                AuditRootPath = @"C:\Audit",
                AuditRecipients = "dsarrazi@justice.gc.ca",
                FrenchAuditProvinceCodes = new string[] { "QC" }
            };

            var apis = new APIBrokerList
            {
                TracingApplicationAPIBroker = tracingApplicationAPIs
            };

            var fileBrokerDB = new DBTools("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                           .ReplaceVariablesWithEnvironmentValues());

            var repositories = new RepositoryList
            {
                FileAudit = fileAuditDB,
                FileTable = fileTableDB,
                MailServiceDB = new DBMailService(fileBrokerDB)
            };

            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
            tracingManager = new IncomingProvincialTracingManager(fileNameNoExt, apis, repositories, auditConfig);

            var doc = new XmlDocument();
            doc.Load(@$"TestDataFiles\{fileName}");

            sourceTracingData = JsonConvert.SerializeXmlNode(doc); // convert xml to json
        }

        [Fact]
        public void Ontario_TwoRequests_GoodData()
        {
            // Arrange
            SetupTestAndLoadFile("ON3D01IT.000001.xml");

            // Act
            var messages = tracingManager.ExtractAndProcessRequestsInFile(sourceTracingData, includeInfoInMessages: true);

            // Assert
            Assert.Equal("Success", fileAuditDB.FileAuditTable[0].ApplicationMessage);
            Assert.Equal("Success", fileAuditDB.FileAuditTable[1].ApplicationMessage);
            Assert.Equal("P00002", messages[1].Description);
        }

        [Fact]
        public void Ontario_OneRequest_BadMaintenanceActionValue()
        {
            // Arrange
            SetupTestAndLoadFile("ON3D01IT.000002.xml");

            // Act
            tracingManager.ExtractAndProcessRequestsInFile(sourceTracingData);

            // Assert
            Assert.Equal("Invalid MaintenanceAction [Z] and MaintenanceLifeState [00] combination.", fileAuditDB.FileAuditTable[0].ApplicationMessage);

        }

    }
}
