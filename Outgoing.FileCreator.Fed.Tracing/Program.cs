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

namespace Outgoing.FileCreator.Fed.Tracing
{
    class Program
    {
        static void Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Tracing Files Creator");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            string fileBrokerConnectionString = configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues();
            var fileBrokerDB = new DBTools(fileBrokerConnectionString);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            CreateOutgoingFederalTracingFiles(fileBrokerDB, apiRootForFiles);

            ColourConsole.Write("Completed.");

        }

        private static void CreateOutgoingFederalTracingFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        {
            var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var tracingApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper),
                TracingApplications = new TracingApplicationAPIBroker(tracingApiHelper),
                TracingResponses = new TraceResponseAPIBroker(tracingApiHelper),
                TracingEvents = new TracingEventAPIBroker(tracingApiHelper),
                Sins = new SinAPIBroker(applicationApiHelper)
            };

            var repositories = new RepositoryList
            {
                FileTable = new DBFileTable(fileBrokerDB),
                FlatFileSpecs = new DBFlatFileSpecification(fileBrokerDB),
                OutboundAuditDB = new DBOutboundAudit(fileBrokerDB),
                ErrorTrackingDB = new DBErrorTracking(fileBrokerDB),
                ProcessParameterTable = new DBProcessParameter(fileBrokerDB),
                MailServiceDB = new DBMailService(fileBrokerDB)
            };

            var federalFileManager = new OutgoingFederalTracingManager(apiBrokers, repositories);

            var federalTraceOutgoingSources = repositories.FileTable.GetFileTableDataForCategory("TRCOUT")
                                                .Where(s => s.Active == true);

            foreach (var federalTraceOutgoingSource in federalTraceOutgoingSources)
            {
                string filePath = federalFileManager.CreateOutputFile(federalTraceOutgoingSource.Name,
                                                                      out List<string> errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalTraceOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        repositories.ErrorTrackingDB.MessageBrokerError("TRCOUT", federalTraceOutgoingSource.Name, 
                                                                        new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
