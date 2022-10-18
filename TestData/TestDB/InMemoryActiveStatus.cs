using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryActiveStatus : IActiveStatusRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public MessageDataList Messages => new();

        public Task<DataList<ActiveStatusData>> GetActiveStatusAsync()
        {
            var result = new DataList<ActiveStatusData>();

            return Task.FromResult(result);
        }
    }
}
