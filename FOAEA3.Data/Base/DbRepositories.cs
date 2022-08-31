using DBHelper;
using FOAEA3.Data.DB;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Data.Base
{
    public class DbRepositories : IRepositories
    {
        private IApplicationRepository applicationDB;
        private IApplicationEventRepository applicationEventDB;
        private IApplicationEventDetailRepository applicationEventDetailDB;
        private ICaseManagementRepository caseManagementDB;
        private IApplicationSearchRepository applicationSearchDB;
        private IApplicationCommentsRepository applicationCommentsDB;
        private ISubmitterRepository submitterDB;
        private IEnfOffRepository enfOffDB;
        private IEnfSrvRepository enfSrvDB;
        private IProvinceRepository provinceDB;
        private ISubjectRepository subjectDB;
        private IInterceptionRepository interceptionDB;
        private ITracingRepository tracingDB;
        private ITraceResponseRepository traceResponseDB;
        private ILicenceDenialRepository licenceDenialDB;
        private ILicenceDenialResponseRepository licenceDenialResponseDB;
        private IAffidavitRepository affidavitDB;
        private ILoginRepository loginDB;
        private INotificationRepository notificationDB;
        private ISubmitterProfileRepository submitterProfileDB;
        private ISubjectRoleRepository subjectRoleDB;
        private ISINResultRepository sinResultDB;
        private IProductionAuditRepository productionAuditDB;
        private ISINChangeHistoryRepository sinChangeHistoryDB;
        private IFamilyProvisionRepository familyProvisionDB;
        private IInfoBankRepository infoBankDB;
        private IAccessAuditRepository accessAuditDB;
        private IFailedSubmitAuditRepository failedSubmitAuditDB;
        private IPostalCodeRepository postalCodeDB;

        public IDBToolsAsync MainDB { get; }

        //private string currentSubmitter = string.Empty;

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

        public DbRepositories(IDBToolsAsync mainDB)
        {
            MainDB = mainDB;
        }

        public ISINResultRepository SINResultTable
        {
            get
            {
                sinResultDB ??= new DBSINResult(MainDB);
                return sinResultDB;
            }
        }

        public ITraceResponseRepository TraceResponseTable
        {
            get
            {
                traceResponseDB ??= new DBTraceResponse(MainDB);
                return traceResponseDB;
            }
        }

        public ILicenceDenialResponseRepository LicenceDenialResponseTable
        {
            get
            {
                licenceDenialResponseDB ??= new DBLicenceDenialResponse(MainDB);
                return licenceDenialResponseDB;
            }
        }

        public IApplicationRepository ApplicationTable
        {
            get
            {
                applicationDB ??= new DBApplication(MainDB);
                return applicationDB;
            }
        }

        public IApplicationCommentsRepository ApplicationCommentsTable
        {
            get
            {
                applicationCommentsDB ??= new DBApplicationComments(MainDB);
                return applicationCommentsDB;
            }
        }

        public IApplicationEventRepository ApplicationEventTable
        {
            get
            {
                applicationEventDB ??= new DBApplicationEvent(MainDB);
                return applicationEventDB;
            }
        }

        public IApplicationEventDetailRepository ApplicationEventDetailTable
        {
            get
            {
                applicationEventDetailDB ??= new DBApplicationEventDetail(MainDB);
                return applicationEventDetailDB;
            }
        }

        public ICaseManagementRepository CaseManagementTable
        {
            get
            {
                caseManagementDB ??= new DBCaseManagement(MainDB);
                return caseManagementDB;
            }
        }

        public IApplicationSearchRepository ApplicationSearchTable
        {
            get
            {
                applicationSearchDB ??= new DBApplicationSearch(MainDB);
                return applicationSearchDB;
            }
        }

        public ISubmitterRepository SubmitterTable
        {
            get
            {
                submitterDB ??= new DBSubmitter(MainDB);
                return submitterDB;
            }
        }

        public IEnfOffRepository EnfOffTable
        {
            get
            {
                enfOffDB ??= new DBEnfOff(MainDB);
                return enfOffDB;
            }
        }

        public IEnfSrvRepository EnfSrvTable
        {
            get
            {
                enfSrvDB ??= new DBEnfSrv(MainDB);
                return enfSrvDB;
            }
        }

        public IProvinceRepository ProvinceTable
        {
            get
            {
                provinceDB ??= new DBProvince(MainDB);
                return provinceDB;
            }
        }

        public ISubjectRepository SubjectTable
        {
            get
            {
                subjectDB ??= new DBSubject(MainDB);
                return subjectDB;
            }
        }

        public IInterceptionRepository InterceptionTable
        {
            get
            {
                interceptionDB ??= new DBInterception(MainDB);
                return interceptionDB;
            }
        }

        public ITracingRepository TracingTable
        {
            get
            {
                tracingDB ??= new DBTracing(MainDB);
                return tracingDB;
            }
        }

        public ILicenceDenialRepository LicenceDenialTable
        {
            get
            {
                licenceDenialDB ??= new DBLicenceDenial(MainDB);
                return licenceDenialDB;
            }
        }

        public IAffidavitRepository AffidavitTable
        {
            get
            {
                affidavitDB ??= new DBAffidavit(MainDB);
                return affidavitDB;
            }
        }
        public ILoginRepository LoginTable
        {
            get
            {
                loginDB ??= new DBLogin(MainDB);
                return loginDB;
            }
        }

        public INotificationRepository NotificationTable
        {
            get
            {
                notificationDB ??= new DBNotification(MainDB);
                return notificationDB;
            }
        }

        public ISubmitterProfileRepository SubmitterProfileTable
        {
            get
            {
                submitterProfileDB ??= new DBSubmitterProfile(MainDB);
                return submitterProfileDB;
            }
        }

        public ISubjectRoleRepository SubjectRoleTable
        {
            get
            {
                subjectRoleDB ??= new DBSubjectRole(MainDB);
                return subjectRoleDB;
            }
        }

        public IProductionAuditRepository ProductionAuditTable
        {
            get
            {
                productionAuditDB ??= new DBProductionAudit(MainDB);
                return productionAuditDB;
            }
        }

        public ISINChangeHistoryRepository SINChangeHistoryTable
        {
            get
            {
                sinChangeHistoryDB ??= new DBSINChangeHistory(MainDB);
                return sinChangeHistoryDB;
            }
        }

        public IFamilyProvisionRepository FamilyProvisionTable
        {
            get
            {
                familyProvisionDB ??= new DBFamilyProvision(MainDB);
                return familyProvisionDB;
            }
        }

        public IInfoBankRepository InfoBankTable
        {
            get
            {
                infoBankDB ??= new DBInfoBank(MainDB);
                return infoBankDB;
            }
        }

        public IAccessAuditRepository AccessAuditTable
        {
            get
            {
                accessAuditDB ??= new DBAccessAudit(MainDB);
                return accessAuditDB;
            }
        }

        public IFailedSubmitAuditRepository FailedSubmitAuditTable
        {
            get
            {
                failedSubmitAuditDB ??= new DBFailedSubmitAudit(MainDB);
                return failedSubmitAuditDB;
            }
        }

        public IPostalCodeRepository PostalCodeTable
        {
            get
            {
                postalCodeDB ??= new DBPostalCode(MainDB);
                return postalCodeDB;
            }
        }
    }
}
