using FOAEA3.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummFAFRRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<SummFAFR_Data>> GetSummFaFrAsync(int summFAFR_Id);
        Task<DataList<SummFAFR_Data>> GetSummFaFrListAsync(List<SummFAFR_DE_Data> summFAFRs);
    }
}
