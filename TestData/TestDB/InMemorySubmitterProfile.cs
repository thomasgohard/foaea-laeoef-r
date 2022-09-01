using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    class InMemorySubmitterProfile : ISubmitterProfileRepository
    {
        public InMemorySubmitterProfile()
        {

        }

        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<SubmitterProfileData> GetSubmitterProfileAsync(string submitterCode)
        {
            throw new NotImplementedException();
        }
    }
}
