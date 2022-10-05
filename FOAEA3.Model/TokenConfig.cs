using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model
{
    public class TokenConfig
    {
        private string key;
        private string issuer;
        private string audience;
        private int expireMinutes;
        private int refreshExpireMinutes;

        public string Key
        {
            get => key;
            set => key = value.ReplaceVariablesWithEnvironmentValues();
        }

        public string Issuer
        {
            get => issuer;
            set => issuer = value.ReplaceVariablesWithEnvironmentValues();
        }

        public string Audience
        {
            get => audience;
            set => audience = value.ReplaceVariablesWithEnvironmentValues();
        }

        public int ExpireMinutes
        {
            get => expireMinutes;
            set => expireMinutes = value;
        }

        public int RefreshTokenExpireMinutes
        {
            get => refreshExpireMinutes;
            set => refreshExpireMinutes = value;
        }
    }

}
