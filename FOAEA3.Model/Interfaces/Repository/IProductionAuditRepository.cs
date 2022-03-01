using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IProductionAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void Insert(string processName, string description, string audience, DateTime? completedDate = null);
    }
}
