using DBHelper;
using FOAEA3.Data.DB;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Data.Base
{
    public class DbRepositories_Finance : IRepositories_Finance
    {
        public IDBTools MainDB { get; }
        private ISummonsSummaryRepository summonsSummaryDB;
        private IActiveSummonsRepository activeSummonsDB;
        private IGarnPeriodRepository garnPeriodDB;
        private IGarnSummaryRepository garnSummaryDB;
        private IDivertFundsRepository divertFundsDB;
        private ISummonsSummaryFixedAmountRepository summonsSummaryFixedAmountDB;
        private ISummDFRepository summDFDB;
        private ISummFAFR_DERepository summFAFR_DEDB;
        private ISummFAFRRepository summFAFRDB;
        private IControlBatchRepository controlBatchDB;

        public string CurrentSubmitter
        {
            get => MainDB.Submitter;
            set => MainDB.Submitter = value;
        }

        public string CurrentUser
        {
            get => MainDB.UserId;
            set => MainDB.UserId = value;
        }

        public DbRepositories_Finance(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public ISummonsSummaryRepository SummonsSummaryRepository
        {
            get
            {
                if (summonsSummaryDB is null)
                    summonsSummaryDB = new DBSummonsSummary(MainDB);
                return summonsSummaryDB;
            }
        }

        public ISummonsSummaryFixedAmountRepository SummonsSummaryFixedAmountRepository
        {
            get
            {
                if (summonsSummaryFixedAmountDB is null)
                    summonsSummaryFixedAmountDB = new DBSummonsSummaryFixedAmount(MainDB);
                return summonsSummaryFixedAmountDB;
            }
        }

        public IActiveSummonsRepository ActiveSummonsRepository
        {
            get
            {
                if (activeSummonsDB is null)
                    activeSummonsDB = new DBActiveSummons(MainDB);
                return activeSummonsDB;
            }
        }

        public IGarnPeriodRepository GarnPeriodRepository
        {
            get
            {
                if (garnPeriodDB is null)
                    garnPeriodDB = new DBGarnPeriod(MainDB);
                return garnPeriodDB;
            }
        }

        public IGarnSummaryRepository GarnSummaryRepository
        {
            get
            {
                if (garnSummaryDB is null)
                    garnSummaryDB = new DBGarnSummary(MainDB);
                return garnSummaryDB;
            }
        }

        public IDivertFundsRepository DivertFundsRepository
        {
            get
            {
                if (divertFundsDB is null)
                    divertFundsDB = new DBDivertFunds(MainDB);
                return divertFundsDB;
            }
        }

        public ISummDFRepository SummDFRepository
        {
            get
            {
                if (summDFDB is null)
                    summDFDB = new DBSummDF(MainDB);
                return summDFDB;
            }
        }

        public ISummFAFR_DERepository SummFAFR_DERepository
        {
            get
            {
                if (summFAFR_DEDB is null)
                    summFAFR_DEDB = new DBSummFAFR_DE(MainDB);
                return summFAFR_DEDB;
            }
        }

        public ISummFAFRRepository SummFAFRRepository
        {
            get
            {
                if (summFAFRDB is null)
                    summFAFRDB = new DBSummFAFR(MainDB);
                return summFAFRDB;
            }
        }

        public IControlBatchRepository ControlBatchRepository
        {
            get
            {
                if (controlBatchDB is null)
                    controlBatchDB = new DBControlBatch(MainDB);
                return controlBatchDB;
            }
        }
    }
}
