using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IFoaeaConfigurationHelper
    {
        List<string> ESDsites { get; }
        string FoaeaConnection { get; }
        List<string> ProductionServers { get; }
        RecipientsConfig Recipients { get; }
        TokenConfig Tokens { get; }
        DeclarationData LicenceDenialDeclaration { get; }
    }
}
