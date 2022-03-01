using FOAEA3.Model.Enums;
using System;

namespace FOAEA3.Model
{
    public class QuickSearchData
    {
        public string EnfService { get; set; }
        public string ControlCode { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string Status { get; set; } // possible values: A, C, I, R, P or X
        public ApplicationState State { get; set; } // possible values: 1, 2, 3, 4, 5, 6, 7, 9, 10, 12, 15, 14, 15, 19 or 35
        public string Category { get; set; } // possible values: T01, I01, L01 or L03
        public string SIN { get; set; }
        public string EnfSourceRefNumber { get; set; }
        public string JusticeNumber { get; set; }
        public DateTime? Appl_Dbtr_Brth_Dte_Start { get; set; }
        public DateTime? Appl_Dbtr_Brth_Dte_End { get; set; }
        public DateTime? Appl_Create_Dte_Start { get; set; }
        public DateTime? Appl_Create_Dte_End { get; set; }
        public DateTime? ActualEnd_Dte_Start { get; set; }
        public DateTime? ActualEnd_Dte_End { get; set; }
        public string Subm_SubmCd { get; set; }
        public bool ViewAllJurisdiction { get; set; }
        public bool SearchOnlySinConfirmed { get; set; }

        public QuickSearchData()
        {
            State = ApplicationState.UNDEFINED; // default
            ViewAllJurisdiction = false;
        }
    }
}
