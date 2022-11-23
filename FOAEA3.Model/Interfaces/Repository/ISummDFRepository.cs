using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummDFRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public DataList<SummDF_Data> GetSummDFList(int summFAFR_Id);

    }
}
