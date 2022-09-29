using FOAEA3.Common;
using FOAEA3.Common.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

var config = builder.Configuration;
var env = builder.Environment;
var apiName = env.ApplicationName;

LoggingHelper.SetupLogging(config, "FOAEA3-API-Interception");

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiName, Version = "v1" });
});

Startup.ConfigureAPIServices(builder.Services, config);

var app = builder.Build();

await Startup.ConfigureAPI(app, env, config, apiName);

app.Run();
