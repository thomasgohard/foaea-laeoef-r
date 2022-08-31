using DBHelper;
using TestData.TestDB;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDataBase
{
    public class InMemory_RepositoriesFinance : IRepositories_Finance
    {
        public IDBToolsAsync MainDB { get; }

        public InMemory_RepositoriesFinance()
        {
            SummonsSummaryRepository = new InMemorySummonsSummary();
            SummonsSummaryFixedAmountRepository = new InMemorySummonsSummaryFixedAmount();
            ActiveSummonsRepository = new InMemoryActiveSummons();
            GarnPeriodRepository = new InMemoryGarnPeriod();
            DivertFundsRepository = new InMemoryDivertFunds();
            // SummDFRepository = new InMemorySummDF();
        }

        public string CurrentSubmitter { get; set; }

        public ISummonsSummaryRepository SummonsSummaryRepository { get; }

        public IActiveSummonsRepository ActiveSummonsRepository { get; }

        public IGarnPeriodRepository GarnPeriodRepository { get; }

        public ISummonsSummaryFixedAmountRepository SummonsSummaryFixedAmountRepository { get; }

        public IDivertFundsRepository DivertFundsRepository { get; }

        public ISummDFRepository SummDFRepository { get; }

        public ISummFAFR_DERepository SummFAFR_DERepository => throw new NotImplementedException();

        public ISummFAFRRepository SummFAFRRepository => throw new NotImplementedException();

        public IControlBatchRepository ControlBatchRepository => throw new NotImplementedException();

        public IGarnSummaryRepository GarnSummaryRepository => throw new NotImplementedException();
    }
}
