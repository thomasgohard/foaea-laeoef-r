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

namespace Outgoing.FileCreator.Fed.LicenceDenial
{
    class Program
    {
        static async Task Main(string[] args)
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
            var fileBrokerDB = new DBToolsAsync(fileBrokerConnectionString);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            await CreateOutgoingFederalLicenceDenialFilesAsync(fileBrokerDB, apiRootForFiles, configuration);

            ColourConsole.Write("Completed.");

        }

        private static async Task CreateOutgoingFederalLicenceDenialFilesAsync(DBToolsAsync fileBrokerDB, ApiConfig apiRootForFiles,
                                                                               IConfiguration config)
        {
            var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var licenceDenialApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaLicenceDenialRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            // TODO: fix token
            string token = "";
            var apiBrokers = new APIBrokerList
            {
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper, token),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(licenceDenialApiHelper, token),
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

            var federalFileManager = new OutgoingFederalLicenceDenialManager(apiBrokers, repositories, config);

            var federalLicenceDenialOutgoingSources = (await repositories.FileTable.GetFileTableDataForCategoryAsync("LICOUT"))
                                                                  .Where(s => s.Active == true);

            foreach (var federalLicenceDenialOutgoingSource in federalLicenceDenialOutgoingSources)
            {
                var errors = new List<string>();
                string filePath = await federalFileManager.CreateOutputFileAsync(federalLicenceDenialOutgoingSource.Name,
                                                                                 errors);
                if (errors.Count == 0)
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    foreach (var error in errors)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalLicenceDenialOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                        await repositories.ErrorTrackingTable.MessageBrokerErrorAsync("LICOUT", federalLicenceDenialOutgoingSource.Name, 
                                                                                   new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
