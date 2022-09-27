using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Helpers;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileBroker.API.Account.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult> CreateToken([FromBody] LoginData loginData, 
                                        [FromServices] IConfiguration config,
                                        [FromServices] IUserRepository userDB)
        {
            var thisUser = await userDB.GetUserByNameAsync(loginData.UserName);

            if (!IsValidLogin(loginData, thisUser))
                return BadRequest();

            var apiKey = config["Tokens:Key"].ReplaceVariablesWithEnvironmentValues();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginData.UserName),
                new Claim(ClaimTypes.Role, thisUser.SecurityRole),
                new Claim(JwtRegisteredClaimNames.Sub, thisUser.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, loginData.UserName)
            };

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var encodedApiKey = Encoding.UTF8.GetBytes(apiKey);
            var securityKey = new SymmetricSecurityKey(encodedApiKey);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Tokens:Issuer"].ReplaceVariablesWithEnvironmentValues(),
                                            config["Tokens:Audience"].ReplaceVariablesWithEnvironmentValues(),
                                            claims, signingCredentials: creds,
                                            expires: DateTime.UtcNow.AddMinutes(20));

            return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
        }

        private static bool IsValidLogin(LoginData loginData, UserData thisUser)
        {            
            if (thisUser == null)
                return false;

            if (!PasswordHelper.IsValidPassword(loginData.Password, thisUser.PasswordSalt, thisUser.EncryptedPassword))
                return false;

            return true;
        }
                
    }
}
