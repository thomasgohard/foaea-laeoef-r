using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct FedSin_RecType01
    {
        public int RecType;
        public int Cycle;
        public DateTime FileDate;
    }

    public struct FedSin_RecType02
    {
        public int RecType;
        public string dat_Appl_EnfSrvCd;
        public string dat_Appl_CtrlCd;
        public int dat_ValStat_Cd;
        public string dat_SVR_TolCd;
        public string dat_SVR_SIN;
        public int dat_SVR_DOB_TolCd;
        public int dat_SVR_GvnNme_TolCd;
        public int dat_SVR_MddlNme_TolCd;
        public int dat_SVR_SurNme_TolCd;
        public int dat_SVR_MotherNme_TolCd;
        public int dat_SVR_Gendr_TolCd;
    }

    public struct FedSin_RecType99
    {
        public int RecType;
        public int ResponseCnt;
    }

    public class FedSinFileBase
    {
        public FedSin_RecType01 SININ01;
        [JsonConverter(typeof(SingleOrArrayConverter<FedSin_RecType02>))]
        public List<FedSin_RecType02> SININ02;
        public FedSin_RecType99 SININ99;

        public FedSinFileBase()
        {
            SININ02 = new List<FedSin_RecType02>();
        }

    }

}
