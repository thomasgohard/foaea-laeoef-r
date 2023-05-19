using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class SubmitterProfileManager
    {
        private readonly IRepositories DB;

        public SubmitterProfileManager(IRepositories repositories)
        {
            DB = repositories;
        }

        public async Task<SubmitterProfileData> GetSubmitterProfileAsync(string submitterCode)
        {
            return await DB.SubmitterProfileTable.GetSubmitterProfileAsync(submitterCode);
        }

    }
}
