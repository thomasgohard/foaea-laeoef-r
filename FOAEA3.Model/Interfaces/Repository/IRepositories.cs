using DBHelper;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Model.Interfaces
{
    public interface IRepositories
    {
        IDBToolsAsync MainDB { get; }

        string CurrentSubmitter { get; set; }
        string CurrentUser { get; set; }

        IApplicationRepository ApplicationRepository { get; }
        IApplicationEventRepository ApplicationEventRepository { get; }
        IApplicationEventDetailRepository ApplicationEventDetailRepository { get; }
        ICaseManagementRepository CaseManagementRepository { get; }
        IApplicationSearchRepository ApplicationSearchRepository { get; }
        ISubmitterRepository SubmitterRepository { get; }
        IEnfOffRepository EnfOffRepository { get; }
        IEnfSrvRepository EnfSrvRepository { get; }
        IProvinceRepository ProvinceRepository { get; }
        IInterceptionRepository InterceptionRepository { get; }
        ISubjectRepository SubjectRepository { get; }
        ITracingRepository TracingRepository { get; }
        ITraceResponseRepository TraceResponseRepository { get; }
        ILicenceDenialRepository LicenceDenialRepository { get; }
        ILicenceDenialResponseRepository LicenceDenialResponseRepository { get; }
        IAffidavitRepository AffidavitRepository { get; }
        ILoginRepository LoginRepository { get; }
        INotificationRepository NotificationRepository { get; }
        //IUserProfileRepository UserProfileRepository { get; }
        ISubjectRoleRepository SubjectRoleRepository { get; }
        ISubmitterProfileRepository SubmitterProfileRepository { get; }
        ISINResultRepository SINResultRepository { get; }
        IProductionAuditRepository ProductionAuditRepository { get; }
        ISINChangeHistoryRepository SINChangeHistoryRepository { get; }
        IFamilyProvisionRepository FamilyProvisionRepository { get; }
        IInfoBankRepository InfoBankRepository { get; }
        IAccessAuditRepository AccessAuditRepository { get; }
        IFailedSubmitAuditRepository FailedSubmitAuditRepository { get; }
        IPostalCodeRepository PostalCodeRepository { get; }
    }
}
