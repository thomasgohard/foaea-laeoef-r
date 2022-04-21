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

namespace Outgoing.FileCreator.MEP
{
    class Program
    {
        static void Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting MEP Outgoing File Creator");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var fileBrokerDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            CreateOutgoingProvincialTracingFiles(fileBrokerDB, apiRootForFiles);
            CreateOutgoingProvincialLicenceDenialFiles(fileBrokerDB, apiRootForFiles);
            // CreateOutgoingProvincialInterceptionFiles(fileBrokerDB, apiRootForFiles);
            // CreateOutgoingProvincialStatusFiles(fileBrokerDB, apiRootForFiles);
        }

        private static void CreateOutgoingProvincialTracingFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        {
            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI)),
                TracingApplications = new TracingApplicationAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
                TracingResponses = new TraceResponseAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
                TracingEvents = new TracingEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
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

            var federalFileManager = new OutgoingProvincialTracingManager(apiBrokers, repositories);

            var provincialTraceOutgoingSources = repositories.FileTable.GetFileTableDataForCategory("TRCAPPOUT")
                                                .Where(s => s.Active == true);

            foreach (var provincialTraceOutgoingSource in provincialTraceOutgoingSources)
            {
                string filePath = federalFileManager.CreateOutputFile(provincialTraceOutgoingSource.Name, out List<string> errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{provincialTraceOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        repositories.ErrorTrackingDB.MessageBrokerError("TRCAPPOUT", provincialTraceOutgoingSource.Name, new Exception(error), false);
                    }

            }

        }

        private static void CreateOutgoingProvincialLicenceDenialFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        {
            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI)),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI)),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI)),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI)),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI)),
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

            var federalFileManager = new OutgoingProvincialTracingManager(apiBrokers, repositories);

            var provincialTraceOutgoingSources = repositories.FileTable.GetFileTableDataForCategory("TRCAPPOUT")
                                                .Where(s => s.Active == true);

            foreach (var provincialTraceOutgoingSource in provincialTraceOutgoingSources)
            {
                string filePath = federalFileManager.CreateOutputFile(provincialTraceOutgoingSource.Name, out List<string> errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{provincialTraceOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        repositories.ErrorTrackingDB.MessageBrokerError("TRCAPPOUT", provincialTraceOutgoingSource.Name, new Exception(error), false);
                    }
            }

        }

    }
}
