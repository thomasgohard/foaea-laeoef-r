using DBHelper;
using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Data.DB;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
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
            //CreateOutgoingProvincialLicencingFiles(fileBrokerDB, apiRootForFiles);
            //CreateOutgoingProvincialStatusFiles(fileBrokerDB, apiRootForFiles);
        }

        //private static void CreateOutgoingProvincialStatusFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        //{
        //    throw new NotImplementedException();
        //}

        //private static void CreateOutgoingProvincialLicencingFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        //{
        //    throw new NotImplementedException();
        //}

        private static void CreateOutgoingProvincialTracingFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        {
            var apiBrokers = new APIBrokerList
            {
                ApplicationEventAPIBroker = new ApplicationEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI)),
                TracingApplicationAPIBroker = new TracingApplicationAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
                TraceResponseAPIBroker = new TraceResponseAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
                TracingEventAPIBroker = new TracingEventAPIBroker(new APIBrokerHelper(apiRootForFiles.FoaeaTracingRootAPI)),
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

            var allErrors = new Dictionary<string, List<string>>();
            foreach (var provincialTraceOutgoingSource in provincialTraceOutgoingSources)
            {
                string filePath = federalFileManager.CreateOutputFile(provincialTraceOutgoingSource.Name, out List<string> errors);
                allErrors.Add(provincialTraceOutgoingSource.Name, errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            }

            foreach (var error in allErrors)
                ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{error.Key}[/cyan]: [red]{error.Value}[/red]");
        }
    }
}
