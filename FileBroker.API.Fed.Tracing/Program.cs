using FileBroker.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

var configuration = builder.Configuration;
var env = builder.Environment;
var apiName = env.ApplicationName;

Startup.ConfigureAPIServices(builder.Services, configuration, apiName);

var app = builder.Build();

Startup.ConfigureAPI(app, env, configuration, apiName);

app.Run();