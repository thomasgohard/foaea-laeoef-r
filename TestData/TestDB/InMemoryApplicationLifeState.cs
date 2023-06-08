using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationLifeState : IApplicationLifeStateRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStates()
        {
            var result = new DataList<ApplicationLifeStateData>();

            return Task.FromResult(result);
        }
    }
}
