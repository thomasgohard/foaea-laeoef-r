using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace FOAEA3.Common.Helpers
{
    public static class LoggingHelper
    {
        public static void SetupLogging(IConfiguration config, string sourceNameForEvents,
                                        string connName = "FOAEAMain", string apiTableName = "Logs-API")
        {
            string logPath = config["FileLogPath"];
            LoggerConfiguration logConfig = SetupLogConfiguration(config, sourceNameForEvents, connName, apiTableName);

            if (!string.IsNullOrEmpty(logPath))
                logConfig = logConfig.WriteTo.File(logPath,
                                                   rollingInterval: RollingInterval.Day,
                                                   outputTemplate: "API: {Timestamp} {Message}{NewLine:1}{Exception:1}");

            Log.Logger = logConfig.CreateLogger();
        }

        private static LoggerConfiguration SetupLogConfiguration(IConfiguration config, string sourceName,
                                                                 string connName, string apiTableName)
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
                TableName = apiTableName,
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
                           connectionString: config[$"ConnectionStrings:{connName}"].ReplaceVariablesWithEnvironmentValues(),
                           restrictedToMinimumLevel: LogEventLevel.Information,
                           sinkOptions: sqlSinkOpts,
                           columnOptions: sqlColumnOpts);

            // also log to the Windows Event Viewer

            logConfig = logConfig.WriteTo.EventLog(source: sourceName,
                                  logName: "Application",
                                  manageEventSource: true,
                                  restrictedToMinimumLevel: LogEventLevel.Information);
            return logConfig;
        }
    }
}
