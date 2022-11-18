using DBHelper;
using FileBroker.Business;
using FileBroker.Common;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Outgoing.FileCreator.Fed.Tracing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Tracing Files Creator");

            var config = new ConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            await CreateOutgoingFederalTracingFiles(fileBrokerDB, config.ApiRootData, config);

            ColourConsole.Write("Completed.");

        }

        private static async Task CreateOutgoingFederalTracingFiles(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                                    ConfigurationHelper config)
        {

            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);
            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var federalFileManager = new OutgoingFederalTracingManager(foaeaApis, db, config);

            var federalTraceOutgoingSources = (await db.FileTable.GetFileTableDataForCategoryAsync("TRCOUT"))
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
                        await db.ErrorTrackingTable.MessageBrokerErrorAsync("TRCOUT", federalTraceOutgoingSource.Name,
                                                                                   new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
