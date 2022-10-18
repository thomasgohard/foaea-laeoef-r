using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Resources.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

var config = builder.Configuration;
var env = builder.Environment;
var apiName = env.ApplicationName;

LoggingHelper.SetupLogging(config, "FOAEA3-API-LicenceDenial");

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiName, Version = "v1" });
});

if (!Startup.UseInMemoryData(builder))
    Startup.AddDBServices(builder.Services, config.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());
Startup.ConfigureAPIServices(builder.Services, config);

var app = builder.Build();

Startup.ConfigureAPI(app, env, config, apiName);

if (!Startup.UseInMemoryData(builder))
    await Startup.AddReferenceDataFromDB(app);

var api_url = config["Urls"];

ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]\n");

app.Run();
