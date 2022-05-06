using System;
using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct MEPInterception_RecType01
    {
        public string RecType;
        public string Cycle;
        public DateTime FileDate;
        public string TermsAccepted;
    }

    public struct MEPInterception_RecType10
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_Appl_Source_RfrNr;
        public string dat_Appl_EnfSrvCd;
        public string dat_Subm_Rcpt_SubmCd;
        public DateTime dat_Appl_Lgl_Dte;
        public string dat_Appl_Dbtr_SurNme;
        public string dat_Appl_Dbtr_FrstNme;
        public string dat_Appl_Dbtr_MddleNme;
        public DateTime dat_Appl_Dbtr_Brth_Dte;
        public string dat_Appl_Dbtr_Gendr_Cd;
        public string dat_Appl_Dbtr_Entrd_SIN;
        public string dat_Appl_Dbtr_Parent_SurNme_Birth;
        public string dat_Appl_CommSubm_Text;
        public DateTime dat_Appl_Rcptfrm_dte;
        public string dat_Appl_AppCtgy_Cd;
        public string dat_Appl_Group_Batch_Cd;
        public string dat_Appl_Medium_Cd;
        public string dat_Appl_Affdvt_Doc_TypCd;
        public string dat_Appl_Reas_Cd;
        public DateTime dat_Appl_Reactv_Dte;
        public string dat_Appl_LiSt_Cd;
        public string Maintenance_ActionCd;
        public string dat_New_Owner_RcptSubmCd;
        public string dat_New_Owner_SubmCd;
    }

    public struct MEPInterception_RecType11
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_Appl_Dbtr_LngCd;
        public string dat_Appl_Dbtr_Addr_Ln;
        public string dat_Appl_Dbtr_Addr_Ln1;
        public string dat_Appl_Dbtr_Addr_CityNme;
        public string dat_Appl_Dbtr_Addr_CtryCd;
        public string dat_Appl_Dbtr_Addr_PCd;
        public string dat_Appl_Dbtr_Addr_PrvCd;
        public string dat_Appl_Crdtr_SurNme;
        public string dat_Appl_Crdtr_FrstNme;
        public string dat_Appl_Crdtr_MddleNme;
        public DateTime dat_Appl_Crdtr_Brth_Dte;
    }

    public struct MEPInterception_RecType12
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_IntFinH_LmpSum_Money;
        public string dat_IntFinH_Perpym_Money;
        public string dat_PymPr_Cd;
        public string dat_IntFinH_CmlPrPym_Ind;
        public string dat_IntFinH_NextRecalc_Dte;
        public string dat_HldbCtg_Cd;
        public string dat_IntFinH_DfHldbPrcnt;
        public string dat_IntFinH_DefHldbAmn_Money;
        public string dat_IntFinH_DefHldbAmn_Period;
        public DateTime? dat_IntFinH_VarIss_Dte;
    }

    public struct MEPInterception_RecType13
    {
        public string RecType;
        public string dat_Subm_SubmCd;
        public string dat_Appl_CtrlCd;
        public string dat_EnfSrv_Cd;
        public string dat_HldbCtg_Cd;
        public string dat_HldbCnd_SrcHldbPrcnt;
        public string dat_HldbCnd_Hldb_Amn_Money;
        public string dat_HldbCnd_MxmPerChq_Money;
    }

    public struct MEPInterception_RecType99
    {
        public string RecType;
        public string ResponseCnt;
    }

    public struct MEPInterception_InterceptionDataSet
    {
        public MEPInterception_RecType01 INTAPPIN01;
        public List<MEPInterception_RecType10> INTAPPIN10;
        public List<MEPInterception_RecType11> INTAPPIN11;
        public List<MEPInterception_RecType12> INTAPPIN12;
        public List<MEPInterception_RecType13> INTAPPIN13;
        public MEPInterception_RecType99 INTAPPIN99;
    }

    public struct MEPInterception_InterceptionDataSetSingle
    {
        public MEPInterception_RecType01 INTAPPIN01;
        public MEPInterception_RecType10 INTAPPIN10;
        public MEPInterception_RecType11 INTAPPIN11;
        public MEPInterception_RecType12 INTAPPIN12;
        public List<MEPInterception_RecType13> INTAPPIN13;
        public MEPInterception_RecType99 INTAPPIN99;
    }

    public struct MEPInterception_InterceptionDataSetSingleSource
    {
        public MEPInterception_RecType01 INTAPPIN01;
        public MEPInterception_RecType10 INTAPPIN10;
        public MEPInterception_RecType11 INTAPPIN11;
        public MEPInterception_RecType12 INTAPPIN12;
        public MEPInterception_RecType13 INTAPPIN13;
        public MEPInterception_RecType99 INTAPPIN99;
    }

    public struct MEPInterception_InterceptionDataSetNoSource
    {
        public MEPInterception_RecType01 INTAPPIN01;
        public MEPInterception_RecType10 INTAPPIN10;
        public MEPInterception_RecType11 INTAPPIN11;
        public MEPInterception_RecType12 INTAPPIN12;
        public MEPInterception_RecType99 INTAPPIN99;
    }

    public class MEPInterceptionFileData
    {
        public MEPInterception_InterceptionDataSet NewDataSet;

        public MEPInterceptionFileData()
        {
            NewDataSet.INTAPPIN10 = new List<MEPInterception_RecType10>();
            NewDataSet.INTAPPIN11 = new List<MEPInterception_RecType11>();
            NewDataSet.INTAPPIN12 = new List<MEPInterception_RecType12>();
            NewDataSet.INTAPPIN13 = new List<MEPInterception_RecType13>();
        }
    }

    public class MEPInterceptionFileDataSingle
    {
        public MEPInterception_InterceptionDataSetSingle NewDataSet;

        public MEPInterceptionFileDataSingle()
        {
            NewDataSet.INTAPPIN13 = new List<MEPInterception_RecType13>();
        }
    }

    public class MEPInterceptionFileDataSingleSource
    {
        public MEPInterception_InterceptionDataSetSingleSource NewDataSet;
    }

    public class MEPInterceptionFileDataNoSource
    {
        public MEPInterception_InterceptionDataSetNoSource NewDataSet;
    }

}
