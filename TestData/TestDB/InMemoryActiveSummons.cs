using DBHelper;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryActiveSummons : IActiveSummonsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<ActiveSummonsCoreData> GetActiveSummonsCoreAsync(DateTime payableDate, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var data = (from s in InMemData.SummSmryTestData
                        join a in InMemData.ApplicationTestData on new { s.Appl_EnfSrv_Cd, s.Appl_CtrlCd } equals new { a.Appl_EnfSrv_Cd, a.Appl_CtrlCd }
                        where (s.Appl_EnfSrv_Cd == appl_EnfSrv_Cd) && (s.Appl_CtrlCd == appl_CtrlCd) && (payableDate.Date.IsBetween(s.Start_Dte.Date, s.End_Dte.Date))
                           && (a.AppLiSt_Cd.In(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12, ApplicationState.EXPIRED_15, ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19))
                        select new { s.Appl_EnfSrv_Cd, s.Appl_CtrlCd, s.Appl_TotalAmnt }).FirstOrDefault();

            var result = new ActiveSummonsCoreData
            {
                Appl_CtrlCd = data.Appl_CtrlCd,
                Appl_EnfSrv_Cd = data.Appl_EnfSrv_Cd,
                Appl_TotalAmnt = data.Appl_TotalAmnt
            };

            return Task.FromResult(result);
        }

        public Task<ActiveSummonsData> GetActiveSummonsDataAsync(DateTime payableDate, string appl_CtrlCd, string appl_EnfSrv_Cd, bool isVariation = false)
        {
            var data = (from a in InMemData.ApplicationTestData
                        join i in InMemData.IntFinHoldbackTestData on new { a.Appl_EnfSrv_Cd, a.Appl_CtrlCd } equals new { i.Appl_EnfSrv_Cd, i.Appl_CtrlCd }
                        join s in InMemData.SummSmryTestData on new { a.Appl_EnfSrv_Cd, a.Appl_CtrlCd } equals new { s.Appl_EnfSrv_Cd, s.Appl_CtrlCd }
                        where (i.ActvSt_Cd == "A") &&
                              (a.AppCtgy_Cd == "I01") &&
                              (a.AppLiSt_Cd.In(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12, ApplicationState.EXPIRED_15, ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)) &&
                              (payableDate.Date.IsBetween(s.Start_Dte.Date, s.End_Dte.Date)) &&
                              (a.Appl_EnfSrv_Cd == appl_EnfSrv_Cd) &&
                              (a.Appl_CtrlCd == appl_CtrlCd)
                        select new
                        {
                            a.Subm_SubmCd,
                            a.Appl_JusticeNr,
                            i.IntFinH_LmpSum_Money,
                            i.IntFinH_PerPym_Money,
                            i.IntFinH_MxmTtl_Money,
                            i.PymPr_Cd,
                            i.IntFinH_CmlPrPym_Ind,
                            i.HldbCtg_Cd,
                            i.IntFinH_DefHldbPrcnt,
                            i.IntFinH_DefHldbAmn_Money,
                            i.IntFinH_DefHldbAmn_Period,
                            i.HldbTyp_Cd,
                            s.Start_Dte,
                            s.FeeDivertedTtl_Money,
                            s.LmpSumDivertedTtl_Money,
                            s.PerPymDivertedTtl_Money,
                            s.HldbAmtTtl_Money,
                            s.Appl_TotalAmnt,
                            i.IntFinH_Dte,
                            s.End_Dte,
                            a.Appl_RecvAffdvt_Dte,
                            i.IntFinH_VarIss_Dte,
                            s.LmpSumOwedTtl_Money,
                            s.PerPymOwedTtl_Money,
                            a.Appl_EnfSrv_Cd,
                            VarEnterDte = (i.IntFinH_VarIss_Dte ?? i.IntFinH_RcvtAffdvt_Dte),
                            a.Appl_CtrlCd
                        }).First();

            var result = new ActiveSummonsData
            {
                Subm_SubmCd = data.Subm_SubmCd,
                Appl_JusticeNr = data.Appl_JusticeNr,
                IntFinH_LmpSum_Money = data.IntFinH_LmpSum_Money,
                IntFinH_PerPym_Money = data.IntFinH_PerPym_Money,
                IntFinH_MxmTtl_Money = data.IntFinH_MxmTtl_Money,
                PymPr_Cd = data.PymPr_Cd,
                IntFinH_CmlPrPym_Ind = data.IntFinH_CmlPrPym_Ind,
                HldbCtg_Cd = data.HldbCtg_Cd,
                IntFinH_DefHldbPrcnt = data.IntFinH_DefHldbPrcnt,
                IntFinH_DefHldbAmn_Money = data.IntFinH_DefHldbAmn_Money,
                IntFinH_DefHldbAmn_Period = data.IntFinH_DefHldbAmn_Period,
                HldbTyp_Cd = data.HldbTyp_Cd,
                Start_Dte = data.Start_Dte,
                FeeDivertedTtl_Money = data.FeeDivertedTtl_Money,
                LmpSumDivertedTtl_Money = data.LmpSumDivertedTtl_Money,
                PerPymDivertedTtl_Money = data.PerPymDivertedTtl_Money,
                HldbAmtTtl_Money = data.HldbAmtTtl_Money,
                Appl_TotalAmnt = data.Appl_TotalAmnt,
                IntFinH_Dte = data.IntFinH_Dte,
                End_Dte = data.End_Dte,
                Appl_RecvAffdvt_Dte = data.Appl_RecvAffdvt_Dte,
                IntFinH_VarIss_Dte = data.IntFinH_VarIss_Dte,
                LmpSumOwedTtl_Money = data.LmpSumOwedTtl_Money,
                PerPymOwedTtl_Money = data.PerPymOwedTtl_Money,
                Appl_EnfSrv_Cd = data.Appl_EnfSrv_Cd,
                VarEnterDte = data.VarEnterDte,
                Appl_CtrlCd = data.Appl_CtrlCd
            };

            return Task.FromResult(result);
        }

        public Task<DateTime> GetLegalDateAsync(string appl_CtrlCd, string appl_EnfSrv_Cd)
        {            
            throw new NotImplementedException();
        }
    }
}
