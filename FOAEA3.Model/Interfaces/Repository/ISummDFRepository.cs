using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummDFRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        DataList<SummDF_Data> GetSummDFList(int summFAFR_Id);
        Task<bool> ActiveDFExistsForSin(string confirmedSIN);
    }
}
