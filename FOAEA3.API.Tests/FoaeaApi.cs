using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TestData.TestDataBase;
using TestData.TestDB;

namespace FOAEA3.API.Tests
{
    public class FoaeaApi : WebApplicationFactory<Program>
    {
        public bool UseInMemoryData { get; }

        public FoaeaApi(bool useInMemoryData = false)
        {
            UseInMemoryData = useInMemoryData;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if (UseInMemoryData)
            {
                builder.UseSetting("UseInMemoryData", "Yes");

                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IRepositories>(m => ActivatorUtilities.CreateInstance<InMemory_Repositories>(m));
                    services.AddSingleton<IRepositories_Finance>(m => ActivatorUtilities.CreateInstance<InMemory_RepositoriesFinance>(m)); // to access database procs for finance tables

                    services.AddScoped<IFoaEventsRepository>(m => ActivatorUtilities.CreateInstance<InMemoryFoaEvents>(m));
                    services.AddScoped<IActiveStatusRepository>(m => ActivatorUtilities.CreateInstance<InMemoryActiveStatus>(m));
                    services.AddScoped<IGenderRepository>(m => ActivatorUtilities.CreateInstance<InMemoryGender>(m));
                    services.AddScoped<IApplicationCommentsRepository>(m => ActivatorUtilities.CreateInstance<InMemoryApplicationComments>(m));
                    services.AddScoped<IApplicationLifeStateRepository>(m => ActivatorUtilities.CreateInstance<InMemoryApplicationLifeState>(m));
                });
            }

            base.ConfigureWebHost(builder);
        }

    }
}
