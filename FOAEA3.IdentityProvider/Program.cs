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

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.User.RequireUniqueEmail = false;
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireNonAlphanumeric = true;
                        options.Password.RequireDigit = true;
                    })
                .AddEntityFrameworkStores<FOAEA3IdentityManagerContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer();

builder.Services.AddDbContext<FOAEA3IdentityManagerContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddRazorPages();

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
