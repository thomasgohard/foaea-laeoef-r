using System;

namespace FileBroker.Model
{
    public class SecurityTokenData
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public int UserId { get; set; }
        public string SecurityRole { get; set; }
        public string EmailAddress { get; set; }
        public string FromRefreshToken { get; set; }
    }
}
