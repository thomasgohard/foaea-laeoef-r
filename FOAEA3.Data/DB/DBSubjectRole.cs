using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSubjectRole : DBbase, ISubjectRoleRepository
    {
        public DBSubjectRole(IDBToolsAsync mainDB) : base(mainDB)
        {

        }
        public async Task<List<SubjectRoleData>> GetAllSubjectRolesAsync()
        {
            return await MainDB.GetAllDataAsync<SubjectRoleData>("SubjectRole", FillSubjectRoleData);
        }

        public async Task<List<string>> GetAssumedRolesForSubjectAsync(string subjectName)
        {
            var assumedRoles = new List<string>();

            foreach (SubjectRoleData subjectRoleData in await GetSubjectRolesAsync(subjectName))
            {
                assumedRoles.Add(subjectRoleData.RoleName);
            }

            return assumedRoles;
        }

        public async Task<List<SubjectRoleData>> GetSubjectRolesAsync(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "SubjectName", subjectName }
            };

            return await MainDB.GetDataFromStoredProcAsync<SubjectRoleData>("UserGetRoleNames", parameters, FillSubjectRoleData);

        }
        private void FillSubjectRoleData(IDBHelperReader rdr, SubjectRoleData data)
        {
            if (rdr.ColumnExists("RoleId"))
                data.RoleId = (int)(rdr["RoleId"]);
            if (rdr.ColumnExists("RoleName"))
                data.RoleName = (string)(rdr["RoleName"]);
            if (rdr.ColumnExists("OrganizationId"))
                data.OrganizationId = (int)(rdr["OrganizationId"]);
            if (rdr.ColumnExists("OrganizationName"))
                data.OrganizationName = (string)(rdr["OrganizationName"]);
            if (rdr.ColumnExists("Description"))
                data.Description = (string)(rdr["Description"]);
            if (rdr.ColumnExists("SubjectName"))
                data.SubjectName = (string)(rdr["SubjectName"]);
            if (rdr.ColumnExists("SubjectId"))
                data.SubjectId = (int)(rdr["SubjectId"]);
        }
    }
}
