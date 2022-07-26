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

namespace Outgoing.FileCreator.Fed.LicenceDenial
{
    class Program
    {
        static void Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Licence Denial Files Creator");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            string fileBrokerConnectionString = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();
            var fileBrokerDB = new DBTools(fileBrokerConnectionString);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            CreateOutgoingFederalLicenceDenialFiles(fileBrokerDB, apiRootForFiles);

            ColourConsole.Write("Completed.");

        }

        private static void CreateOutgoingFederalLicenceDenialFiles(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
        {
            var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var licenceDenialApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(licenceDenialApiHelper),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(licenceDenialApiHelper),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(licenceDenialApiHelper),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(licenceDenialApiHelper),
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

            var federalFileManager = new OutgoingFederalLicenceDenialManager(apiBrokers, repositories);

            var federalLicenceDenialOutgoingSources = repositories.FileTable.GetFileTableDataForCategory("LICOUT")
                                                                  .Where(s => s.Active == true);

            foreach (var federalLicenceDenialOutgoingSource in federalLicenceDenialOutgoingSources)
            {
                string filePath = federalFileManager.CreateOutputFile(federalLicenceDenialOutgoingSource.Name,
                                                                      out List<string> errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalLicenceDenialOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        repositories.ErrorTrackingDB.MessageBrokerError("LICOUT", federalLicenceDenialOutgoingSource.Name, 
                                                                        new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
