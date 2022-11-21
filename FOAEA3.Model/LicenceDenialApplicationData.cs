using System;

namespace FOAEA3.Model
{
    public class LicenceDenialApplicationData : ApplicationData
    {
        public LicenceDenialApplicationData()
        {
            AppCtgy_Cd = "L01";
            ActvSt_Cd = "A";
            Appl_SIN_Cnfrmd_Ind = 0;
            Appl_Create_Dte = DateTime.Now;
            Appl_Rcptfrm_Dte = Appl_Create_Dte.Date; // only date, no time
            Appl_Lgl_Dte = Appl_Create_Dte;
            Appl_LastUpdate_Dte = Appl_Create_Dte;
        }

        public DateTime LicSusp_SupportOrder_Dte { get; set; }
        public DateTime LicSusp_NoticeSentToDbtr_Dte { get; set; }
        public string LicSusp_CourtNme { get; set; }
        public string PymPr_Cd { get; set; }
        public short? LicSusp_NrOfPymntsInDefault { get; set; }
        public decimal? LicSusp_AmntOfArrears { get; set; }
        public string LicSusp_Dbtr_EmplNme { get; set; }
        public string LicSusp_Dbtr_EmplAddr_Ln { get; set; }
        public string LicSusp_Dbtr_EmplAddr_Ln1 { get; set; }
        public string LicSusp_Dbtr_EmplAddr_CityNme { get; set; }
        public string LicSusp_Dbtr_EmplAddr_PrvCd { get; set; }
        public string LicSusp_Dbtr_EmplAddr_CtryCd { get; set; }
        public string LicSusp_Dbtr_EmplAddr_PCd { get; set; }
        public string LicSusp_Dbtr_EyesColorCd { get; set; }
        public string LicSusp_Dbtr_HeightUOMCd { get; set; }
        public int? LicSusp_Dbtr_HeightQty { get; set; }
        public string LicSusp_Dbtr_PhoneNumber { get; set; }
        public string LicSusp_Dbtr_EmailAddress { get; set; }
        public string LicSusp_Dbtr_Brth_CityNme { get; set; }
        public string LicSusp_Dbtr_Brth_CtryCd { get; set; }
        public DateTime? LicSusp_TermRequestDte { get; set; }
        public byte LicSusp_Still_InEffect_Ind { get; set; }
        public byte LicSusp_AnyLicRvkd_Ind { get; set; }
        public byte LicSusp_AnyLicReinst_Ind { get; set; }
        public short LicSusp_LiStCd { get; set; }
        public string LicSusp_Appl_CtrlCd { get; set; }
        public string LicSusp_Dbtr_LastAddr_Ln { get; set; }
        public string LicSusp_Dbtr_LastAddr_Ln1 { get; set; }
        public string LicSusp_Dbtr_LastAddr_CityNme { get; set; }
        public string LicSusp_Dbtr_LastAddr_PrvCd { get; set; }
        public string LicSusp_Dbtr_LastAddr_CtryCd { get; set; }
        public string LicSusp_Dbtr_LastAddr_PCd { get; set; }
        public bool? LicSusp_Declaration_Ind { get; set; }
        public string LicSusp_Declaration { get; set; }

        public void Merge(LicenceDenialApplicationData data)
        {
            LicSusp_SupportOrder_Dte = data.LicSusp_SupportOrder_Dte;
            LicSusp_NoticeSentToDbtr_Dte = data.LicSusp_NoticeSentToDbtr_Dte;
            LicSusp_CourtNme = data.LicSusp_CourtNme;
            PymPr_Cd = data.PymPr_Cd;
            LicSusp_NrOfPymntsInDefault = data.LicSusp_NrOfPymntsInDefault;
            LicSusp_AmntOfArrears = data.LicSusp_AmntOfArrears;
            LicSusp_Dbtr_EmplNme = data.LicSusp_Dbtr_EmplNme;
            LicSusp_Dbtr_EmplAddr_Ln = data.LicSusp_Dbtr_EmplAddr_Ln;
            LicSusp_Dbtr_EmplAddr_Ln1 = data.LicSusp_Dbtr_EmplAddr_Ln1;
            LicSusp_Dbtr_EmplAddr_CityNme = data.LicSusp_Dbtr_EmplAddr_CityNme;
            LicSusp_Dbtr_EmplAddr_PrvCd = data.LicSusp_Dbtr_EmplAddr_PrvCd;
            LicSusp_Dbtr_EmplAddr_CtryCd = data.LicSusp_Dbtr_EmplAddr_CtryCd;
            LicSusp_Dbtr_EmplAddr_PCd = data.LicSusp_Dbtr_EmplAddr_PCd;
            LicSusp_Dbtr_EyesColorCd = data.LicSusp_Dbtr_EyesColorCd;
            LicSusp_Dbtr_HeightUOMCd = data.LicSusp_Dbtr_HeightUOMCd;
            LicSusp_Dbtr_HeightQty = data.LicSusp_Dbtr_HeightQty;
            LicSusp_Dbtr_Brth_CityNme = data.LicSusp_Dbtr_Brth_CityNme;
            LicSusp_Dbtr_Brth_CtryCd = data.LicSusp_Dbtr_Brth_CtryCd;
            LicSusp_TermRequestDte = data.LicSusp_TermRequestDte;
            LicSusp_Still_InEffect_Ind = data.LicSusp_Still_InEffect_Ind;
            LicSusp_AnyLicRvkd_Ind = data.LicSusp_AnyLicRvkd_Ind;
            LicSusp_AnyLicReinst_Ind = data.LicSusp_AnyLicReinst_Ind;
            LicSusp_LiStCd = data.LicSusp_LiStCd;
            LicSusp_Appl_CtrlCd = data.LicSusp_Appl_CtrlCd;
            LicSusp_Dbtr_LastAddr_Ln = data.LicSusp_Dbtr_LastAddr_Ln;
            LicSusp_Dbtr_LastAddr_Ln1 = data.LicSusp_Dbtr_LastAddr_Ln1;
            LicSusp_Dbtr_LastAddr_CityNme = data.LicSusp_Dbtr_LastAddr_CityNme;
            LicSusp_Dbtr_LastAddr_PrvCd = data.LicSusp_Dbtr_LastAddr_PrvCd;
            LicSusp_Dbtr_LastAddr_CtryCd = data.LicSusp_Dbtr_LastAddr_CtryCd;
            LicSusp_Dbtr_LastAddr_PCd = data.LicSusp_Dbtr_LastAddr_PCd;
        }
    }
}
