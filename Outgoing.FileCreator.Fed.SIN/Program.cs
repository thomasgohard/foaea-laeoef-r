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

namespace Outgoing.FileCreator.Fed.SIN;

internal class Program
{
    static async Task Main(string[] args)
    {
        ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing SIN File Creator");

        string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        string fileBrokerConnectionString = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();
        var fileBrokerDB = new DBToolsAsync(fileBrokerConnectionString);
        var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

        await CreateOutgoingFederalSinFileAsync(fileBrokerDB, apiRootForFiles);

        ColourConsole.Write("Completed.\n");
    }

    private static async Task CreateOutgoingFederalSinFileAsync(DBToolsAsync fileBrokerDB, ApiConfig apiRootForFiles)
    {
        var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

        // TODO: fix token
        string token = "";
        var apiBrokers = new APIBrokerList
        {
            ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper, token),
            Sins = new SinAPIBroker(applicationApiHelper, token)
        };

        var repositories = new RepositoryList
        {
            FileTable = new DBFileTable(fileBrokerDB),
            FlatFileSpecs = new DBFlatFileSpecification(fileBrokerDB),
            OutboundAuditTable = new DBOutboundAudit(fileBrokerDB),
            ErrorTrackingTable = new DBErrorTracking(fileBrokerDB),
            ProcessParameterTable = new DBProcessParameter(fileBrokerDB) 
        };

        var federalFileManager = new OutgoingFederalSinManager(apiBrokers, repositories);

        var federalSinOutgoingSources = (await repositories.FileTable.GetFileTableDataForCategoryAsync("SINOUT"))
                                          .Where(s => s.Active == true);

        foreach (var federalSinOutgoingSource in federalSinOutgoingSources)
        {
            var errors = new List<string>();
            string filePath = await federalFileManager.CreateOutputFileAsync(federalSinOutgoingSource.Name, errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalSinOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                    await repositories.ErrorTrackingTable.MessageBrokerErrorAsync("SINOUT", federalSinOutgoingSource.Name, 
                                                                               new Exception(error), displayExceptionError: true);
                }
        }

    }
}
