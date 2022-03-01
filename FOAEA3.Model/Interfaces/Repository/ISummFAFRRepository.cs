using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummFAFRRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public DataList<SummFAFR_Data> GetSummFaFr(int summFAFR_Id);
        public DataList<SummFAFR_Data> GetSummFaFrList(List<SummFAFR_DE_Data> summFAFRs);
    }
}
