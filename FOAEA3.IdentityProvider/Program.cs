using FOAEA3.IdentityManager.Areas.Identity;
using FOAEA3.IdentityManager.Areas.Identity.Data;
using FOAEA3.IdentityManager.Data;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FOAEA3IdentityManagerContextConnection").ReplaceVariablesWithEnvironmentValues()
                            ?? throw new InvalidOperationException("Connection string 'FOAEA3IdentityManagerContextConnection' not found.");

builder.Services.AddDbContext<FOAEA3IdentityManagerContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                    })
                .AddEntityFrameworkStores<FOAEA3IdentityManagerContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddRazorPages();

// WARNING: this is only used for local testing -- dev/uat/production would use a cloud-based provider (or a department one at the minimum)
//builder.Services.AddIdentityServer()
//                .AddInMemoryIdentityResources(builder.Configuration.GetSection("IdentityServer:IdentityResources"))
//                .AddInMemoryApiResources(builder.Configuration.GetSection("IdentityServer:ApiResources"))
//                .AddInMemoryClients(builder.Configuration.GetSection("IdentityServer:Clients"))
//                .AddDeveloperSigningCredential()
//                .AddAspNetIdentity<ApplicationUser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
