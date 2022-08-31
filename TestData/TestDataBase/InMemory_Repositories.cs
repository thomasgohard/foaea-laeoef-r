using DBHelper;
using TestData.TestDB;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;

namespace TestData.TestDataBase
{
    public class InMemory_Repositories : IRepositories
    {
        public IDBToolsAsync MainDB { get; }

        public string CurrentSubmitter { get; set; }

        public string CurrentUser { get; set; }

        public IApplicationRepository ApplicationRepository { get; }

        public IApplicationEventRepository ApplicationEventRepository { get; }

        public IApplicationSearchRepository ApplicationSearchRepository { get; }

        public ISubmitterRepository SubmitterRepository { get; }

        public IEnfOffRepository EnfOffRepository { get; }

        public IEnfSrvRepository EnfSrvRepository { get; }

        public IProvinceRepository ProvinceRepository { get; }

        public ISubjectRepository SubjectRepository { get; }

        public ITracingRepository TracingRepository { get; }

        public IAffidavitRepository AffidavitRepository { get; }

        public ILoginRepository LoginRepository { get; }

        public ISubmitterProfileRepository SubmitterProfileRepository { get; }

        public ISubjectRoleRepository SubjectRoleRepository { get; }

        public ILicenceDenialRepository LicenceDenialRepository => throw new System.NotImplementedException();

        public ISINResultRepository SINResultRepository => throw new System.NotImplementedException();

        public ITraceResponseRepository TraceResponseRepository => throw new System.NotImplementedException();

        public IProductionAuditRepository ProductionAuditRepository { get; }

        public ISINChangeHistoryRepository SINChangeHistoryRepository => throw new System.NotImplementedException();

        public IFamilyProvisionRepository FamilyProvisionRepository => throw new System.NotImplementedException();

        public IInfoBankRepository InfoBankRepository => throw new System.NotImplementedException();

        public IInterceptionRepository InterceptionRepository => throw new System.NotImplementedException();

        public INotificationRepository NotificationRepository => throw new System.NotImplementedException();

        public IApplicationEventDetailRepository ApplicationEventDetailRepository => throw new System.NotImplementedException();

        public ICaseManagementRepository CaseManagementRepository => throw new System.NotImplementedException();

        public IAccessAuditRepository AccessAuditRepository => throw new System.NotImplementedException();

        public IFailedSubmitAuditRepository FailedSubmitAuditRepository => throw new System.NotImplementedException();

        public IPostalCodeRepository PostalCodeRepository => throw new System.NotImplementedException();

        public ILicenceDenialResponseRepository LicenceDenialResponseRepository => throw new System.NotImplementedException();

        public InMemory_Repositories()
        {
            MainDB = new InMemory_MainDB();

            CurrentSubmitter = "ON2D68";

            ApplicationRepository = new InMemoryApplication();
            ApplicationEventRepository = new InMemoryApplicationEvent();
            // EventSINDetailRepository = new InMemoryEventSINDetail();
            ApplicationSearchRepository = new InMemoryApplicationSearch();
            SubmitterRepository = new InMemorySubmitter();
            EnfOffRepository = new InMemoryEnfOff();
            EnfSrvRepository = new InMemoryEnfSrv();
            ProvinceRepository = new InMemoryProvince();
            SubjectRepository = new InMemorySubject();
            TracingRepository = new InMemoryTracing();
            AffidavitRepository = new InMemoryAffidavit();
            LoginRepository = new InMemoryLogin();
            SubmitterProfileRepository = new InMemorySubmitterProfile();
            SubjectRoleRepository = new InMemorySubjectRole();
            ProductionAuditRepository = new InMemoryProductionAudit();
        }
    }
}
