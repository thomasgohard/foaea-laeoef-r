using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Incoming.FileWatcher.Fed.Tracing;

class Program
{
    static async Task Main(string[] args)
    {
        ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] Federal Tracing File Monitor");

        var config = new FileBrokerConfigurationHelper(args);

        var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);
        var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

        var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(config.ApiRootData);

        var federalFileManager = new IncomingFederalTracingFile(db, foaeaApis, config);

        var traceInFileTableData = await db.FileTable.GetFileTableDataForCategory("TRCIN");
        var folders = traceInFileTableData.Select(m => m.Path).Distinct().ToList();

        var processedFiles = new List<string>();
        bool finished = false;
        while (!finished)
        {
            var allNewFiles = new List<string>();
            foreach (var folder in folders)
                await federalFileManager.GetNextExpectedIncomingFilesFoundInFolder(folder, allNewFiles);

            if (allNewFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");

                foreach (var thisFile in allNewFiles)
                {
                    if (processedFiles.Contains(thisFile))
                    {
                        finished = true;
                        break;
                    }

                    processedFiles.Add(thisFile);

                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{thisFile}[/green]...");

                    await federalFileManager.ProcessWaitingFile(thisFile);

                    if (federalFileManager.Errors.Any())
                    {
                        finished = true;
                        foreach (var error in federalFileManager.Errors)
                            await db.ErrorTrackingTable.MessageBrokerError("TRCIN", thisFile, new Exception(error), displayExceptionError: true);
                    }

                    federalFileManager.Errors.Clear();
                }
            }
            else
            {
                finished = true;
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");
            }
        }
    }

}
