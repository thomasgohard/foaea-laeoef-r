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

namespace Outgoing.FileCreator.Fed.SIN;

internal class Program
{
    static void Main(string[] args)
    {
        ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing SIN File Creator");

        string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        string fileBrokerConnectionString = configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues();
        var fileBrokerDB = new DBTools(fileBrokerConnectionString);
        var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

        CreateOutgoingFederalSinFile(fileBrokerDB, apiRootForFiles);

        ColourConsole.Write("Completed.\n");
    }

    private static void CreateOutgoingFederalSinFile(DBTools fileBrokerDB, ApiConfig apiRootForFiles)
    {
        var applicationApiHelper = new APIBrokerHelper(apiRootForFiles.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

        var apiBrokers = new APIBrokerList
        {
            ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper),
            Sins = new SinAPIBroker(applicationApiHelper)
        };

        var repositories = new RepositoryList
        {
            FileTable = new DBFileTable(fileBrokerDB),
            FlatFileSpecs = new DBFlatFileSpecification(fileBrokerDB),
            OutboundAuditDB = new DBOutboundAudit(fileBrokerDB),
            ErrorTrackingDB = new DBErrorTracking(fileBrokerDB),
            ProcessParameterTable = new DBProcessParameter(fileBrokerDB) 
        };

        var federalFileManager = new OutgoingFederalSinManager(apiBrokers, repositories);

        var federalSinOutgoingSources = repositories.FileTable.GetFileTableDataForCategory("SINOUT")
                                          .Where(s => s.Active == true);

        foreach (var federalSinOutgoingSource in federalSinOutgoingSources)
        {
            string filePath = federalFileManager.CreateOutputFile(federalSinOutgoingSource.Name,
                                                                  out List<string> errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalSinOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                    repositories.ErrorTrackingDB.MessageBrokerError("SINOUT", federalSinOutgoingSource.Name, 
                                                                    new Exception(error), displayExceptionError: true);
                }
        }

    }
}
