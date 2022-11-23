using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Administration
{
    public class SubjectManager
    {
        private IRepositories DB { get; }
        public SubjectManager(IRepositories repositories)
        {
            DB = repositories;
        }

        public async Task<SubjectData> GetSubjectByConfirmationCodeAsync(string confirmationCode)
        {
            return await DB.SubjectTable.GetSubjectByConfirmationCodeAsync(confirmationCode);
        }

    }
}
