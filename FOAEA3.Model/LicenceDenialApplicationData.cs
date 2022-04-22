using FOAEA3.Resources;
using FOAEA3.Resources.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace FOAEA3.Model
{
    public class LicenceDenialApplicationData : ApplicationData
    {
        public LicenceDenialApplicationData()
        {
            AppCtgy_Cd = "L01";
        }

        //public LicenceDenialData(IReferenceData referenceData): base(referenceData)
        //{

        //}

        [Display(Name = "SUPPORTORDER_DTE", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = DateTimeExtensions.YYYY_MM_DD)]
        public DateTime LicSusp_SupportOrder_Dte { get; set; }
        [Display(Name = "NOTICESENTTODEBTOR_DTE", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = DateTimeExtensions.YYYY_MM_DD)]
        public DateTime LicSusp_NoticeSentToDbtr_Dte { get; set; }
        [Display(Name = "COURT_NAME", ResourceType = typeof(LanguageResource))]
        public string LicSusp_CourtNme { get; set; }
        [Display(Name = "PAYMENT_PERIOD_CODE", ResourceType = typeof(LanguageResource))]
        public string PymPr_Cd { get; set; }
        [Display(Name = "NUMBER_PAYMENTS_IN_DEFAULT", ResourceType = typeof(LanguageResource))]
        public short? LicSusp_NrOfPymntsInDefault { get; set; }
        [Display(Name = "AMOUNT_OF_ARREARS", ResourceType = typeof(LanguageResource))]
        public decimal? LicSusp_AmntOfArrears { get; set; }
        [Display(Name = "EMPLOYER_NAME", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplNme { get; set; }
        [Display(Name = "EMPLOYER_ADDRESS1", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_Ln { get; set; }
        [Display(Name = "IF_KNOWN", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_Ln1 { get; set; }
        [Display(Name = "EMPLOYER_ADDRESS_CITY", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_CityNme { get; set; }
        [Display(Name = "EMPLOYER_ADDRESS_PROVCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_PrvCd { get; set; }
        [Display(Name = "EMPLOYER_ADDRESS_COUNTRYCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_CtryCd { get; set; }
        [Display(Name = "EMPLOYER_ADDRESS_POSTALCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EmplAddr_PCd { get; set; }
        [Display(Name = "EYE_COLOUR", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_EyesColorCd { get; set; }
        public string LicSusp_Dbtr_HeightUOMCd { get; set; }
        [Display(Name = "DEBTOR_HEIGHT", ResourceType = typeof(LanguageResource))]
        public int? LicSusp_Dbtr_HeightQty { get; set; }
        public string LicSusp_Dbtr_PhoneNumber { get; set; }
        public string LicSusp_Dbtr_EmailAddress { get; set; }
        [Display(Name = "DEBTOR_BIRTH_CITY", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_Brth_CityNme { get; set; }
        [Display(Name = "DEBTOR_BIRTH_COUNTRY", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_Brth_CtryCd { get; set; }
        [Display(Name = "TERM_REQ_DATE", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = DateTimeExtensions.YYYY_MM_DD)]
        public DateTime? LicSusp_TermRequestDte { get; set; }
        public byte LicSusp_Still_InEffect_Ind { get; set; }
        public byte LicSusp_AnyLicRvkd_Ind { get; set; }
        public byte LicSusp_AnyLicReinst_Ind { get; set; }
        public short LicSusp_LiStCd { get; set; }
        public string LicSusp_Appl_CtrlCd { get; set; }
        [Display(Name = "DEBTOR_LAST_ADDRESS", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_LastAddr_Ln { get; set; }
        public string LicSusp_Dbtr_LastAddr_Ln1 { get; set; }
        [Display(Name = "DEBTOR_LAST_CITY", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_LastAddr_CityNme { get; set; }
        [Display(Name = "DEBTOR_LAST_PROVCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_LastAddr_PrvCd { get; set; }
        [Display(Name = "DEBTOR_LAST_COUNTRYCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_LastAddr_CtryCd { get; set; }
        [Display(Name = "DEBTOR_LAST_POSTALCODE", ResourceType = typeof(LanguageResource))]
        public string LicSusp_Dbtr_LastAddr_PCd { get; set; }
        public bool? LicSusp_Declaration_Ind { get; set; }

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
