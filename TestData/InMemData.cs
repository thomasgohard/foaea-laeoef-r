using FOAEA3.Model;
using System;
using System.Collections.Generic;

namespace TestData
{
    public static class InMemData
    {
        public static List<string> ProductionAuditData { get; } = new List<string>();
        
        public static List<SummonsSummaryData> SummSmryTestData { get; } = new List<SummonsSummaryData>();
        public static List<ApplicationData> ApplicationTestData { get; } = new List<ApplicationData>();
        public static List<InterceptionFinancialHoldbackData> IntFinHoldbackTestData { get; } = new List<InterceptionFinancialHoldbackData>();
        public static List<GarnPeriodData> GarnPeriodTestData { get; } = new List<GarnPeriodData>();
        public static List<SummFAFR_Data> SummFAFRTestData { get; } = new List<SummFAFR_Data>();
        public static List<SummDF_Data> SummDFTestData { get; } = new List<SummDF_Data>();
    }
}
