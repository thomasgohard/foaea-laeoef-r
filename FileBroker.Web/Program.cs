using DBHelper;
using FileBroker.Common;
using FileBroker.Model;
using FileBroker.Web.Filter;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var configuration = builder.Configuration;

builder.Services.Configure<ProvincialAuditFileConfig>(configuration.GetSection("AuditConfig"));
builder.Services.Configure<ApiConfig>(configuration.GetSection("APIroot"));

string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

string actualConnection = DataHelper.ConfigureDBServices(builder.Services, fileBrokerCON);
var mainDB = new DBTools(actualConnection);

builder.Services.AddRazorPages()
        .AddMvcOptions(options =>
        {
            options.Filters.Add(new RazorPageActionFilter(configuration, mainDB));
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
