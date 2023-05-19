using DBHelper;
using FileBroker.Business;
using FileBroker.Common;
using FileBroker.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Outgoing.FileCreator.Fed.Tracing;

public static class OutgoingFileCreatorFedTracing
{
    public static async Task Run(string[] args = null)
    {
        args ??= Array.Empty<string>();

        ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Tracing Files Creator");

        var config = new FileBrokerConfigurationHelper(args);

        var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

        await CreateOutgoingFederalTracingFiles(fileBrokerDB, config.ApiRootData, config);

        ColourConsole.Write("Completed.");
    }

    private static async Task CreateOutgoingFederalTracingFiles(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                                IFileBrokerConfigurationHelper config)
    {

        var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);
        var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

        var federalFileManager = new OutgoingFederalTracingManager(foaeaApis, db, config);

        var federalTraceOutgoingSources = (await db.FileTable.GetFileTableDataForCategoryAsync("TRCOUT"))
                                            .Where(s => s.Active == true);

        foreach (var federalTraceOutgoingSource in federalTraceOutgoingSources)
        {
            var errors = new List<string>();
            (string filePath, errors) = await federalFileManager.CreateOutputFileAsync(federalTraceOutgoingSource.Name);
            if (errors.Count == 0)
            {
                if (!string.IsNullOrEmpty(filePath))
                    ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
                else
                    ColourConsole.WriteEmbeddedColorLine($"Skipped [cyan]{federalTraceOutgoingSource.Name}[/cyan]. No data/events were found.");
            }
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{federalTraceOutgoingSource.Name}[/cyan]: [red]{error}[/red]");
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync("TRCOUT", federalTraceOutgoingSource.Name,
                                                                               new Exception(error), displayExceptionError: true);
                }
        }

    }
}
