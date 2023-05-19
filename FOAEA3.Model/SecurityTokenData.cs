using System;

namespace FOAEA3.Model
{
    public class SecurityTokenData
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string SubjectName { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Subm_Class { get; set; }
        public string FromRefreshToken { get; set; }
    }
}
