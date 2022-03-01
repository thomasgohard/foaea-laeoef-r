using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace FOAEA3.Data.DB
{
    internal class DBSubjectRole: DBbase, ISubjectRoleRepository
    {
        public DBSubjectRole(IDBTools mainDB) : base(mainDB)
        {

        }
        public List<SubjectRoleData> GetAllSubjectRoles()
        {
            return MainDB.GetAllData<SubjectRoleData>("SubjectRole", FillSubjectRoleData);
        }

        public List<string> GetAssumedRolesForSubject(string subjectName)
        {
            var assumedRoles = new List<string>();

            foreach (SubjectRoleData subjectRoleData in GetSubjectRoles(subjectName))
            {
                assumedRoles.Add(subjectRoleData.RoleName);
            }

            return assumedRoles;
        }

        public List<SubjectRoleData> GetSubjectRoles(string subjectName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "SubjectName", subjectName }
            };

            return MainDB.GetDataFromStoredProc<SubjectRoleData>("UserGetRoleNames", parameters, FillSubjectRoleData);
            
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
