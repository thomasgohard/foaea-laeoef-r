using DBHelper;
using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Data.DB;
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

namespace Outgoing.FileCreator.Fed.Tracing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Tracing Files Creator");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            string fileBrokerConnectionString = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();
            var fileBrokerDB = new DBToolsAsync(fileBrokerConnectionString);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            await CreateOutgoingFederalTracingFiles(fileBrokerDB, apiRootForFiles, configuration);

            ColourConsole.Write("Completed.");

        }

        private static async Task CreateOutgoingFederalTracingFiles(DBToolsAsync fileBrokerDB, ApiConfig apiRootForFiles,
                                                                    IConfiguration config)
        {
            var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var tracingApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);

            // TODO: fix token
            string token = "";
            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper, token),
                TracingApplications = new TracingApplicationAPIBroker(tracingApiHelper, token),
                TracingResponses = new TraceResponseAPIBroker(tracingApiHelper, token),
                TracingEvents = new TracingEventAPIBroker(tracingApiHelper, token),
                Sins = new SinAPIBroker(applicationApiHelper, token)
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

            var federalFileManager = new OutgoingFederalTracingManager(apiBrokers, repositories, config);

            var federalTraceOutgoingSources = (await repositories.FileTable.GetFileTableDataForCategoryAsync("TRCOUT"))
                                                .Where(s => s.Active == true);

            foreach (var federalTraceOutgoingSource in federalTraceOutgoingSources)
            {
                var errors = new List<string>();
                string filePath = await federalFileManager.CreateOutputFileAsync(federalTraceOutgoingSource.Name, errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalTraceOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        await repositories.ErrorTrackingTable.MessageBrokerErrorAsync("TRCOUT", federalTraceOutgoingSource.Name, 
                                                                                   new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
