using DBHelper;
using FOAEA3.Data.DB;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Data.Base
{
    public class DbRepositories_Finance : IRepositories_Finance
    {
        public IDBToolsAsync MainDB { get; }
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
        private IPADRRepository padrDB;

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

        public DbRepositories_Finance(IDBToolsAsync mainDB)
        {
            MainDB = mainDB;
        }

        public ISummonsSummaryRepository SummonsSummaryRepository
        {
            get
            {
                summonsSummaryDB ??= new DBSummonsSummary(MainDB);
                return summonsSummaryDB;
            }
        }

        public ISummonsSummaryFixedAmountRepository SummonsSummaryFixedAmountRepository
        {
            get
            {
                summonsSummaryFixedAmountDB ??= new DBSummonsSummaryFixedAmount(MainDB);
                return summonsSummaryFixedAmountDB;
            }
        }

        public IActiveSummonsRepository ActiveSummonsRepository
        {
            get
            {
                activeSummonsDB ??= new DBActiveSummons(MainDB);
                return activeSummonsDB;
            }
        }

        public IGarnPeriodRepository GarnPeriodRepository
        {
            get
            {
                garnPeriodDB ??= new DBGarnPeriod(MainDB);
                return garnPeriodDB;
            }
        }

        public IGarnSummaryRepository GarnSummaryRepository
        {
            get
            {
                garnSummaryDB ??= new DBGarnSummary(MainDB);
                return garnSummaryDB;
            }
        }

        public IDivertFundsRepository DivertFundsRepository
        {
            get
            {
                divertFundsDB ??= new DBDivertFunds(MainDB);
                return divertFundsDB;
            }
        }

        public ISummDFRepository SummDFRepository
        {
            get
            {
                summDFDB ??= new DBSummDF(MainDB);
                return summDFDB;
            }
        }

        public ISummFAFR_DERepository SummFAFR_DERepository
        {
            get
            {
                summFAFR_DEDB ??= new DBSummFAFR_DE(MainDB);
                return summFAFR_DEDB;
            }
        }

        public ISummFAFRRepository SummFAFRRepository
        {
            get
            {
                summFAFRDB ??= new DBSummFAFR(MainDB);
                return summFAFRDB;
            }
        }

        public IControlBatchRepository ControlBatchRepository
        {
            get
            {
                controlBatchDB ??= new DBControlBatch(MainDB);
                return controlBatchDB;
            }
        }

        public IPADRRepository PADRrepository
        {
            get
            {
                padrDB ??= new DBPADR(MainDB);
                return padrDB;
            }
        }
    }
}
