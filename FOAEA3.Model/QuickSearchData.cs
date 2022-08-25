using FOAEA3.Model.Enums;
using System;

namespace FOAEA3.Model
{
    public class QuickSearchData
    {
        public string EnforcementService { get; set; }
        public string ControlCode { get; set; }
        public string DebtorSurname { get; set; }
        public string DebtorMiddleName { get; set; }
        public string DebtorFirstName { get; set; }
        public string Status { get; set; } 
        public ApplicationState State { get; set; } 
        public string Category { get; set; } 
        public string SIN { get; set; }
        public string ReferenceNumber { get; set; }
        public string JusticeNumber { get; set; }
        public DateTime? DebtorDateOfBirth_Start { get; set; }
        public DateTime? DebtorDateOfBirth_End { get; set; }
        public DateTime? CreatedDate_Start { get; set; }
        public DateTime? CreatedDate_End { get; set; }
        public DateTime? ActualEndDate_Start { get; set; }
        public DateTime? ActualEndDate_End { get; set; }
        public string Submitter { get; set; }
        public bool ViewAllJurisdiction { get; set; }
        public bool SearchOnlySinConfirmed { get; set; }

        public QuickSearchData()
        {
            State = ApplicationState.UNDEFINED; 
            ViewAllJurisdiction = false;
        }
    }
}
