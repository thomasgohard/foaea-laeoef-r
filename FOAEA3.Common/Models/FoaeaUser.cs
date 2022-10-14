using FOAEA3.Model;
using System.Collections.Generic;

namespace FOAEA3.Common.Models
{
    public class FoaeaUser
    {
        public string SubjectName { get; set; }
        public SubmitterData Submitter { get; set; }
        public string OfficeCode { get; set; }
        public List<string> UserRoles { get; set; }

        public FoaeaUser()
        {
            UserRoles = new List<string>();
        }

        public bool HasRole(string role)
        {
            return UserRoles.Contains(role);
        }

        public bool IsSameEnfService(string enfService)
        {
            return Submitter.EnfSrv_Cd == enfService;
        }

        public bool IsSameOffice(string submitterCode)
        {
            return OfficeCode == submitterCode.Substring(3, 1);
        }
    }
}
