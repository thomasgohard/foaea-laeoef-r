﻿using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Structs;
using FOAEA3.Resources.Helpers;
using static FOAEA3.Resources.Helpers.ColourConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using FileBroker.Data;
using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using FileBroker.Common;

namespace Incoming.FileWatcher.MEP;

internal class Program
{
    static async Task Main(string[] args)
    {
        string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        var fileBrokerDB = new DBToolsAsync(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
        var db = new RepositoryList
        {
            FlatFileSpecs = new DBFlatFileSpecification(fileBrokerDB),
            FileTable = new DBFileTable(fileBrokerDB),
            FileAudit = new DBFileAudit(fileBrokerDB),
            ProcessParameterTable = new DBProcessParameter(fileBrokerDB),
            OutboundAuditTable = new DBOutboundAudit(fileBrokerDB),
            ErrorTrackingTable = new DBErrorTracking(fileBrokerDB),
            MailService = new DBMailService(fileBrokerDB),
            TranslationTable = new DBTranslation(fileBrokerDB),
            RequestLogTable = new DBRequestLog(fileBrokerDB),
            LoadInboundAuditTable = new DBLoadInboundAudit(fileBrokerDB)
        };

        await db.RequestLogTable.DeleteAllAsync();

        string provinceCode = args.Any() ? args.First()?.ToUpper() : "ALL";
        var filesToProcess = await GetFileTableDataForArgsOrAllAsync(args, new DBFileTable(fileBrokerDB));

        if (!filesToProcess.Any())
        {
            await GenerateError(db.ErrorTrackingTable, "No items found in FileTable?");
            return;
        }

        var provinces = GetSelectedProvinceCodes(filesToProcess);

        if ((string.IsNullOrEmpty(provinceCode) || !provinces.Contains(provinceCode)) && (provinceCode != "ALL"))
        {
            await GenerateError(db.ErrorTrackingTable, $"Invalid province argument on command line: [{provinceCode}]\nMust be one of: " +
                                                       string.Join(", ", provinces + " or ALL"));
            return;
        }

        DateTime start = DateTime.Now;
        WriteEmbeddedColorLine($"Starting [cyan]{provinceCode}[/cyan] MEP File Monitor");
        WriteEmbeddedColorLine($"Starting time [orange]{start}[/orange]");

        var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();        

        APIBrokerList foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

        int totalFilesFound = 0;
        foreach (var itemProvince in provinces)
        {
            string provCode = itemProvince.ToUpper();
            var searchPaths = GetFileSearchPaths(filesToProcess, out FileBaseName fileBaseName, provCode);

            var fileTable = new DBFileTable(fileBrokerDB);
            var provincialFileManager = new IncomingProvincialFile(db, foaeaApis, fileBaseName, configuration);

            bool foundZero = true;

            var processedFiles = new List<string>();
            bool AlreadyProcessed(string f) => processedFiles.Contains(f);

            bool finishedForProvince = false;
            while (!finishedForProvince)
            {
                var allNewFiles = await provincialFileManager.GetWaitingFiles(searchPaths);

                if (allNewFiles.Any())
                {
                    foundZero = false;
                    string moreThanOne = allNewFiles.Count > 1 ? "s" : "";

                    if (!AlreadyProcessed(allNewFiles.First()))
                    {
                        WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file{moreThanOne} in [green]{itemProvince}[/green]");
                        totalFilesFound += allNewFiles.Count;
                    }

                    foreach (var newFile in allNewFiles)
                    {
                        if (AlreadyProcessed(newFile))
                        {
                            finishedForProvince = true;
                            break;
                        }

                        processedFiles.Add(newFile);

                        WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");

                        var errors = new List<string>();

                        await provincialFileManager.ProcessWaitingFile(newFile, errors);

                        if (errors.Any())
                        {
                            foreach (var error in errors)
                                await db.ErrorTrackingTable.MessageBrokerErrorAsync($"{provinceCode} APPIN", newFile, new Exception(error), displayExceptionError: true);
                            finishedForProvince = true;
                        }
                    }
                }
                else
                {
                    finishedForProvince = true;
                    if (foundZero)
                        WriteEmbeddedColorLine($"Found [green]0[/green] file in [green]{itemProvince}[/green]");
                }
            }

        }

        WriteEmbeddedColorLine($"Completed. [yellow]{totalFilesFound}[/yellow] processed.");

        DateTime end = DateTime.Now;
        var duration = end - start;

        WriteEmbeddedColorLine($"Completion time [orange]{end}[/orange] (duration: [yellow]{duration.Minutes}[/yellow] minutes)");

        Console.ReadKey();

    }

    private static async Task GenerateError(IErrorTrackingRepository errorTrackingDB, string error)
    {
        WriteEmbeddedColorLine(error);
        await errorTrackingDB.MessageBrokerErrorAsync($"Incoming MEP File Processing", "Error starting MEP File Monitor",
                                                      new Exception(error), displayExceptionError: true);
    }

    private static List<string> GetFileSearchPaths(List<FileTableData> filesToProcess, out FileBaseName fileBaseName, string provinceCode)
    {
        var searchPaths = new List<string>();
        fileBaseName = new FileBaseName();

        var thisProvinceFilesData = filesToProcess.Where(f => f.Name[..2].ToUpper() == provinceCode);
        string interceptionPath = string.Empty;
        foreach (var provinceFileData in thisProvinceFilesData)
        {
            string category = provinceFileData.Category.ToUpper().Trim();

            switch (category)
            {
                case "TRCAPPIN":
                    fileBaseName.Tracing = provinceFileData.Name.Trim().ToUpper();
                    break;
                case "INTAPPIN":
                    fileBaseName.Interception = provinceFileData.Name.Trim().ToUpper();
                    interceptionPath = provinceFileData.Path.ToUpper();
                    break;
                case "ESD":
                    fileBaseName.ESD = provinceFileData.Name.Trim().ToUpper();
                    break;
                case "LICAPPIN":
                    fileBaseName.Licencing = provinceFileData.Name.Trim().ToUpper();
                    break;
                default:
                    // ignore all other categories
                    break;
            }

            string thisPath = provinceFileData.Path.ToUpper();
            if ((!searchPaths.Contains(thisPath)) && (category != "ESD"))
                searchPaths.Add(thisPath);
        }

        if (!string.IsNullOrEmpty(interceptionPath))
            searchPaths.Add(interceptionPath.AppendPath("ESD"));

        return searchPaths;
    }

    private static List<string> GetSelectedProvinceCodes(List<FileTableData> fileTableData)
    {
        return fileTableData.Where(f => f.Active.HasValue && f.Active.Value)
                            .GroupBy(f => f.Name[..2].ToUpper())
                            .Select(group => group.First().Name[..2].ToUpper())
                            .ToList();
    }

    private static async Task<List<FileTableData>> GetFileTableDataForArgsOrAllAsync(string[] args, DBFileTable fileTable)
    {
        var fileTableData = new List<FileTableData>();

        bool loadAllCategories = true;
        if (args.Length > 1)
        {
            string option = args[1].ToUpper();
            switch (option)
            {
                case "TRACE_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("TRCAPPIN"));
                    loadAllCategories = false;
                    break;
                case "INTERCEPTION_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("INTAPPIN"));
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("ESD"));
                    loadAllCategories = false;
                    break;
                case "LICENCE_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("LICAPPIN"));
                    loadAllCategories = false;
                    break;
                default:
                    break;
            }
        }
        if (loadAllCategories)
        {
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("TRCAPPIN"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("INTAPPIN"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("ESD"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("LICAPPIN"));
        }

        fileTableData = fileTableData.Where(f => f.Active.HasValue && f.Active.Value).ToList();

        return fileTableData;
    }

}


