using CompareOldAndNewData.CommandLine;
using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Comparing Data From Old System with new System");
Console.WriteLine("----------------------------------------------");

string? aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true)
        .Build();

var foaea2DB = new DBTools(configuration.GetConnectionString("Foaea2DB").ReplaceVariablesWithEnvironmentValues());
var foaea3DB = new DBTools(configuration.GetConnectionString("Foaea3DB").ReplaceVariablesWithEnvironmentValues());

var repositories2 = new DbRepositories(foaea2DB);
var repositories2Finance = new DbRepositories_Finance(foaea2DB);

var repositories3 = new DbRepositories(foaea3DB);
var repositories3Finance = new DbRepositories_Finance(foaea3DB);

string enfSrv = "ON01";
string ctrlCd = "P02862";

// appl
var diffs = CompareAppl.Run(repositories2, repositories3, enfSrv, ctrlCd);
Console.WriteLine($"Table\tKey\tColumn\tGood\tBad");
foreach (var diff in diffs)
    Console.WriteLine($"Appl\t{diff.Key}\t{diff.ColName}\t{diff.GoodValue}\t{diff.BadValue}");

// summSmry
diffs = CompareSummSmry.Run(repositories2Finance, repositories3Finance, enfSrv, ctrlCd);
foreach (var diff in diffs)
    Console.WriteLine($"SummSmry\t{diff.Key}\t{diff.ColName}\t{diff.GoodValue}\t{diff.BadValue}");

// intFinH
diffs = CompareIntFinH.Run(repositories2, repositories3, enfSrv, ctrlCd);
foreach (var diff in diffs)
    Console.WriteLine($"IntFinH\t{diff.Key}\t{diff.ColName}\t{diff.GoodValue}\t{diff.BadValue}");


// hldbCnd

// evntSubm

// evntBF
Console.WriteLine("\nFinished");
