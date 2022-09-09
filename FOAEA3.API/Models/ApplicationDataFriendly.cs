namespace FOAEA3.API.Model
{
    public class ApplicationDataFriendly
    {
        public string EnforcementServiceCode { get; set; }
        public string ControlCode { get; set; }
        public string SourceReferenceNumber { get; set; }
        public string JusticeNumber { get; set; }
        public string Submitter { get; set; }
        public string RecipientSubmitter { get; set; }
        public string LegalDate { get; set; }
        public string FormReceiptDate { get; set; }
        public string AffidavitSubmitter { get; set; }
        public string AffidavitReceivedDate { get; set; }

        public string CreditorName { get; set; }
        public string CreditorDateOfBirth { get; set; }
        public string DebtorName { get; set; }
        public string DebtorDateOfBirth { get; set; }
        public string DebtorAddress { get; set; }
        public string DebtorParentSurname { get; set; }
        public string DebtorLanguage { get; set; }
        public string DebtorGender { get; set; }
        public string DebtorEnteredSIN { get; set; }
        public string DebtorConfirmedSIN { get; set; }

        public string ApplicationCategory { get; set; }
        public string ApplicationLifeState { get; set; }
        public string Comments { get; set; }
        public string Created { get; set; }
        public string LastUpdated { get; set; }
        public string ActiveState { get; set; }
    }
}
