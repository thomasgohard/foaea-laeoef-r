using System.Collections.Generic;

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

    public struct MEPTracing_RecType99
    {
        public string RecType;
        public string ResponseCnt;
    }

    public struct MEPTracing_TracingDataSet
    {
        public MEPTracing_RecType01 TRCAPPIN01;
        public List<MEPTracing_RecType20> TRCAPPIN20;
        public List<MEPTracing_RecType21> TRCAPPIN21;
        public MEPTracing_RecType99 TRCAPPIN99;
    }

    public struct MEPTracing_TracingDataSetSingle
    {
        public MEPTracing_RecType01 TRCAPPIN01;
        public MEPTracing_RecType20 TRCAPPIN20;
        public MEPTracing_RecType21 TRCAPPIN21;
        public MEPTracing_RecType99 TRCAPPIN99;
    }


    public class MEPTracingFileData
    {
        public MEPTracing_TracingDataSet NewDataSet;

        public MEPTracingFileData()
        {
            NewDataSet.TRCAPPIN20 = new List<MEPTracing_RecType20>();
            NewDataSet.TRCAPPIN21 = new List<MEPTracing_RecType21>();
        }
    }

    public class MEPTracingFileDataSingle
    {
        public MEPTracing_TracingDataSetSingle NewDataSet;
    }

}
