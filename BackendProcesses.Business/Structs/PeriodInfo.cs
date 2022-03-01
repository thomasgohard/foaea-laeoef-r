using BackendProcesses.Business.Enums;
using System;

namespace BackendProcesses.Business.Structs
{
    public struct PeriodInfo
    {
        public DateTime CalcAcceptedDate { get; set; }
        public EPeriodFrequency PeriodFrequency { get; set; }
        public EStartDateUsed StartDateUsed { get; set; }
    }
}
