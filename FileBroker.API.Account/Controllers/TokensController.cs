using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Helpers;
using FOAEA3.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FileBrokerModel = FileBroker.Model;

namespace FileBroker.API.Account.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult> CreateToken([FromBody] FileBrokerModel.LoginData loginData,
                                                    [FromServices] IConfiguration config,
                                                    [FromServices] IUserRepository userDB)
        {
            var thisUser = await userDB.GetUserByNameAsync(loginData.UserName);

            if (!IsValidLogin(loginData, thisUser))
                return BadRequest();

            string apiKey = config["Tokens:Key"].ReplaceVariablesWithEnvironmentValues();
            string issuer = config["Tokens:Issuer"].ReplaceVariablesWithEnvironmentValues();
            string audience = config["Tokens:Audience"].ReplaceVariablesWithEnvironmentValues();
            if (!int.TryParse(config["Tokens:ExpireMinutes"], out int expireMinutes))
                expireMinutes = 20;

            return Created("", CreateNewToken(apiKey, issuer, audience, expireMinutes, thisUser));
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRefreshData refreshData,
                                                     [FromServices] IConfiguration config,
                                                     [FromServices] IUserRepository userDB)
        {
            // TODO: fix this
            //var thisUser = await userDB.GetUserByNameAsync(refreshData.UserName);

            //if (thisUser is not null && String.Equals(thisUser.RefreshToken, refreshData.RefreshToken) && thisUser.RefreshTokenExpiration > DateTime.Now)
            //{
            //    string apiKey = config["Tokens:Key"].ReplaceVariablesWithEnvironmentValues();
            //    string issuer = config["Tokens:Issuer"].ReplaceVariablesWithEnvironmentValues();
            //    string audience = config["Tokens:Audience"].ReplaceVariablesWithEnvironmentValues();
            //    if (!int.TryParse(config["Tokens:ExpireMinutes"], out int expireMinutes))
            //        expireMinutes = 20;

            //    return Created("", CreateNewToken(apiKey, issuer, audience, expireMinutes, thisUser));
            //}
            //else
                return BadRequest();
        }

        private static bool IsValidLogin(FileBrokerModel.LoginData loginData, FileBrokerModel.UserData thisUser)
        {
            if (thisUser == null)
                return false;

            if (!PasswordHelper.IsValidPassword(loginData.Password, thisUser.PasswordSalt, thisUser.EncryptedPassword))
                return false;

            return true;
        }

        private static TokenData CreateNewToken(string apiKey, string issuer, string audience, int expireMinutes, UserData userData)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userData.UserName),
                new Claim(ClaimTypes.Role, userData.SecurityRole),
                new Claim(JwtRegisteredClaimNames.Sub, userData.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userData.UserName)
            };

            JwtSecurityToken token = SecurityTokenHelper.GenerateToken(issuer, audience, expireMinutes, apiKey, claims);
            string refreshToken = SecurityTokenHelper.GenerateRefreshToken();

            return new TokenData
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                TokenExpiration = token.ValidTo
            };
        }

    }
}
