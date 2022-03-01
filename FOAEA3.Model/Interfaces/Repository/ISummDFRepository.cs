using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummDFRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public DataList<SummDF_Data> GetSummDFList(int summFAFR_Id);

    }
}
