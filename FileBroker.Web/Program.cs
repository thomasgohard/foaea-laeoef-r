using DBHelper;
using FileBroker.Common;
using FileBroker.Web.Filter;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var config = new FileBrokerConfigurationHelper(args);

string actualConnection = DataHelper.ConfigureDBServices(builder.Services, config.FileBrokerConnection);
var mainDB = new DBTools(actualConnection);

builder.Services.AddRazorPages()
        .AddMvcOptions(options =>
        {
            options.Filters.Add(new RazorPageActionFilter(mainDB));
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
