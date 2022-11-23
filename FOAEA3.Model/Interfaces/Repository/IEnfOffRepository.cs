using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IEnfOffRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<EnfOffData>> GetEnfOffAsync(string enfOffName = null, string enfOffCode = null, string province = null, string enfServCode = null);
    }
}
