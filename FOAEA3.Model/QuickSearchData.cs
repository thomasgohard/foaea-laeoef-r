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
        public string Status { get; set; } // possible values: A, C, I, R, P or X
        public ApplicationState State { get; set; } // possible values: 1, 2, 3, 4, 5, 6, 7, 9, 10, 12, 15, 14, 15, 19 or 35
        public string Category { get; set; } // possible values: T01, I01, L01 or L03
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
            State = ApplicationState.UNDEFINED; // default
            ViewAllJurisdiction = false;
        }
    }
}
