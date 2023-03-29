using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace BackendProcesses.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new FoaeaConfigurationHelper(args);

            LoggerConfiguration logConfig = SetupLogConfiguration(config);

            Log.Logger = logConfig.CreateLogger();

            try
            {
                Log.Information("Starting API BackendProcesses");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The API failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static LoggerConfiguration SetupLogConfiguration(IFoaeaConfigurationHelper config)
        {
            var logConfig = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .Enrich.WithEnvironmentUserName();

            var sqlSinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs-API",
                SchemaName = "dbo",
                AutoCreateSqlTable = false
            };

            var sqlColumnOpts = new ColumnOptions();
            sqlColumnOpts.Store.Remove(StandardColumn.MessageTemplate);
            sqlColumnOpts.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn {ColumnName = "SubmitterCode", DataType = SqlDbType.Char, DataLength = 6},
                new SqlColumn {ColumnName = "MachineName", DataType = SqlDbType.VarChar, DataLength = 50},
                new SqlColumn {ColumnName = "ActionName", DataType = SqlDbType.VarChar, DataLength = 1024},
                new SqlColumn {ColumnName = "EnvironmentUserName", DataType = SqlDbType.VarChar, DataLength = 1024},
                new SqlColumn {ColumnName = "APIpath", DataType = SqlDbType.VarChar, DataLength = 2048}
            };

            // log to SQL Server table

            logConfig = logConfig.WriteTo.MSSqlServer(
                           connectionString: config.FoaeaConnection,
                           restrictedToMinimumLevel: LogEventLevel.Information,
                           sinkOptions: sqlSinkOpts,
                           columnOptions: sqlColumnOpts);

            // also log to the Windows Event Viewer

            logConfig = logConfig.WriteTo.EventLog(source: "BackendProcess-API",
                                  logName: "Application",
                                  manageEventSource: true,
                                  restrictedToMinimumLevel: LogEventLevel.Information);
            return logConfig;
        }
    }
}
