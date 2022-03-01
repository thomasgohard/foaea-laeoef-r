using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    class InMemorySubmitterProfile : ISubmitterProfileRepository
    {
        public InMemorySubmitterProfile()
        {

        }

        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public SubmitterProfileData GetSubmitterProfile(string subjectName)
        {
            throw new NotImplementedException();
        }
    }
}
