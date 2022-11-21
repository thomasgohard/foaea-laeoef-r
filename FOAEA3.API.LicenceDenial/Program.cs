using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Model;

var builder = WebApplication.CreateBuilder(args);
var localConfig = builder.Configuration;
LicenceDenialManager.Declaration = localConfig.GetSection("Declaration").Get<DeclarationData>();

await Startup.SetupAndRun(args);

public partial class Program { }