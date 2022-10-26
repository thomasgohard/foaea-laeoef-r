using DBHelper;
using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Data.DB;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Outgoing.FileCreator.MEP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting MEP Outgoing File Creator");

            string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var fileBrokerDB = new DBToolsAsync(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            bool generateTracingFiles = true;
            bool generateLicencingFiles = true;
            bool generateStatsFiles = true;

            if (args.Length > 0)
            {
                string option = args[0].ToUpper();
                switch (option)
                {
                    case "TRACE_ONLY":
                        generateLicencingFiles = false;
                        generateStatsFiles = false;
                        break;
                    case "LICENCE_ONLY":
                        generateTracingFiles = false;
                        generateStatsFiles = false;
                        break;
                    case "STATS_ONLY":
                        generateLicencingFiles = false;
                        generateTracingFiles = false;
                        break;
                    default:
                        break;
                }
            }

            var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var tracingApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var licenceDenialApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            string token = "";
            var apiBrokers = new APIBrokerList
            {
                Applications = new ApplicationAPIBroker(applicationApiHelper, token),
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper, token),
                TracingApplications = new TracingApplicationAPIBroker(tracingApiHelper, token),
                TracingResponses = new TraceResponseAPIBroker(tracingApiHelper, token),
                TracingEvents = new TracingEventAPIBroker(tracingApiHelper, token),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(licenceDenialApiHelper, token),
            };

            var repositories = new RepositoryList
            {
                FileTable = new DBFileTable(fileBrokerDB),
                FlatFileSpecs = new DBFlatFileSpecification(fileBrokerDB),
                OutboundAuditTable = new DBOutboundAudit(fileBrokerDB),
                ErrorTrackingTable = new DBErrorTracking(fileBrokerDB),
                ProcessParameterTable = new DBProcessParameter(fileBrokerDB),
                MailService = new DBMailService(fileBrokerDB)
            };

            if (generateTracingFiles)
                await CreateOutgoingProvincialFiles(repositories, "TRCAPPOUT", new OutgoingProvincialTracingManager(apiBrokers, repositories, configuration));

            if (generateLicencingFiles)
                await CreateOutgoingProvincialFiles(repositories, "LICAPPOUT", new OutgoingProvincialLicenceDenialManager(apiBrokers, repositories, configuration));

            if (generateStatsFiles)
                await CreateOutgoingProvincialFiles(repositories, "STATAPPOUT", new OutgoingProvincialStatusManager(apiBrokers, repositories, configuration));
        }

        private static async Task CreateOutgoingProvincialFiles(RepositoryList repositories, string category,
                                                                IOutgoingFileManager outgoingProvincialFileManager)
        {
            var provincialOutgoingSources = (await repositories.FileTable.GetFileTableDataForCategoryAsync(category))
                                                                  .Where(s => s.Active == true);

            foreach (var provincialOutgoingSource in provincialOutgoingSources)
            {
                var errors = new List<string>();
                string filePath = await outgoingProvincialFileManager.CreateOutputFileAsync(provincialOutgoingSource.Name, errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{provincialOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        await repositories.ErrorTrackingTable.MessageBrokerErrorAsync(category, provincialOutgoingSource.Name, 
                                                                        new Exception(error), displayExceptionError: true);
                    }
            }
        }
    }
}
