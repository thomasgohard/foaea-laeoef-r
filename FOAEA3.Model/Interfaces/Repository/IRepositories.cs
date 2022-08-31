using DBHelper;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Model.Interfaces
{
    public interface IRepositories
    {
        IDBToolsAsync MainDB { get; }

        string CurrentSubmitter { get; set; }
        string CurrentUser { get; set; }

        IApplicationRepository ApplicationTable { get; }
        IApplicationEventRepository ApplicationEventTable { get; }
        IApplicationEventDetailRepository ApplicationEventDetailTable { get; }
        ICaseManagementRepository CaseManagementTable { get; }
        IApplicationSearchRepository ApplicationSearchTable { get; }
        ISubmitterRepository SubmitterTable { get; }
        IEnfOffRepository EnfOffTable { get; }
        IEnfSrvRepository EnfSrvTable { get; }
        IProvinceRepository ProvinceTable { get; }
        IInterceptionRepository InterceptionTable { get; }
        ISubjectRepository SubjectTable { get; }
        ITracingRepository TracingTable { get; }
        ITraceResponseRepository TraceResponseTable { get; }
        ILicenceDenialRepository LicenceDenialTable { get; }
        ILicenceDenialResponseRepository LicenceDenialResponseTable { get; }
        IAffidavitRepository AffidavitTable { get; }
        ILoginRepository LoginTable { get; }
        INotificationRepository NotificationTable { get; }
        //IUserProfileRepository UserProfileRepository { get; }
        ISubjectRoleRepository SubjectRoleTable { get; }
        ISubmitterProfileRepository SubmitterProfileTable { get; }
        ISINResultRepository SINResultTable { get; }
        IProductionAuditRepository ProductionAuditTable { get; }
        ISINChangeHistoryRepository SINChangeHistoryTable { get; }
        IFamilyProvisionRepository FamilyProvisionTable { get; }
        IInfoBankRepository InfoBankTable { get; }
        IAccessAuditRepository AccessAuditTable { get; }
        IFailedSubmitAuditRepository FailedSubmitAuditTable { get; }
        IPostalCodeRepository PostalCodeTable { get; }
    }
}
