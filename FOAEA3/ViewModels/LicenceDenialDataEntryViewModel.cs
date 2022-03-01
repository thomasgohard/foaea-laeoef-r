using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.ViewModels
{
    public class LicenceDenialDataEntryViewModel
    {
        private LicenceDenialData licenceDenialData;

        public LicenceDenialData LicenceDenial
        {
            get { return licenceDenialData; }
            set
            {
                licenceDenialData = value;
            }
        }

        public string Declarant { get; set; }
        public string CourtValue { get; set; }
        public bool CanEditCore { get; set; } = true;
        public bool CanEditCommentsAndRef { get; set; } = true;
        public bool IsCreate { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
