using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSecurityToken : DBbase, ISecurityTokenRepository
    {
        public DBSecurityToken(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task CreateAsync(SecurityTokenData securityToken)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Token", securityToken.Token},
                    {"TokenExpiration", securityToken.TokenExpiration},
                    {"RefreshToken", securityToken.RefreshToken},
                    {"RefreshTokenExpiration", securityToken.RefreshTokenExpiration},
                    {"SubjectName", securityToken.SubjectName},
                    {"Subm_SubmCd", securityToken.Subm_SubmCd},
                    {"Subm_Class", securityToken.Subm_Class}
                };

            if (!string.IsNullOrEmpty(securityToken.FromRefreshToken))
                parameters.Add("FromRefreshToken", securityToken.FromRefreshToken);

            await MainDB.ExecProcAsync("SecurityToken_Insert", parameters);
        }

        public async Task<SecurityTokenData> GetTokenDataAsync(string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Token", token}
            };

            var data = await MainDB.GetDataFromStoredProcAsync<SecurityTokenData>("SecurityToken_Select", parameters, FillDataFromReader);
            return data.FirstOrDefault();
        }

        public async Task MarkTokenAsExpired(string token)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Token", token}
            };

            await MainDB.ExecProcAsync("SecurityToken_MarkAsExpired", parameters);
        }

        private void FillDataFromReader(IDBHelperReader rdr, SecurityTokenData data)
        {
            data.Token = rdr["Token"] as string;
            data.TokenExpiration = (DateTime)rdr["TokenExpiration"];
            data.RefreshToken = rdr["RefreshToken"] as string;
            data.RefreshTokenExpiration = (DateTime)rdr["RefreshTokenExpiration"];
            data.SubjectName = rdr["SubjectName"] as string;
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Subm_Class = rdr["Subm_Class"] as string;
            data.FromRefreshToken = rdr["FromRefreshToken"] as string; // can be null 
        }
    }
}
