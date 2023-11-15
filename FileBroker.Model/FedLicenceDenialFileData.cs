using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FileBroker.Model
{
    public struct FedLicenceDenial_RecType01
    {
        public string RecType;
        public string Cycle;
        public DateTime FileDate;
    }

    public struct FedLicenceDenial_RecType02
    {
        public string RecType;
        public string Appl_EnfSrv_Cd;
        public string Appl_CtrlCd;
        public string RqstStat_Cd;
        public string Source_RefNo;
        public string LicRsp_Comments;
    }

    public struct FedLicenceDenial_RecType99
    {
        public string RecType;
        public int ResponseCnt;
    }

    public struct FedLicenceDenial_DataSet
    {
        public FedLicenceDenial_RecType01 LICIN01;
        [JsonConverter(typeof(SingleOrArrayConverter<FedLicenceDenial_RecType02>))]
        public List<FedLicenceDenial_RecType02> LICIN02;
        public FedLicenceDenial_RecType99 LICIN99;
    }

    public struct FedLicenceDenial_DataSetSingle
    {
        public FedLicenceDenial_RecType01 LICIN01;
        public FedLicenceDenial_RecType02 LICIN02;
        public FedLicenceDenial_RecType99 LICIN99;
    }


    public class FedLicenceDenialFileData
    {
        public FedLicenceDenial_DataSet NewDataSet;

        public FedLicenceDenialFileData()
        {
            NewDataSet.LICIN02 = new List<FedLicenceDenial_RecType02>();
        }
    }

    public class FedLicenceDenialFileDataSingle
    {
        public FedLicenceDenial_DataSetSingle NewDataSet;
    }
}
