using DBHelper;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Incoming.FileWatcher.MEP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var fileBrokerDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());
            var errorTrackingDB = new DBErrorTracking(fileBrokerDB);

            var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

            var fileTable = new DBFileTable(fileBrokerDB);
            var fileTableData = fileTable.GetFileTableDataForCategory("INTAPPIN");
            fileTableData.AddRange(fileTable.GetFileTableDataForCategory("LICAPPIN"));
            fileTableData.AddRange(fileTable.GetFileTableDataForCategory("TRCAPPIN"));

            var validProvinces = fileTableData.Where(f => f.Active.HasValue && f.Active.Value)
                                              .GroupBy(f => f.Name[..2].ToUpper())
                                              .Select(group => group.First().Name[..2].ToUpper())
                                              .ToList();

            string provinceCode = (args.Length > 0) ? args[0]?.ToUpper() : string.Empty;
            if (string.IsNullOrEmpty(provinceCode) || !validProvinces.Contains(provinceCode))
            {
                ColourConsole.WriteEmbeddedColorLine("Error starting MEP File Monitor. Missing or invalid province argument on command line.");
                ColourConsole.WriteEmbeddedColorLine("Must be one of: " + string.Join(", ", validProvinces));
                return;
            }

            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{provinceCode}[/cyan] MEP File Monitor");

            var provinceFilesData = fileTableData.Where(f => (f.Name[..2].ToUpper() == provinceCode) &&
                                                            (f.Category.ToUpper().In("TRCAPPIN", "INTAPPIN", "LICAPPIN")) &&
                                                            (f.Active.HasValue && f.Active.Value)).ToList();

            string tracingName = null;
            string interceptionName = null;
            string licenceName = null;

            var searchPaths = new List<string>();
            foreach (var provinceFileData in provinceFilesData)
            {
                if (provinceFileData.Category.ToUpper() == "TRCAPPIN") tracingName = provinceFileData.Name;
                if (provinceFileData.Category.ToUpper() == "INTAPPIN") interceptionName = provinceFileData.Name;
                if (provinceFileData.Category.ToUpper() == "LICAPPIN") licenceName = provinceFileData.Name;

                string thisPath = provinceFileData.Path.ToUpper();
                if (!searchPaths.Contains(thisPath))
                    searchPaths.Add(thisPath);
            }

            var apiAction = new APIBrokerHelper();
            var provincialFileManager = new IncomingProvincialFile(fileBrokerDB, apiRootData, apiAction,
                                                                   interceptionBaseName: interceptionName,
                                                                   tracingBaseName: tracingName,
                                                                   licencingBaseName: licenceName);

            var newFiles = new Dictionary<string, FileTableData>();
            foreach(string searchPath in searchPaths)
                provincialFileManager.GetNewFiles(searchPath, ref newFiles);

            if (newFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{newFiles.Count}[/green] file(s)");

                foreach (var newFile in newFiles)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile.Key}[/green]...");

                    var errors = new List<string>();
                    provincialFileManager.ProcessNewFile(newFile.Key, ref errors);
                    if (errors.Any())
                        foreach (var error in errors)
                            errorTrackingDB.MessageBrokerError($"{provinceCode} APPIN", newFile.Key, new Exception(error), false);
                }
            }
            else
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");

        }
    }
}
