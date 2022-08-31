using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    internal class SubmitterProfileManager
    {
        private readonly IRepositories Repositories;

        public SubmitterProfileManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        public async Task<SubmitterProfileData> GetSubmitterProfileAsync(string submitterCode)
        {
            return await Repositories.SubmitterProfileRepository.GetSubmitterProfileAsync(submitterCode);
        }

    }
}
