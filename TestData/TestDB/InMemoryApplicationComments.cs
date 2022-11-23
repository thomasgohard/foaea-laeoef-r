using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationComments : IApplicationCommentsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public MessageDataList Messages => new();

        public Task<DataList<ApplicationCommentsData>> GetApplicationCommentsAsync()
        {
            var result = new DataList<ApplicationCommentsData>();

            return Task.FromResult(result);
        }
    }
}
