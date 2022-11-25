using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IFoaeaConfigurationHelper
    {
        string FoaeaConnection { get; }
        RecipientsConfig Recipients { get; }
        TokenConfig Tokens { get; }
        DeclarationData LicenceDenialDeclaration { get; }
        List<string> ProductionServers { get; }
        public List<string> AutoSwear { get; }
        public List<string> AutoAccept { get; }
        List<string> ESDsites { get; }
    }
}
