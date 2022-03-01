namespace FOAEA3.Model.Interfaces
{
    public interface IAffidavitRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        AffidavitData GetAffidavitData(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
