using FileBroker.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

var localConfig = builder.Configuration;

var configuration = new ConfigurationHelper(args);
var env = builder.Environment;
var apiName = env.ApplicationName;

Startup.ConfigureAPIServices(builder.Services, configuration, apiName);

var app = builder.Build();

Startup.ConfigureAPI(app, env, configuration, apiName, localConfig["Urls"]);

app.Run();