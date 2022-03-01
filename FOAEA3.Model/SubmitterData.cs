using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FOAEA3.Model
{
    [Serializable]
    public class SubmitterData : IMessageList
    {
        public SubmitterData()
        {
            // default values
            ActvSt_Cd = "A";
            Lng_Cd = "E";
            Subm_EnfOffAuth_Ind = true;
        }

        [Display(Name = "SUBMITTER_CODE", ResourceType = typeof(LanguageResource))]
        public string Subm_SubmCd { get; set; }

        [Display(Name = "FIRST_NAME", ResourceType = typeof(LanguageResource))]
        public string Subm_FrstNme { get; set; }

        [Display(Name = "SECOND_NAME", ResourceType = typeof(LanguageResource))]
        public string Subm_MddleNme { get; set; }

        [Display(Name = "SURNAME", ResourceType = typeof(LanguageResource))]
        public string Subm_SurNme { get; set; }

        [Display(Name = "LANGUAGE_PREFERENCE", ResourceType = typeof(LanguageResource))]
        public string Lng_Cd { get; set; }

        public DateTime? Subm_LstLgn_Dte { get; set; }

        [Display(Name = "EMAIL_ADDRESS", ResourceType = typeof(LanguageResource))]
        public string Subm_Assg_Email { get; set; }

        public string Subm_IP_Addr { get; set; }

        [Display(Name = "LICENSE_DENIAL", ResourceType = typeof(LanguageResource))]
        public bool Subm_Lic_AccsPrvCd { get; set; }

        [Display(Name = "TRACING", ResourceType = typeof(LanguageResource))]
        public bool Subm_Trcn_AccsPrvCd { get; set; }

        [Display(Name = "INTERCEPTION", ResourceType = typeof(LanguageResource))]
        public bool Subm_Intrc_AccsPrvCd { get; set; }

        [Display(Name = "TELEPHONE", ResourceType = typeof(LanguageResource))]
        public string Subm_Tel_AreaC { get; set; }

        public string Subm_TelNr { get; set; }

        public string Subm_TelEx { get; set; }

        [Display(Name = "FAX", ResourceType = typeof(LanguageResource))]
        public string Subm_Fax_AreaC { get; set; }

        public string Subm_FaxNr { get; set; }

        public string Subm_Last_SeqNr { get; set; }

        public string Subm_Altrn_SubmCd { get; set; }

        [Display(Name = "ENFORCEMENT_SERVICE", ResourceType = typeof(LanguageResource))]
        public string EnfSrv_Cd { get; set; }

        [Display(Name = "ENFORCEMENT_OFFICE", ResourceType = typeof(LanguageResource))]
        public string EnfOff_City_LocCd { get; set; }

        [Display(Name = "SUBMITTER_TITLE", ResourceType = typeof(LanguageResource))]
        public string Subm_Title { get; set; }

        public string Subm_SgnAuth_SubmCd { get; set; }

        [Display(Name = "COMMENTS", ResourceType = typeof(LanguageResource))]
        public string Subm_Comments { get; set; }

        public bool Subm_TrcNtf_Ind { get; set; }

        [Display(Name = "SWEARS_AFFIDAVITS", ResourceType = typeof(LanguageResource))]
        public bool Subm_LglSgnAuth_Ind { get; set; }

        [Display(Name = "ENFORCEMENT_SERVICE", ResourceType = typeof(LanguageResource))]
        public bool Subm_EnfSrvAuth_Ind { get; set; }

        [Display(Name = "ENFORCEMENT_OFFICE", ResourceType = typeof(LanguageResource))]
        public bool Subm_EnfOffAuth_Ind { get; set; }

        [Display(Name = "SYSTEM_MANAGER", ResourceType = typeof(LanguageResource))]
        public bool Subm_SysMgr_Ind { get; set; }

        [Display(Name = "APPLICATION_MANAGER", ResourceType = typeof(LanguageResource))]
        public bool Subm_AppMgr_Ind { get; set; }

        [Display(Name = "FINANCE", ResourceType = typeof(LanguageResource))]
        public bool Subm_Fin_Ind { get; set; }

        [Display(Name = "COURT_USER", ResourceType = typeof(LanguageResource))]
        public bool Subm_CourtUsr_Ind { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(LanguageResource))]
        public string ActvSt_Cd { get; set; }

        public short? Subm_LiSt_Cd { get; set; }

        public string Subm_Class { get; set; }

        public string Password { get; set; }

        public int? PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        public string Subm_LastUpdate_Usr { get; set; }

        public string Subm_Create_Usr { get; set; }

        [NonSerialized]
        private MessageDataList messages;

        public MessageDataList Messages
        {
            get { return messages; }
            set { messages = value; }
        }

    }
}
