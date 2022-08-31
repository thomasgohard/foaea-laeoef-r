using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    public class AccessAuditManager
    {
        private readonly IRepositories DB;
        private readonly Dictionary<AccessAuditElement, AccessAuditElementTypeData> AccessAuditElements;
        public readonly List<string> Errors;

        public AccessAuditManager(IRepositories repositories)
        {
            DB = repositories;
            Errors = new List<string>();

            // load all values from database
            AccessAuditElements = new Dictionary<AccessAuditElement, AccessAuditElementTypeData>();

            var allElementTypes = DB.AccessAuditTable.GetAllElementAccessTypeAsync().Result;
            foreach (var elementType in allElementTypes)
            {
                if (Enum.IsDefined(typeof(AccessAuditElement), elementType.AccessAuditDataElementValueType_ID))
                    AccessAuditElements.Add((AccessAuditElement)elementType.AccessAuditDataElementValueType_ID, elementType);
                else
                    Errors.Add($"Undefined access audit element type: {elementType.AccessAuditDataElementValueType_ID} [{elementType.ElementName}]");
            }
        }

        public async Task<int> AddAuditHeaderAsync(AccessAuditPage page)
        {
            string subject_submitter = $"{DB.CurrentUser} ({DB.CurrentSubmitter})";

            return await DB.AccessAuditTable.SaveDataPageInfoAsync(page, subject_submitter);
        }

        public async Task AddAuditElementAsync(int headerId, AccessAuditElement elementType, string elementValue)
        {
            string elementName = AccessAuditElements[elementType].ElementName;
            await DB.AccessAuditTable.SaveDataValueAsync(headerId, elementName, elementValue);
        }

        public async Task AddAuditElementsAsync(int headerId, Dictionary<AccessAuditElement, string> elements)
        {
            foreach (var (elementType, elementValue) in elements)
                await AddAuditElementAsync(headerId, elementType, elementValue);
        }

    }
}
