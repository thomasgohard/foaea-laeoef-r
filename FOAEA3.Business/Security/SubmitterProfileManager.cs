using FOAEA3.Model;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Business.Security
{
    internal class SubmitterProfileManager
    {
        private readonly IRepositories Repositories;

        internal SubmitterProfileManager(IRepositories repositories)
        {
            Repositories = repositories;
        }

        internal SubmitterProfileData GetSubmitterProfile(string submitterCode)
        {
            return Repositories.SubmitterProfileRepository.GetSubmitterProfile(submitterCode);
        }

    }
}
