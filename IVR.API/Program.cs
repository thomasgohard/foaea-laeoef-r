using FOAEA3.Common;
using FOAEA3.Model.Interfaces.Repository;
using IVR.API.Data;

// WARNING: FOR testing only -- will use fake data instead of db data
var argsList = args.ToList();
argsList.Add("--UseInMemoryData=Yes"); 

await Startup.SetupAndRun(argsList.ToArray(), AddFakeDataService);

static void AddFakeDataService(IServiceCollection services)
{
    services.AddScoped<IIVRRepository>(m => ActivatorUtilities.CreateInstance<FakeDataIVR>(m));
}
