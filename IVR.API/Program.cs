using FOAEA3.IVR;
//using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using FOAEA3.IVR.Helpers;
//using FOAEA3.Model.Interfaces.Repository;
//using IVR.API.Data;

//// WARNING: FOR testing only -- will use fake data instead of db data
//var argsList = args.ToList();
//argsList.Add("--UseInMemoryData=Yes"); 

//await Startup.SetupAndRun(argsList.ToArray(), AddFakeDataService);

//static void AddFakeDataService(IServiceCollection services)
//{
//    services.AddScoped<IIVRRepository>(m => ActivatorUtilities.CreateInstance<FakeDataIVR>(m));
//}
await Startup.SetupAndRun(args);
//namespace IVR.API
//{
//    public partial class Program
//    {
//        public static void Main(string[] args)
//        {
//            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

//            var config = new IVRConfigurationHelper(args);

//            CreateHostBuilder(args).Build().Run();


//        }

//        public static IHostBuilder CreateHostBuilder(string[] args) =>
//            Host.CreateDefaultBuilder(args)
//                .UseSerilog()
//                .ConfigureWebHostDefaults(webBuilder =>
//                {
//                    webBuilder.UseStartup<Startup>();
//                });



//    }
//}