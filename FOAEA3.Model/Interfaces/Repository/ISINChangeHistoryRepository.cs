using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISINChangeHistoryRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public bool CreateSINChangeHistory(SINChangeHistoryData data);

    }
}
