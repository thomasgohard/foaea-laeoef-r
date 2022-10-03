using System;

namespace FOAEA3.Model
{
    public class TokenData
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
