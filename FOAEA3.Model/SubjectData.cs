using System;

namespace FOAEA3.Model
{
    public class SubjectData
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string defaultRoleName { get; set; }
        public int defaultRoleId { get; set; }
        public bool IsAccessingUsingRole { get; set; }
        public DateTime roleChangeDate { get; set; }
        public string currentRoleName { get; set; }
        public bool AllowedAccess { get; set; }
        public bool IsTrusted { get; set; }
        public string PhoneNumber { get; set; }
        public string EMailAddress { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string EntrustName { get; set; }
        public string Password { get; set; }
        public int? PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public bool? IsAccountLocked { get; set; }
        public int? UnsuccessfulLoginAttempts { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime? ConfirmationSent { get; set; }
        public bool? ConfirmationReceived { get; set; }
        public DateTime? PasswordSent { get; set; }
        public bool? PasswordReset { get; set; }
        public DateTime? PKIExpiration { get; set; }
        public bool HasAcceptedNewTermsOfRef { get; set; }
    }
}
