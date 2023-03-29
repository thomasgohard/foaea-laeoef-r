using DBHelper;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IRepositories_Finance
    {
        public IDBToolsAsync MainDB { get; }

        public string CurrentSubmitter { get; set; }

        public ISummonsSummaryRepository SummonsSummaryRepository { get; }
        public ISummonsSummaryFixedAmountRepository SummonsSummaryFixedAmountRepository { get; }
        public IActiveSummonsRepository ActiveSummonsRepository { get; }
        public IGarnPeriodRepository GarnPeriodRepository { get; }
        public IGarnSummaryRepository GarnSummaryRepository { get; }
        public IDivertFundsRepository DivertFundsRepository { get; }
        public ISummDFRepository SummDFRepository { get; }
        public ISummFAFR_DERepository SummFAFR_DERepository { get; }
        public ISummFAFRRepository SummFAFRRepository { get; }
        public IControlBatchRepository ControlBatchRepository { get; }
        public IFinancialRepository FinancialRepository { get; }
    }
}
