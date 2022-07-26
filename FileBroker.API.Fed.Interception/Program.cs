using FileBroker.Common;
using FileBroker.Model;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;

ColourConsole.WriteEmbeddedColorLine("Starting [cyan]FileBroker.API.Fed.Interception[/cyan]...");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

builder.Services.Configure<ProvincialAuditFileConfig>(configuration.GetSection("AuditConfig"));
builder.Services.Configure<ApiConfig>(configuration.GetSection("APIroot"));

string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

string actualConnection = DataHelper.ConfigureDBServices(builder.Services, fileBrokerCON);

ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{actualConnection}[/yellow]");

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{app.Environment.EnvironmentName}[/yellow]");

var api_url = configuration["Urls"];

ColourConsole.WriteEmbeddedColorLine($"\n[green]Waiting for API calls...[/green][yellow]{api_url}[/yellow]\n");

app.UseAuthorization();

app.MapControllers();

app.Run();