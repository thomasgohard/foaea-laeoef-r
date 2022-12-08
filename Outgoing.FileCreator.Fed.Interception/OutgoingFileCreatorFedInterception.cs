using DBHelper;
using FileBroker.Business;
using FileBroker.Common;
using FileBroker.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;

namespace Outgoing.FileCreator.Fed.Interception
{
    public static class OutgoingFileCreatorFedInterception
    {
        public static async Task RunCRA(string[] args = null)
        {
            args ??= Array.Empty<string>();

            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing CRA EISO File Creator");

            var config = new FileBrokerConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            await CreateOutgoingCRAasync(fileBrokerDB, config.ApiRootData, config);

            ColourConsole.Write("Completed.\n");
        }

        private static async Task CreateOutgoingCRAasync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                         IFileBrokerConfigurationHelper config)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var financialManager = new OutgoingFinancialEISOmanager(foaeaApis, db, config);

            var outgoingProcessData = (await db.FileTable.GetFileTableDataForCategoryAsync("EISOOUT")).First();

            var errors = new List<string>();

            string filePath = await financialManager.CreateCRAfile(outgoingProcessData.Name, errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{outgoingProcessData.Name}[/cyan]: [red]{error}[/red]");
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync(outgoingProcessData.Category, outgoingProcessData.Name,
                                                                                new Exception(error), displayExceptionError: true);
                }
        }

        public static async Task RunEI(string[] args = null)
        {
            args ??= Array.Empty<string>();

            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing EI EISO File Creator");

            var config = new FileBrokerConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            await CreateOutgoingEIasync(fileBrokerDB, config.ApiRootData, config);

            ColourConsole.Write("Completed.\n");
        }

        private static async Task CreateOutgoingEIasync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                        IFileBrokerConfigurationHelper config)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var financialManager = new OutgoingFinancialEISOmanager(foaeaApis, db, config);

            var outgoingProcessData = (await db.FileTable.GetFileTableDataForCategoryAsync("DOJEEOUT")).First();

            var errors = new List<string>();

            string filePath = await financialManager.CreateEIfile(outgoingProcessData.Name, errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{outgoingProcessData.Name}[/cyan]: [red]{error}[/red]");
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync(outgoingProcessData.Category, outgoingProcessData.Name,
                                                                                new Exception(error), displayExceptionError: true);
                }

        }

        public static async Task RunCPP(string[] args = null)
        {
            args ??= Array.Empty<string>();

            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing CPP EISO File Creator");

            var config = new FileBrokerConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            await CreateOutgoingCPPasync(fileBrokerDB, config.ApiRootData, config);

            ColourConsole.Write("Completed.\n");
        }

        private static async Task CreateOutgoingCPPasync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                         IFileBrokerConfigurationHelper config)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var financialManager = new OutgoingFinancialEISOmanager(foaeaApis, db, config);

            var outgoingProcessData = (await db.FileTable.GetFileTableDataForCategoryAsync("DOJCPPOUT")).First();

            var errors = new List<string>();

            string filePath = await financialManager.CreateCPPfile(outgoingProcessData.Name, errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{outgoingProcessData.Name}[/cyan]: [red]{error}[/red]");
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync(outgoingProcessData.Category, outgoingProcessData.Name,
                                                                                new Exception(error), displayExceptionError: true);
                }

        }

        public static async Task RunBlockFunds(string[] args = null)
        {
            if ((args == null) || (args.Length == 0))
            {
                ColourConsole.WriteEmbeddedColorLine("[red]Error:[/red] Missing category.\n");
                return;
            }

            ColourConsole.WriteEmbeddedColorLine("Starting Federal Outgoing Interception File Creator");

            var config = new FileBrokerConfigurationHelper();
            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            foreach (string category in args)
            {
                ColourConsole.WriteEmbeddedColorLine($"Processing [yellow]{category}[/yellow]...");

                await CreateOutgoingBlockFundsAsync(fileBrokerDB, config.ApiRootData, config, category);
            }

            ColourConsole.Write("Completed.\n");
        }

        private static async Task CreateOutgoingBlockFundsAsync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                                IFileBrokerConfigurationHelper config, string category)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var financialManager = new OutgoingFinancialBlockFundsManager(foaeaApis, db, config);

            var outgoingProcessData = (await db.FileTable.GetFileTableDataForCategoryAsync(category)).First();

            var errors = new List<string>();

            string filePath = await financialManager.CreateBlockFundsFile(outgoingProcessData.Name, errors);

            if (errors.Count == 0)
                ColourConsole.WriteEmbeddedColorLine($"Successfully created [cyan]{filePath}[/cyan]");
            else
                foreach (var error in errors)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Error creating [cyan]{outgoingProcessData.Name}[/cyan]: [red]{error}[/red]");
                    await db.ErrorTrackingTable.MessageBrokerErrorAsync(outgoingProcessData.Category, outgoingProcessData.Name,
                                                                                new Exception(error), displayExceptionError: true);
                }
        }
    }
}
