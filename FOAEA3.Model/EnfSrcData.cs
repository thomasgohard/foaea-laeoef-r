using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model
{
    public class EnfSrcData
    {
        public string EnfSrc_Cd { get; set; }
        public string EnfSrc_LocCd { get; set; }
        public string EnfSrc_SubLocCd { get; set; }
        public string EnfSrc_Nme { get; set; }
        public short? EnfSrc_Tel_AreaC { get; set; }
        public string EnfSrc_Contact { get; set; }
        public int? EnfSrc_TelNr { get; set; }
        public int? EnfSrc_TelEx { get; set; }
        public short? EnfSrc_Fax_AreaC { get; set; }
        public int? EnfSrc_FaxNr { get; set; }
        public string EnfSrc_Addr_Ln { get; set; }
        public string EnfSrc_Addr_Ln1 { get; set; }
        public string EnfSrc_Addr_CityNme { get; set; }
        public string EnfSrc_Addr_PrvCd { get; set; }
        public string EnfSrc_Addr_CtryCd { get; set; }
        public string EnfSrc_Addr_PCd { get; set; }
        public string EnfSrc_Fin_VndrCd { get; set; }
        public string EnfSrc_Comments { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
