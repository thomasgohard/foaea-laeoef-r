using FileBroker.Common.Filters;
using FileBroker.Model;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileBroker.Common
{
    public static class Startup
    {
        public static void ConfigureAPIServices(IServiceCollection services, IConfiguration configuration, string apiName)
        {
            LoggingHelper.SetupLogging(configuration, "FILEBROKER-API", "FileBroker", "Logs-API-FileBroker");

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiName, Version = "v1" });
            });

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.RespectBrowserAcceptHeader = true;
                options.Filters.Add(new ActionAutoLoggerFilter());
            }).AddXmlSerializerFormatters();

            //services.AddAuthentication(LoggingHelper.COOKIE_ID)
            //        .AddJwtBearer(options =>
            //        {
            //            options.TokenValidationParameters = new TokenValidationParameters()
            //            {
            //                ValidIssuer = "Justice",
            //                ValidAudience = "Justice",
            //                IssuerSigningKey = new SymmetricSecurityKey(
            //                    Encoding.UTF8.GetBytes(configuration["Tokens:Key"].ReplaceVariablesWithEnvironmentValues()))
            //            };
            //        });

            services.AddEndpointsApiExplorer();
            services.Configure<ProvincialAuditFileConfig>(configuration.GetSection("AuditConfig"));
            services.Configure<ApiConfig>(configuration.GetSection("APIroot"));

            string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

            DataHelper.ConfigureDBServices(services, fileBrokerCON);
        }

        public static void ConfigureAPI(WebApplication app, IWebHostEnvironment env, IConfiguration configuration, string apiName)
        {
            string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{fileBrokerCON}[/yellow]");

            Log.Information("Starting API {apiName}", apiName);
            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);
            Log.Information("Connection = {fileBrokerCON}", fileBrokerCON);

            string currentServer = Environment.MachineName;
            var prodServersSection = configuration.GetSection("ProductionServers");
            var prodServers = prodServersSection.Get<List<string>>();
            for (int i = 0; i < prodServers.Count; i++)
                prodServers[i] = prodServers[i].ReplaceVariablesWithEnvironmentValues();

            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else if (prodServers.Any(prodServer => prodServer.ToLower() == currentServer.ToLower()))
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later");
                    });
                });
                app.UseHsts();
            }
            else
            {

                Log.Fatal("Trying to use Production environment on non-production server {currentServer}. Application stopping!", currentServer);
                Console.WriteLine($"Trying to use Production environment on non-production server {currentServer}");
                Console.WriteLine("Application stopping...");

                Task.Delay(2000).Wait();

                app.Lifetime.StopApplication();
            }

            var api_url = configuration["Urls"];
            ColourConsole.WriteEmbeddedColorLine($"\n[green]Waiting for API calls...[/green][yellow]{api_url}[/yellow]\n");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

        /*
         
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("Submitter", submitter),
                //new Claim(JwtRegisteredClaimNames.Sub, subject.EMailAddress),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.UniqueName, userName)
            };

            var identity = new ClaimsIdentity(claims, LoggingHelper.COOKIE_ID);
            var principal = new ClaimsPrincipal(identity);
          
            var encodedApiKey = Encoding.UTF8.GetBytes(apiKey);
            var securityKey = new SymmetricSecurityKey(encodedApiKey);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("Justice", "Justice", claims, signingCredentials: creds,
                                 expires: DateTime.UtcNow.AddMinutes(20));    
        
            //[AllowAnonymous]
            //[HttpPost("TestTokens")]
            //public async Task<ActionResult> CreateToken([FromBody] LoginData2 loginData,
            //                                            [FromServices] IRepositories db,
            //                                            [FromServices] IConfiguration config)
            //{
            //    // WARNING: not for production use!
            //    string tokenKey = config["Tokens:Key"];
            //    (var principal, var token) = await TestLogin.AutoLogin(loginData.UserName, loginData.Password,
            //                                                           loginData.Submitter, db,
            //                                                           tokenKey.ReplaceVariablesWithEnvironmentValues());
            //    if (principal is not null && principal.Identity is not null)
            //    {
            //        await HttpContext.SignInAsync(LoggingHelper.COOKIE_ID, principal);

            //        return Created("", new
            //        {
            //            token = new JwtSecurityTokenHandler().WriteToken(token),
            //            expiration = token.ValidTo
            //        });
            //    }
            //    else
            //    {
            //        return BadRequest();
            //    }
            //}
         */
    }
}
