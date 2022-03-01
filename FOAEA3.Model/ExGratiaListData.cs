using System;

namespace FOAEA3.Model
{
    public class ExGratiaListData
    {
        public int ID { get; set; }
        public string EnfSrv_Cd { get; set; }
        public string Source_Ref_Nr { get; set; }
        public DateTime DateAdded { get; set; }
        public string Surname { get; set; }
        public string SIN { get; set; }
        public bool BlockSIN { get; set; }
        public bool BlockREF { get; set; }
    }
}
