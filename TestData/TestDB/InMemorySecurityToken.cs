using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySecurityToken : ISecurityTokenRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        private List<SecurityTokenData> _data = new List<SecurityTokenData>();

        public Task CreateAsync(SecurityTokenData securityToken)
        {
            _data.Add(securityToken);
            return Task.CompletedTask;
        }

        public Task<SecurityTokenData> GetTokenDataAsync(string token)
        {
            var thisToken = _data.Where(m => m.Token == token).FirstOrDefault();
            return Task.FromResult(thisToken);
        }

        public Task MarkTokenAsExpired(string token)
        {
            var thisToken = _data.Where(m => m.Token == token).FirstOrDefault();
            if (thisToken != null)
                thisToken.RefreshTokenExpiration = DateTime.Now;
            return Task.CompletedTask;
        }
    }
}
