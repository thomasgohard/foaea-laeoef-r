using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ICaseManagementRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        void CreateCaseManagement(CaseManagementData caseManagementData);
    }
}
