using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FOAEA3.ViewModels
{
    public class TracingAffidavitDataEntryViewModel
    {
        public string Province { get; init; }
        public string Creditor { get; init; }
        public string Debtor { get; init; }
        public string Declarant { get; init; }
        public TracingApplicationData Tracing { get; init; }
        public string CurrentDate { get; init; }
        public SelectList FamilyProvisions { get; init; }
        public SelectList InfoBanks { get; init; }
        public SelectList Commissionners { get; init; }

    }
}
