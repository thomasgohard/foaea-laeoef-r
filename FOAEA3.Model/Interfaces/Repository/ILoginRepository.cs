namespace FOAEA3.Model.Interfaces
{
    public interface ILoginRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        bool IsLoginExpired(string subjectName);
        bool CheckPreviousPasswords(int subjectId, string newPassword);
        void GetAllowedAccess(string username, ref bool IsAllowed);
        void AcceptNewTermsOfReferernce(string username);
        void SetPassword(string username, string password, int passwordFormat, string passwordSalt, int passwordExpireDays);
    }
}