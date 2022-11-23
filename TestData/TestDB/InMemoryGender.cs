using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryGender : IGenderRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public MessageDataList Messages => new();

        public Task<DataList<GenderData>> GetGendersAsync()
        {
            var result = new DataList<GenderData>();

            return Task.FromResult(result);
        }
    }
}
