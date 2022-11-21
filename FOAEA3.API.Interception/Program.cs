using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;

var builder = WebApplication.CreateBuilder(args);
var localConfig = builder.Configuration;
InterceptionManager.ESDsites = localConfig.GetSection("ESDsites").Get<List<string>>();

await Startup.SetupAndRun(args);

public partial class Program { }