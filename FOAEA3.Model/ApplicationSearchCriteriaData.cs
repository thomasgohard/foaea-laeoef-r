using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class ApplicationSearchCriteriaData
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string EnfSourceRefNumber { get; set; }
        public ApplicationState State { get; set; } // possible values: 1, 2, 3, 4, 5, 6, 7, 9, 10, 12, 15, 14, 15, 19 or 35
        public string SIN { get; set; }
        public string ControlCode { get; set; }
        public string JusticeNumber { get; set; }
        public string Status { get; set; } // possible values: A, C, I R, P or X,
        public string Category { get; set; } // possible values: T01, I01, L01 or L03
        public string ReceiptDateFrom { get; set; }
        public string ReceiptDateTo { get; set; }
        public string ExpiryDateFrom { get; set; }
        public string ExpiryDateTo { get; set; }
        public string DateOfBirthFrom { get; set; }
        public string DateOfBirthTo { get; set; }

        public ApplicationSearchCriteriaData()
        {
            State = ApplicationState.UNDEFINED; // default
        }
    }
}
