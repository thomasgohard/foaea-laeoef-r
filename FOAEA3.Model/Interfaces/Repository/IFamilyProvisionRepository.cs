using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IFamilyProvisionRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<FamilyProvisionData>> GetFamilyProvisionsAsync();
    }
}
