using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Security
{
    public class AccessAuditManager
    {
        private readonly IRepositories DB;
        public readonly List<string> Errors;

        public AccessAuditManager(IRepositories repositories)
        {
            DB = repositories;
            Errors = new List<string>();
        }

        public async Task<int> AddAuditHeaderAsync(AccessAuditPage page)
        {
            string subject_submitter = $"{DB.CurrentUser} ({DB.CurrentSubmitter})";

            return await DB.AccessAuditTable.SaveDataPageInfoAsync(page, subject_submitter);
        }

        public async Task AddAuditElementAsync(int headerId, AccessAuditElement elementType, string elementValue)
        {
            var accessAuditElements = new Dictionary<AccessAuditElement, AccessAuditElementTypeData>();
            var allElementTypes = await DB.AccessAuditTable.GetAllElementAccessTypeAsync();

            foreach (var thisElementType in allElementTypes)
            {
                if (Enum.IsDefined(typeof(AccessAuditElement), thisElementType.AccessAuditDataElementValueType_ID))
                    accessAuditElements.Add((AccessAuditElement)thisElementType.AccessAuditDataElementValueType_ID, thisElementType);
                else
                    Errors.Add($"Undefined access audit element type: {thisElementType.AccessAuditDataElementValueType_ID} [{thisElementType.ElementName}]");
            }

            string elementName = accessAuditElements[elementType].ElementName;
            await DB.AccessAuditTable.SaveDataValueAsync(headerId, elementName, elementValue);
        }

        public async Task AddAuditElementsAsync(int headerId, Dictionary<AccessAuditElement, string> elements)
        {
            foreach (var (elementType, elementValue) in elements)
                await AddAuditElementAsync(headerId, elementType, elementValue);
        }

    }
}
