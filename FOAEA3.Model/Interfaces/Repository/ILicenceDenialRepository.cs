using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ILicenceDenialRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        LicenceDenialData GetLicenceDenialData(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
