using System;

namespace FOAEA3.Model
{
    public class SubmitterProfileData
    {
        public string Subm_SubmCd { get; set; }
        public string Subm_FrstNme { get; set; }
        public string Subm_MddleNme { get; set; }
        public string Subm_SurNme { get; set; }
        public string Lng_Cd { get; set; }
        public DateTime? Subm_LstLgn_Dte { get; set; }
        public string Subm_Assg_Email { get; set; }
        public byte? Subm_Lic_AccsPrvCd { get; set; }
        public byte? Subm_Trcn_AccsPrvCd { get; set; }
        public byte? Subm_Intrc_AccsPrvCd { get; set; }
        public string Subm_Last_SeqNr { get; set; }
        public string Subm_Altrn_SubmCd { get; set; }
        public string EnfSrv_Cd { get; set; }
        public string EnfOff_City_LocCd { get; set; }
        public byte? Subm_TrcNtf_Ind { get; set; }
        public byte? Subm_LglSgnAuth_Ind { get; set; }
        public string Subm_SgnAuth_SubmCd { get; set; }
        public byte? Subm_EnfSrvAuth_Ind { get; set; }
        public byte? Subm_EnfOffAuth_Ind { get; set; }
        public byte? Subm_SysMgr_Ind { get; set; }
        public byte? Subm_AppMgr_Ind { get; set; }
        public string EnfSrv_Nme { get; set; }
        public string EnfSrv_Subm_Auth_SubmCd { get; set; }
        public string EnfOff_Nme { get; set; }
        public string EnfOff_Subm_Auth_SubmCd { get; set; }
        public string EnfOff_Addr_CityNme { get; set; }
        public string EnfOff_Dstrct_Nme { get; set; }
        public string Prv_Txt_E { get; set; }
        public string Prv_Cd { get; set; }
        public string Ctry_Txt_E { get; set; }
        public string Ctry_Cd { get; set; }
        public string Subm_Class { get; set; }
        public string ActvSt_Cd { get; set; }
        public byte? Subm_Fin_Ind { get; set; }

    }
}
