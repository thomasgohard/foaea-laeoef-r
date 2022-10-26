using DBHelper;
using FileBroker.Business.Tests.InMemory;
using FileBroker.Data;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

            var myConfiguration = new Dictionary<string, string>
                {
                    {"FOAEA:userName", "%FILEBROKER_FOAEA_USERNAME%".ReplaceVariablesWithEnvironmentValues()},
                    {"FOAEA:userPassword", "%FILEBROKER_FOAEA_USERPASSWORD%".ReplaceVariablesWithEnvironmentValues()},
                    {"FOAEA:submitter", "%FILEBROKER_FOAEA_SUBMITTER%".ReplaceVariablesWithEnvironmentValues()}
                };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build(); 

            var auditConfig = new ProvincialAuditFileConfig
            {
                AuditRootPath = @"C:\Audit",
                AuditRecipients = "dsarrazi@justice.gc.ca",
                FrenchAuditProvinceCodes = new string[] { "QC" }
            };

            var apis = new APIBrokerList
            {
                TracingApplications = tracingApplicationAPIs
            };

            string connection = "Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;";
            var fileBrokerDB = new DBToolsAsync(connection.ReplaceVariablesWithEnvironmentValues());

            var repositories = new RepositoryList
            {
                FileAudit = fileAuditDB,
                FileTable = fileTableDB,
                MailService = new DBMailService(fileBrokerDB)
            };

            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
            tracingManager = new IncomingProvincialTracingManager(fileNameNoExt, apis, repositories, auditConfig,
                                                                  config);

            var doc = new XmlDocument();
            doc.Load(@$"TestDataFiles\{fileName}");

            sourceTracingData = JsonConvert.SerializeXmlNode(doc); // convert xml to json
        }

        [Fact]
        public async Task Ontario_TwoRequests_GoodData()
        {
            // Arrange
            SetupTestAndLoadFile("ON3D01IT.000001.xml");
            var unknownTags = new List<UnknownTag>();

            // Act
            _ = await tracingManager.ExtractAndProcessRequestsInFileAsync(sourceTracingData, unknownTags, includeInfoInMessages: true);

            // Assert
            Assert.Equal("Success", fileAuditDB.FileAuditTable[0].ApplicationMessage);
            Assert.Equal("Success", fileAuditDB.FileAuditTable[1].ApplicationMessage);
            // Assert.Equal("P00002", messages[1].Description);
        }

        [Fact]
        public async Task Ontario_OneRequest_BadMaintenanceActionValue()
        {
            // Arrange
            SetupTestAndLoadFile("ON3D01IT.000002.xml");
            var unknownTags = new List<UnknownTag>();

            // Act
            await tracingManager.ExtractAndProcessRequestsInFileAsync(sourceTracingData, unknownTags);

            // Assert
            Assert.Equal("Invalid MaintenanceAction [Z] and MaintenanceLifeState [00] combination.", fileAuditDB.FileAuditTable[0].ApplicationMessage);

        }

    }
}
