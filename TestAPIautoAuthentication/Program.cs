using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;

bool runFileBrokerTest = false;
bool runFoaeaTest = true;

string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
    .AddCommandLine(args)
    ;

IConfiguration configuration = builder.Build();

var apiFilesConfig = configuration.GetSection("APIroot");

if (runFileBrokerTest)
{
    ColourConsole.WriteLine("--- API TOKEN TEST ---", "cyan");

    var apiBrokerHelper = new APIBrokerHelper(currentSubmitter: "TEST", currentUser: "TEST");

    string fileBrokerUserName = configuration["FILE_BROKER:userName"].ReplaceVariablesWithEnvironmentValues();
    string fileBrokerUserPassword = configuration["FILE_BROKER:userPassword"].ReplaceVariablesWithEnvironmentValues();

    var loginData = new FileBroker.Model.LoginData
    {
        UserName = fileBrokerUserName,
        Password = fileBrokerUserPassword
    };

    string api = "api/v1/Tokens";
    string fileBrokerRootAPI = apiFilesConfig["FileBrokerAccountRootAPI"].ReplaceVariablesWithEnvironmentValues();
    var tokenData = await apiBrokerHelper.PostDataAsync<TokenData, FileBroker.Model.LoginData>(api,
                                                            loginData, root: fileBrokerRootAPI);

    if (tokenData is null)
        return;

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Bearer:[/yellow] {tokenData.Token}");

    string fileBrokerMEPInterceptionRootAPI = apiFilesConfig["FileBrokerMEPInterceptionRootAPI"].ReplaceVariablesWithEnvironmentValues();

    string versionAPI = "api/v1/interceptionFiles/Version";
    var response = await apiBrokerHelper.GetStringAsync(versionAPI, root: fileBrokerMEPInterceptionRootAPI,
                                                        token: tokenData);

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Version:[/yellow] {response}");
}

if (runFoaeaTest)
{
    ColourConsole.WriteLine("\n--- FOAEA LOGIN TEST ---", "cyan");

    string foaeaApplicationRootAPI = apiFilesConfig["FoaeaApplicationRootAPI"].ReplaceVariablesWithEnvironmentValues();
    string foaeaInterceptionRootAPI = apiFilesConfig["FoaeaInterceptionRootAPI"].ReplaceVariablesWithEnvironmentValues();

    var apiApplHelper = new APIBrokerHelper(foaeaApplicationRootAPI, "TEST", "TEST");
    var applicationApplicationAPIs = new ApplicationAPIBroker(apiApplHelper);
    var productionAuditAPIs = new ProductionAuditAPIBroker(apiApplHelper);
    var loginAPIs = new LoginsAPIBroker(apiApplHelper);

    var apiInterceptionApplHelper = new APIBrokerHelper(foaeaInterceptionRootAPI, "TEST", "TEST");
    var interceptionApplicationAPIs = new InterceptionApplicationAPIBroker(apiInterceptionApplHelper);

    var apis = new APIBrokerList
    {
        Applications = applicationApplicationAPIs,
        InterceptionApplications = interceptionApplicationAPIs,
        ProductionAudits = productionAuditAPIs,
        Accounts = loginAPIs
    };

    string foaeaUserName = configuration["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues();
    string foaeaUserPassword = configuration["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues();
    string foaeaSubmitter = configuration["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues();

    var foaeaLoginData = new LoginData2
    {
        UserName = foaeaUserName,
        Password = foaeaUserPassword,
        Submitter = foaeaSubmitter
    };

    string loginResult = await apis.Accounts.LoginAsync(foaeaLoginData);

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Login:[/yellow] {loginResult}");

    string loginVerification = await apis.Accounts.LoginVerificationAsync(foaeaLoginData);

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Login Verification:[/yellow] {loginVerification}");

    string foaeaInterceptionVersion = await interceptionApplicationAPIs.GetVersionAsync();

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Interception API Version:[/yellow] {foaeaInterceptionVersion}");

    await apis.Accounts.LogoutAsync(foaeaLoginData);

    ColourConsole.WriteLine("\nLogout", "yellow");

    string loginVerification2 = await apis.Accounts.LoginVerificationAsync(foaeaLoginData);

    ColourConsole.WriteEmbeddedColorLine($"\n[yellow]Login Verification:[/yellow] {loginVerification2}");
}

Console.ReadKey();