using DBHelper;
using FileBroker.Business;
using FileBroker.Common;
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

        await CreateOutgoingFederalSinFileAsync(fileBrokerDB, apiRootForFiles, configuration);

        ColourConsole.Write("Completed.\n");
    }

    private static async Task CreateOutgoingFederalSinFileAsync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                                IConfiguration config)
    {
        var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

        var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

        var federalFileManager = new OutgoingFederalSinManager(foaeaApis, db, config);

        var federalSinOutgoingSources = (await db.FileTable.GetFileTableDataForCategoryAsync("SINOUT"))
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
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync("SINOUT", federalSinOutgoingSource.Name, 
                                                                               new Exception(error), displayExceptionError: true);
                }
        }

    }
}
