using FOAEA3.Common.Brokers;
using FOAEA3.Model;

namespace FileBroker.Common;

public class FoaeaSystemAccess
{
    public FoaeaLoginData LoginData { get; }

    public string CurrentToken { get; set; }
    private string CurrentRefreshToken { get; set; }

    private APIBrokerList APIs { get; }

    public FoaeaSystemAccess(APIBrokerList apis, string userName, string userPassword, string submitter)
    {
        APIs = apis;

        LoginData = new FoaeaLoginData
        {
            UserName = userName,
            Password = userPassword,
            Submitter = submitter
        };
    }

    public FoaeaSystemAccess(APIBrokerList apis, FoaeaLoginData foaeaLoginData)
    {
        APIs = apis;

        LoginData = foaeaLoginData;
    }

    public async Task<bool> SystemLogin()
    {
        var tokenData = await APIs.Accounts.Login(LoginData);

        if (tokenData.Token is null)
            return false;

        CurrentToken = tokenData.Token;
        CurrentRefreshToken = tokenData.RefreshToken;

        SetTokenForAPIs();

        if (APIs.Accounts is not null) APIs.Accounts.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.TracingApplications is not null) APIs.TracingApplications.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.TracingEvents is not null) APIs.TracingEvents.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.TracingResponses is not null) APIs.TracingResponses.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.Financials is not null) APIs.Financials.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.ControlBatches is not null) APIs.ControlBatches.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.Transactions is not null) APIs.Transactions.ApiHelper.GetRefreshedToken = RefreshToken;

        if (APIs.InterceptionApplications is not null) APIs.InterceptionApplications.ApiHelper.GetRefreshedToken = RefreshToken;

        if (APIs.LicenceDenialEvents is not null) APIs.LicenceDenialEvents.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.LicenceDenialApplications is not null) APIs.LicenceDenialApplications.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.LicenceDenialTerminationApplications is not null) APIs.LicenceDenialTerminationApplications.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.LicenceDenialResponses is not null) APIs.LicenceDenialResponses.ApiHelper.GetRefreshedToken = RefreshToken;

        if (APIs.ApplicationEvents is not null) APIs.ApplicationEvents.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.Applications is not null) APIs.Applications.ApiHelper.GetRefreshedToken = RefreshToken;
        if (APIs.DataModifications is not null) APIs.DataModifications.ApiHelper.GetRefreshedToken = RefreshToken;

        if (APIs.Sins is not null) APIs.Sins.ApiHelper.GetRefreshedToken = RefreshToken;

        if (APIs.ProductionAudits is not null) APIs.ProductionAudits.ApiHelper.GetRefreshedToken = RefreshToken;

        return true;
    }

    public async Task SystemLogout()
    {
        await APIs.Accounts.Logout(LoginData);
    }

    private void SetTokenForAPIs()
    {
        if (APIs.Accounts is not null) APIs.Accounts.Token = CurrentToken;
        if (APIs.TracingApplications is not null) APIs.TracingApplications.Token = CurrentToken;
        if (APIs.TracingEvents is not null) APIs.TracingEvents.Token = CurrentToken;
        if (APIs.TracingResponses is not null) APIs.TracingResponses.Token = CurrentToken;
        if (APIs.Financials is not null) APIs.Financials.Token = CurrentToken;
        if (APIs.ControlBatches is not null) APIs.ControlBatches.Token = CurrentToken;
        if (APIs.Transactions is not null) APIs.Transactions.Token = CurrentToken;

        if (APIs.InterceptionApplications is not null) APIs.InterceptionApplications.Token = CurrentToken;

        if (APIs.LicenceDenialEvents is not null) APIs.LicenceDenialEvents.Token = CurrentToken;
        if (APIs.LicenceDenialApplications is not null) APIs.LicenceDenialApplications.Token = CurrentToken;
        if (APIs.LicenceDenialTerminationApplications is not null) APIs.LicenceDenialTerminationApplications.Token = CurrentToken;
        if (APIs.LicenceDenialResponses is not null) APIs.LicenceDenialResponses.Token = CurrentToken;

        if (APIs.ApplicationEvents is not null) APIs.ApplicationEvents.Token = CurrentToken;
        if (APIs.Applications is not null) APIs.Applications.Token = CurrentToken;
        if (APIs.DataModifications is not null) APIs.DataModifications.Token = CurrentToken;

        if (APIs.Sins is not null) APIs.Sins.Token = CurrentToken;

        if (APIs.ProductionAudits is not null) APIs.ProductionAudits.Token = CurrentToken;
    }

    private async Task<string> RefreshToken()
    {
        var result = await APIs.Accounts.RefreshToken(CurrentToken, CurrentRefreshToken);

        CurrentToken = result.Token;
        CurrentRefreshToken = result.RefreshToken;

        SetTokenForAPIs();

        return CurrentToken;
    }

}
