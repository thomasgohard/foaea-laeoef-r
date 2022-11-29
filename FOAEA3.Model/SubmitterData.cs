using FOAEA3.Model.Interfaces;
using System;

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

        public string Subm_SubmCd { get; set; }
        public string Subm_FrstNme { get; set; }
        public string Subm_MddleNme { get; set; }
        public string Subm_SurNme { get; set; }
        public string Lng_Cd { get; set; }
        public DateTime? Subm_LstLgn_Dte { get; set; }
        public string Subm_Assg_Email { get; set; }
        public string Subm_IP_Addr { get; set; }
        public bool Subm_Lic_AccsPrvCd { get; set; }
        public bool Subm_Trcn_AccsPrvCd { get; set; }
        public bool Subm_Intrc_AccsPrvCd { get; set; }
        public string Subm_Tel_AreaC { get; set; }
        public string Subm_TelNr { get; set; }
        public string Subm_TelEx { get; set; }
        public string Subm_Fax_AreaC { get; set; }
        public string Subm_FaxNr { get; set; }
        public string Subm_Last_SeqNr { get; set; }
        public string Subm_Altrn_SubmCd { get; set; }
        public string EnfSrv_Cd { get; set; }
        public string EnfOff_City_LocCd { get; set; }
        public string Subm_Title { get; set; }
        public string Subm_SgnAuth_SubmCd { get; set; }
        public string Subm_Comments { get; set; }
        public bool Subm_TrcNtf_Ind { get; set; }
        public bool Subm_LglSgnAuth_Ind { get; set; }
        public bool Subm_EnfSrvAuth_Ind { get; set; }
        public bool Subm_EnfOffAuth_Ind { get; set; }
        public bool Subm_SysMgr_Ind { get; set; }
        public bool Subm_AppMgr_Ind { get; set; }
        public bool Subm_Fin_Ind { get; set; }
        public bool Subm_CourtUsr_Ind { get; set; }
        public string ActvSt_Cd { get; set; }
        public short? Subm_LiSt_Cd { get; set; }
        public string Subm_Class { get; set; }
        public string Password { get; set; }
        public int? PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string Subm_LastUpdate_Usr { get; set; }
        public string Subm_Create_Usr { get; set; }
        public bool Subm_Audit_File_Ind { get; set; }

        [NonSerialized]
        private MessageDataList messages;

        public MessageDataList Messages
        {
            get { return messages; }
            set { messages = value; }
        }

    }
}
