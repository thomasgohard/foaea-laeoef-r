using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FileBroker.Model
{
    public struct MEPSwearing_RecType01
    {
        public string RecType;
        public string Cycle;
        public string FileDate;
        public string EnfSrv_Cd;
    }

    public struct MEPSwearing_RecType61
    {
        public string RecType;
        public string EnfSrv_Cd;
        public string Appl_CtrlCd;
        public string Subm_Affdvt_SubmCd;
        public string Affdvt_Sworn_Dte;
    }

    public struct MEPSwearing_RecType99
    {
        public string RecType;
        public string CountOfDetailRecords;
    }

    public struct MEPSwearing_SwearingDataSet
    {
        public MEPSwearing_RecType01 Header;

        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<MEPSwearing_RecType61>))]
        public List<MEPSwearing_RecType61> SwearingDetail;

        public MEPSwearing_RecType99 Trailer;
    }

    public struct MEPSwearing_SwearingDataSetSingle
    {
        public MEPSwearing_RecType01 Header;

        [DataMember(IsRequired = false)]
        public MEPSwearing_RecType61 SwearingDetail;

        public MEPSwearing_RecType99 Trailer;
    }

    public class MEPSwearingFileData
    {
        public MEPSwearing_SwearingDataSet NewDataSet;
    }

    public class MEPSwearingFileDataSingle
    {
        public MEPSwearing_SwearingDataSetSingle NewDataSet;
    }

}
