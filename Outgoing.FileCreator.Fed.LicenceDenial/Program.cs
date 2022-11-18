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
using System.Configuration;
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

            var config = new ConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);

            await CreateOutgoingFederalLicenceDenialFilesAsync(fileBrokerDB, config.ApiRootData, config);

            ColourConsole.Write("Completed.");

        }

        private static async Task CreateOutgoingFederalLicenceDenialFilesAsync(DBToolsAsync fileBrokerDB, ApiConfig apiRootData,
                                                                               ConfigurationHelper config)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);
            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var federalFileManager = new OutgoingFederalLicenceDenialManager(foaeaApis, db, config);

            var federalLicenceDenialOutgoingSources = (await db.FileTable.GetFileTableDataForCategoryAsync("LICOUT"))
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
                        await db.ErrorTrackingTable.MessageBrokerErrorAsync("LICOUT", federalLicenceDenialOutgoingSource.Name, 
                                                                                   new Exception(error), displayExceptionError: true);
                    }
            }

        }
    }
}
