using DBHelper;

namespace FOAEA3.Model.Interfaces
{
    public interface IRepositories
    {
        public IDBTools MainDB { get; }

        public string CurrentSubmitter { get; set; }
        public string CurrentUser { get; set; }

        public IApplicationRepository ApplicationRepository { get; }
        public IApplicationEventRepository ApplicationEventRepository { get; }
        public IApplicationEventDetailRepository ApplicationEventDetailRepository { get; }
        public ICaseManagementRepository CaseManagementRepository { get; }
        public IApplicationSearchRepository ApplicationSearchRepository { get; }
        public ISubmitterRepository SubmitterRepository { get; }
        public IEnfOffRepository EnfOffRepository { get; }
        public IEnfSrvRepository EnfSrvRepository { get; }
        public IProvinceRepository ProvinceRepository { get; }
        public IInterceptionRepository InterceptionRepository { get; }
        public ISubjectRepository SubjectRepository { get; }
        public ITracingRepository TracingRepository { get; }
        public ILicenceDenialRepository LicenceDenialRepository { get; }
        public IAffidavitRepository AffidavitRepository { get; }
        public ILoginRepository LoginRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        //public IUserProfileRepository UserProfileRepository { get; }
        public ISubjectRoleRepository SubjectRoleRepository { get; }
        public ISubmitterProfileRepository SubmitterProfileRepository { get; }
        public ISINResultRepository SINResultRepository { get; }
        public ITraceResponseRepository TraceResponseRepository { get; }
        public IProductionAuditRepository ProductionAuditRepository { get; }
        public ISINChangeHistoryRepository SINChangeHistoryRepository { get; }
        public IFamilyProvisionRepository FamilyProvisionRepository { get; }
        public IInfoBankRepository InfoBankRepository { get; }
        public IAccessAuditRepository AccessAuditRepository { get; }
        public IFailedSubmitAuditRepository FailedSubmitAuditRepository { get; }
        public IPostalCodeRepository PostalCodeRepository { get; }
    }
}
