using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class ProductionAuditData
    {
        public DateTime Process_dte { get; set; }
        public string Process_name { get; set; }
        public string Description { get; set; }
        public string Audience { get; set; }
        public DateTime? Compl_dte { get; set; }
    }
}
