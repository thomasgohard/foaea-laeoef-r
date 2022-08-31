using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ICaseManagementRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task CreateCaseManagementAsync(CaseManagementData caseManagementData);
    }
}
