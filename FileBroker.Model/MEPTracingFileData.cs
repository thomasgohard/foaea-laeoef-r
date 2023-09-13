using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FileBroker.Model
{
    public struct MEPTracing_RecType01
    {
        public string RecType;
        public string Cycle;
        public string FileDate;
        public string TermsAccepted;
    }

    public struct MEPTracing_RecType20
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_Appl_Source_RfrNr;
        public string dat_Appl_EnfSrvCd;
        public string dat_Subm_Rcpt_SubmCd;
        public string dat_Appl_Lgl_Dte;
        public string dat_Appl_Dbtr_SurNme;
        public string dat_Appl_Dbtr_FrstNme;
        public string dat_Appl_Dbtr_MddleNme;
        public string dat_Appl_Dbtr_Brth_Dte;
        public string dat_Appl_Dbtr_Gendr_Cd;
        public string dat_Appl_Dbtr_Entrd_SIN;
        public string dat_Appl_Dbtr_Parent_SurNme_Birth;
        public string dat_Appl_CommSubm_Text;
        public string dat_Appl_Rcptfrm_dte;
        public string dat_Appl_AppCtgy_Cd;
        public string dat_Appl_Group_Batch_Cd;
        public string dat_Appl_Medium_Cd;
        public string dat_Appl_Affdvt_Doc_TypCd;
        public string dat_Appl_Reas_Cd;
        public string dat_Appl_Reactv_Dte;
        public string dat_Appl_LiSt_Cd;
        public string Maintenance_ActionCd;
        public string dat_New_Owner_RcptSubmCd;
        public string dat_New_Owner_SubmCd;
        public string dat_Update_SubmCd;
    }

    public struct MEPTracing_RecType21
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_Appl_Crdtr_SurNme;
        public string dat_Appl_Crdtr_FrstNme;
        public string dat_Appl_Crdtr_MddleNme;
        public string dat_FamPro_Cd;
        public string dat_Trace_Child_Text;
        public string dat_Trace_Breach_Text;
        public string dat_Trace_ReasGround_Text;
        public string dat_InfoBank_Cd;
        public string dat_Statute_Cd;
    }

    public struct Tax_Data_Info
    {
        public string Tax_Year;

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Tax_Form;
    }

    public struct FinancialDetails
    {
        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<Tax_Data_Info>))]
        public List<Tax_Data_Info> Tax_Data;
    }

    public struct MEPTracing_RecType22
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        
        [DataMember(IsRequired = false)]
        public string dat_Trace_Dbtr_PhoneNumber;
        
        [DataMember(IsRequired = false)]
        public string dat_Trace_Dbtr_EmailAddress;
        
        public string dat_Trace_Declaration;
        public string dat_Tracing_Info;
        public string dat_SIN_Information;
        public string dat_Financial_Information;

        [DataMember(IsRequired = false)]
        public FinancialDetails dat_Financial_Details;
    }

    public struct MEPTracing_RecType99
    {
        public string RecType;
        public string ResponseCnt;
    }

    public struct MEPTracing_TracingDataSet
    {
        public MEPTracing_RecType01 TRCAPPIN01;

        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<MEPTracing_RecType20>))]
        public List<MEPTracing_RecType20> TRCAPPIN20;

        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<MEPTracing_RecType21>))]
        public List<MEPTracing_RecType21> TRCAPPIN21;

        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<MEPTracing_RecType22>))]
        public List<MEPTracing_RecType22> TRCAPPIN22;

        public MEPTracing_RecType99 TRCAPPIN99;
    }

    public class MEPTracingFileData
    {
        public MEPTracing_TracingDataSet NewDataSet;
    }

}
