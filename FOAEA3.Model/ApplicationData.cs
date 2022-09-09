using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using FOAEA3.Resources.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FOAEA3.Model
{

    [DebuggerDisplay("{Appl_EnfSrv_Cd}-{Appl_CtrlCd} [{AppCtgy_Cd} at {AppLiSt_Cd}]")]
    [Serializable]
    public class ApplicationData : IMessageList, IEventList
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Subm_Recpt_SubmCd { get; set; }
        public DateTime Appl_Lgl_Dte { get; set; }

        [Display(Name = "APPL_COMMENT", ResourceType = typeof(LanguageResource))]
        public string Appl_CommSubm_Text { get; set; }
        public DateTime Appl_Rcptfrm_Dte { get; set; }
        public string Appl_Group_Batch_Cd { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public string Subm_Affdvt_SubmCd { get; set; }
        public DateTime? Appl_RecvAffdvt_Dte { get; set; }
        public string Appl_Affdvt_DocTypCd { get; set; }
        public string Appl_JusticeNr { get; set; }
        public string Appl_Crdtr_FrstNme { get; set; }
        public string Appl_Crdtr_MddleNme { get; set; }
        public string Appl_Crdtr_SurNme { get; set; }

        [Display(Name = "FIRST_NAME", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_FrstNme { get; set; }

        [Display(Name = "SECOND_NAME", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_MddleNme { get; set; }

        [Display(Name = "SURNAME", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_SurNme { get; set; }

        [Display(Name = "MOTHER_MAIDEN_NAME", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_Parent_SurNme { get; set; }

        [Display(Name = "DOB", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = DateTimeExtensions.FOAEA_DATE_FORMAT)]
        public DateTime? Appl_Dbtr_Brth_Dte { get; set; }

        [Display(Name = "LANGUAGE_OF_CHOICE", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_LngCd { get; set; }

        [Display(Name = "GENDER", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_Gendr_Cd { get; set; }

        [Display(Name = "SIN", ResourceType = typeof(LanguageResource))]
        public string Appl_Dbtr_Entrd_SIN { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public string Appl_Dbtr_RtrndBySrc_SIN { get; set; }
        public string Appl_Dbtr_Addr_Ln { get; set; }
        public string Appl_Dbtr_Addr_Ln1 { get; set; }
        public string Appl_Dbtr_Addr_CityNme { get; set; }
        public string Appl_Dbtr_Addr_PrvCd { get; set; }
        public string Appl_Dbtr_Addr_CtryCd { get; set; }
        public string Appl_Dbtr_Addr_PCd { get; set; }
        public string Medium_Cd { get; set; }
        public DateTime? Appl_Reactv_Dte { get; set; }
        public byte Appl_SIN_Cnfrmd_Ind { get; set; }
        public string AppCtgy_Cd { get; set; }
        public string AppReas_Cd { get; set; }
        public DateTime Appl_Create_Dte { get; set; }
        public string Appl_Create_Usr { get; set; }
        public DateTime? Appl_LastUpdate_Dte { get; set; }
        public string Appl_LastUpdate_Usr { get; set; }
        public string ActvSt_Cd { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
        public Guid? Appl_WFID { get; set; }
        public DateTime? Appl_Crdtr_Brth_Dte { get; set; }

        public MessageDataList Messages { get; set; }

        public ApplicationData()
        {
            // set default values
            AppLiSt_Cd = ApplicationState.UNDEFINED;
            Appl_Dbtr_Gendr_Cd = "M";
            Messages = new MessageDataList();
        }

        public void Merge(ApplicationData data)
        {
            Appl_EnfSrv_Cd = data.Appl_EnfSrv_Cd;
            Appl_CtrlCd = data.Appl_CtrlCd;
            Subm_SubmCd = data.Subm_SubmCd;
            Subm_Recpt_SubmCd = data.Subm_Recpt_SubmCd;
            Appl_Lgl_Dte = data.Appl_Lgl_Dte;
            Appl_CommSubm_Text = data.Appl_CommSubm_Text;
            Appl_Rcptfrm_Dte = data.Appl_Rcptfrm_Dte;
            Appl_Group_Batch_Cd = data.Appl_Group_Batch_Cd;
            Appl_Source_RfrNr = data.Appl_Source_RfrNr;
            Subm_Affdvt_SubmCd = data.Subm_Affdvt_SubmCd;
            Appl_RecvAffdvt_Dte = data.Appl_RecvAffdvt_Dte;
            Appl_Affdvt_DocTypCd = data.Appl_Affdvt_DocTypCd;
            Appl_JusticeNr = data.Appl_JusticeNr;
            Appl_Crdtr_FrstNme = data.Appl_Crdtr_FrstNme;
            Appl_Crdtr_MddleNme = data.Appl_Crdtr_MddleNme;
            Appl_Crdtr_SurNme = data.Appl_Crdtr_SurNme;
            Appl_Dbtr_FrstNme = data.Appl_Dbtr_FrstNme;
            Appl_Dbtr_MddleNme = data.Appl_Dbtr_MddleNme;
            Appl_Dbtr_SurNme = data.Appl_Dbtr_SurNme;
            Appl_Dbtr_Parent_SurNme = data.Appl_Dbtr_Parent_SurNme;
            Appl_Dbtr_Brth_Dte = data.Appl_Dbtr_Brth_Dte;
            Appl_Dbtr_LngCd = data.Appl_Dbtr_LngCd;
            Appl_Dbtr_Gendr_Cd = data.Appl_Dbtr_Gendr_Cd;
            Appl_Dbtr_Entrd_SIN = data.Appl_Dbtr_Entrd_SIN;
            Appl_Dbtr_Cnfrmd_SIN = data.Appl_Dbtr_Cnfrmd_SIN;
            Appl_Dbtr_RtrndBySrc_SIN = data.Appl_Dbtr_RtrndBySrc_SIN;
            Appl_Dbtr_Addr_Ln = data.Appl_Dbtr_Addr_Ln;
            Appl_Dbtr_Addr_Ln1 = data.Appl_Dbtr_Addr_Ln1;
            Appl_Dbtr_Addr_CityNme = data.Appl_Dbtr_Addr_CityNme;
            Appl_Dbtr_Addr_PrvCd = data.Appl_Dbtr_Addr_PrvCd;
            Appl_Dbtr_Addr_CtryCd = data.Appl_Dbtr_Addr_CtryCd;
            Appl_Dbtr_Addr_PCd = data.Appl_Dbtr_Addr_PCd;
            Medium_Cd = data.Medium_Cd;
            Appl_Reactv_Dte = data.Appl_Reactv_Dte;
            Appl_SIN_Cnfrmd_Ind = data.Appl_SIN_Cnfrmd_Ind;
            AppCtgy_Cd = data.AppCtgy_Cd;
            AppReas_Cd = data.AppReas_Cd;
            Appl_Create_Dte = data.Appl_Create_Dte;
            Appl_Create_Usr = data.Appl_Create_Usr;
            Appl_LastUpdate_Dte = data.Appl_LastUpdate_Dte;
            Appl_LastUpdate_Usr = data.Appl_LastUpdate_Usr;
            ActvSt_Cd = data.ActvSt_Cd;
            AppLiSt_Cd = data.AppLiSt_Cd;
            Appl_WFID = data.Appl_WFID;
            Appl_Crdtr_Brth_Dte = data.Appl_Crdtr_Brth_Dte;
        }

    }
}
