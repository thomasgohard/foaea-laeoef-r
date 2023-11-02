using DBHelper;

namespace FileBroker.Business.Helpers
{
    public class IncomingProvincialHelper
    {
        private IFileBrokerConfigurationHelper Config { get; }

        private string ProvCode { get; }

        public IncomingProvincialHelper(IFileBrokerConfigurationHelper config, string provCode)
        {
            Config = config;
            ProvCode = provCode;
        }

        public bool IsValidTermsAccepted(string termsAccepted)
        {
            bool result;

            if (ProvCode.ToUpper().In("NF", "NL"))
            {
                result = string.Equals(Config.TermsAcceptedTextEnglish.Replace("{prv}", "NF"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(Config.TermsAcceptedTextEnglish.Replace("{prv}", "NL"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(Config.TermsAcceptedTextFrench.Replace("{prv}", "NF"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(Config.TermsAcceptedTextFrench.Replace("{prv}", "NL"), termsAccepted, StringComparison.InvariantCultureIgnoreCase);
            }
            else
                result = string.Equals(Config.TermsAcceptedTextEnglish.Replace("{prv}", ProvCode), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(Config.TermsAcceptedTextFrench.Replace("{prv}", ProvCode), termsAccepted, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

    }
}
