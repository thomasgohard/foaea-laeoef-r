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

        public ISINResultRepository SINResultRepository
        {
            get
            {
                if (sinResultDB is null)
                    sinResultDB = new DBSINResult(MainDB);
                return sinResultDB;
            }
        }

        public ITraceResponseRepository TraceResponseRepository
        {
            get
            {
                if (traceResponseDB is null)
                    traceResponseDB = new DBTraceResponse(MainDB);
                return traceResponseDB;
            }
        }

        public ILicenceDenialResponseRepository LicenceDenialResponseRepository
        {
            get
            {
                if (licenceDenialResponseDB is null)
                    licenceDenialResponseDB = new DBLicenceDenialResponse(MainDB);
                return licenceDenialResponseDB;
            }
        }

        public IApplicationRepository ApplicationRepository
        {
            get
            {
                if (applicationDB is null)
                    applicationDB = new DBApplication(MainDB);
                return applicationDB;
            }
        }

        public IApplicationCommentsRepository ApplicationCommentsRepository
        {
            get
            {
                if (applicationCommentsDB is null)
                    applicationCommentsDB = new DBApplicationComments(MainDB);
                return applicationCommentsDB;
            }
        }

        public IApplicationEventRepository ApplicationEventRepository
        {
            get
            {
                if (applicationEventDB is null)
                    applicationEventDB = new DBApplicationEvent(MainDB);
                return applicationEventDB;
            }
        }

        public IApplicationEventDetailRepository ApplicationEventDetailRepository
        {
            get
            {
                if (applicationEventDetailDB is null)
                    applicationEventDetailDB = new DBApplicationEventDetail(MainDB);
                return applicationEventDetailDB;
            }
        }

        public ICaseManagementRepository CaseManagementRepository
        {
            get
            {
                if (caseManagementDB is null)
                    caseManagementDB = new DBCaseManagement(MainDB);
                return caseManagementDB;
            }
        }

        public IApplicationSearchRepository ApplicationSearchRepository
        {
            get
            {
                if (applicationSearchDB is null)
                    applicationSearchDB = new DBApplicationSearch(MainDB);
                return applicationSearchDB;
            }
        }

        public ISubmitterRepository SubmitterRepository
        {
            get
            {
                if (submitterDB is null)
                    submitterDB = new DBSubmitter(MainDB);
                return submitterDB;
            }
        }

        public IEnfOffRepository EnfOffRepository
        {
            get
            {
                if (enfOffDB is null)
                    enfOffDB = new DBEnfOff(MainDB);
                return enfOffDB;
            }
        }

        public IEnfSrvRepository EnfSrvRepository
        {
            get
            {
                if (enfSrvDB is null)
                    enfSrvDB = new DBEnfSrv(MainDB);
                return enfSrvDB;
            }
        }

        public IProvinceRepository ProvinceRepository
        {
            get
            {
                if (provinceDB is null)
                    provinceDB = new DBProvince(MainDB);
                return provinceDB;
            }
        }

        public ISubjectRepository SubjectRepository
        {
            get
            {
                if (subjectDB is null)
                    subjectDB = new DBSubject(MainDB);
                return subjectDB;
            }
        }

        public IInterceptionRepository InterceptionRepository
        {
            get
            {
                if (interceptionDB is null)
                    interceptionDB = new DBInterception(MainDB);
                return interceptionDB;
            }
        }

        public ITracingRepository TracingRepository
        {
            get
            {
                if (tracingDB is null)
                    tracingDB = new DBTracing(MainDB);
                return tracingDB;
            }
        }

        public ILicenceDenialRepository LicenceDenialRepository
        {
            get
            {
                if (licenceDenialDB is null)
                    licenceDenialDB = new DBLicenceDenial(MainDB);
                return licenceDenialDB;
            }
        }

        public IAffidavitRepository AffidavitRepository
        {
            get
            {
                if (affidavitDB is null)
                    affidavitDB = new DBAffidavit(MainDB);
                return affidavitDB;
            }
        }
        public ILoginRepository LoginRepository
        {
            get
            {
                if (loginDB is null)
                    loginDB = new DBLogin(MainDB);
                return loginDB;
            }
        }

        public INotificationRepository NotificationRepository
        {
            get
            {
                if (notificationDB is null)
                    notificationDB = new DBNotification(MainDB);
                return notificationDB;
            }
        }

        public ISubmitterProfileRepository SubmitterProfileRepository
        {
            get
            {
                if (submitterProfileDB is null)
                    submitterProfileDB = new DBSubmitterProfile(MainDB);
                return submitterProfileDB;
            }
        }

        public ISubjectRoleRepository SubjectRoleRepository
        {
            get
            {
                if (subjectRoleDB is null)
                    subjectRoleDB = new DBSubjectRole(MainDB);
                return subjectRoleDB;
            }
        }

        public IProductionAuditRepository ProductionAuditRepository
        {
            get
            {
                if (productionAuditDB is null)
                    productionAuditDB = new DBProductionAudit(MainDB);
                return productionAuditDB;
            }
        }

        public ISINChangeHistoryRepository SINChangeHistoryRepository
        {
            get
            {
                if (sinChangeHistoryDB is null)
                    sinChangeHistoryDB = new DBSINChangeHistory(MainDB);
                return sinChangeHistoryDB;
            }
        }

        public IFamilyProvisionRepository FamilyProvisionRepository
        {
            get
            {
                if (familyProvisionDB is null)
                    familyProvisionDB = new DBFamilyProvision(MainDB);
                return familyProvisionDB;
            }
        }

        public IInfoBankRepository InfoBankRepository
        {
            get
            {
                if (infoBankDB is null)
                    infoBankDB = new DBInfoBank(MainDB);
                return infoBankDB;
            }
        }

        public IAccessAuditRepository AccessAuditRepository
        {
            get
            {
                if (accessAuditDB is null)
                    accessAuditDB = new DBAccessAudit(MainDB);
                return accessAuditDB;
            }
        }

        public IFailedSubmitAuditRepository FailedSubmitAuditRepository
        {
            get
            {
                if (failedSubmitAuditDB is null)
                    failedSubmitAuditDB = new DBFailedSubmitAudit(MainDB);
                return failedSubmitAuditDB;
            }
        }

        public IPostalCodeRepository PostalCodeRepository
        {
            get
            {
                if (postalCodeDB is null)
                    postalCodeDB = new DBPostalCode(MainDB);
                return postalCodeDB;
            }
        }
    }
}
