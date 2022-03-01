using FOAEA3.API.Filters;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FOAEA3.API.Interception
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FOAEA3.API.Startup.ConfigureAPIServices(services, Configuration, "FOAEA Interception API", "v1");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRepositories repositories, IHostApplicationLifetime appLifetime)
        {
            FOAEA3.API.Startup.ConfigureAPI(app, env, repositories, appLifetime, Configuration, "FOAEA3.API.Interception");
        }
    }
}
