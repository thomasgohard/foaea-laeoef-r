using System;
using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct FedTracing_RecType01
    {
        public int RecType;
        public string Appl_EnfSrcCd; 
        public string Appl_CtrlCd;
        public int Cycle;
        public DateTime FileDate;
    }

    public struct FedTracing_RecType02
    {
        public int RecType;
        public string dat_Appl_EnfSrvCd;
        public string dat_Appl_CtrlCd;
        public string dat_TrcSt_Cd;
        public string LinkedSin;
        public int ResAddCnt;
        public int EmpAddCnt;
        public int Type5AddCnt;
    }

    public struct FedTracing_RecTypeResidential
    {
        public int RecType;
        public string dat_Appl_EnfSrvCd;
        public string dat_Appl_CtrlCd;
        public string dat_TrcRsp_Addr_Ln;
        public string dat_TrcRsp_Addr_Ln1;
        public string dat_TrcRsp_Addr_CityNme;
        public string dat_TrcRsp_Addr_PrvCd;
        public string dat_TrcRsp_Addr_PCd;
        public string dat_TrcRsp_Addr_CtryCd;
        public DateTime dat_TrcRsp_Addr_LstUpdte;
    }

    public struct FedTracing_RecTypeEmployer
    {
        public int RecType;
        public string dat_Appl_EnfSrvCd;
        public string dat_Appl_CtrlCd;
        public string dat_TrcRsp_Addr_Ln;
        public string dat_TrcRsp_Addr_Ln1;
        public string dat_TrcRsp_Addr_CityNme;
        public string dat_TrcRsp_Addr_PrvCd;
        public string dat_TrcRsp_Addr_PCd;
        public string dat_TrcRsp_Addr_CtryCd;
        public string dat_TrcRcp_EmplNme;
        public string dat_TrcRcp_EmplNme1;
        public DateTime dat_TrcRsp_Addr_LstUpdte;
    }

    public struct FedTracing_RecType99
    {
        public int RecType;
        public int ResponseCnt;
        public int SkippedResponseCNT;
    }

    public class FedTracingFileBase
    {
        public FedTracing_RecType01 TRCIN01;
        public List<FedTracing_RecType02> TRCIN02;
        public Dictionary<string, List<FedTracing_RecTypeResidential>> TRCINResidentials;
        public Dictionary<string, List<FedTracing_RecTypeEmployer>> TRCINEmployers;
        public FedTracing_RecType99 TRCIN99;

        public FedTracingFileBase()
        {
            TRCIN02 = new List<FedTracing_RecType02>();
            TRCINResidentials = new Dictionary<string, List<FedTracing_RecTypeResidential>>();
            TRCINEmployers = new Dictionary<string, List<FedTracing_RecTypeEmployer>>();
        }

        public void AddResidentialRecTypes(params string[] rectypes)
        {
            foreach (var rectype in rectypes)
                TRCINResidentials.Add(rectype, new List<FedTracing_RecTypeResidential>());
        }
        public void AddEmployerRecTypes(params string[] rectypes)
        {
            foreach (var rectype in rectypes)
                TRCINEmployers.Add(rectype, new List<FedTracing_RecTypeEmployer>());
        }
    }
}
