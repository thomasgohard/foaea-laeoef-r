using DBHelper;
using Microsoft.Extensions.Configuration;

namespace FileBroker.Business.Helpers
{
    internal class IncomingProvincialHelper
    {
        private string TermsAcceptedTextEnglish { get; }
        private string TermsAcceptedTextFrench { get; }
        private string ProvCode { get; }

        public IncomingProvincialHelper(IConfiguration config, string provCode)
        {
            TermsAcceptedTextEnglish = config["Declaration:TermsAccepted:English"];
            TermsAcceptedTextFrench = config["Declaration:TermsAccepted:French"];
            ProvCode = provCode;
        }

        public bool IsValidTermsAccepted(string termsAccepted)
        {
            bool result;

            if (ProvCode.ToUpper().In("NF", "NL"))
            {
                result = string.Equals(TermsAcceptedTextEnglish.Replace("{prv}", "NF"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(TermsAcceptedTextEnglish.Replace("{prv}", "NL"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(TermsAcceptedTextFrench.Replace("{prv}", "NF"), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(TermsAcceptedTextFrench.Replace("{prv}", "NL"), termsAccepted, StringComparison.InvariantCultureIgnoreCase);
            }
            else
                result = string.Equals(TermsAcceptedTextEnglish.Replace("{prv}", ProvCode), termsAccepted, StringComparison.InvariantCultureIgnoreCase) ||
                         string.Equals(TermsAcceptedTextFrench.Replace("{prv}", ProvCode), termsAccepted, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

    }
}
