using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FOAEA3.Resources.Helpers;

ColourConsole.WriteEmbeddedColorLine("Starting SIN Registry File Monitor");

var config = new FileBrokerConfigurationHelper(args);

var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);
var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(config.ApiRootData);

var federalFileManager = new IncomingFederalSinFile(db, foaeaApis, config);

var allNewFiles = new List<string>();
await federalFileManager.AddNewFiles(config.FTProot + @"\Hr3svs", allNewFiles);

if (allNewFiles.Count > 0)
{
    ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");
    foreach (var newFile in allNewFiles)
    {
        var errors = new List<string>();
        ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");
        await federalFileManager.ProcessNewFile(newFile);
        if (federalFileManager.Errors.Any())
            foreach (var error in federalFileManager.Errors)
                await db.ErrorTrackingTable.MessageBrokerError("SININ", newFile, new Exception(error), displayExceptionError: true);

    }
}
else
    ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");

