using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FOAEA3.Business.Security
{
    public class AccessAuditManager
    {
        private readonly IRepositories Repositories;
        private readonly Dictionary<AccessAuditElement, AccessAuditElementTypeData> AccessAuditElements;
        public readonly List<string> Errors;

        public AccessAuditManager(IRepositories repositories)
        {
            Repositories = repositories;
            Errors = new List<string>();

            // load all values from database
            AccessAuditElements = new Dictionary<AccessAuditElement, AccessAuditElementTypeData>();

            var allElementTypes = Repositories.AccessAuditRepository.GetAllElementAccessType();
            foreach (var elementType in allElementTypes)
            {
                if (Enum.IsDefined(typeof(AccessAuditElement), elementType.AccessAuditDataElementValueType_ID))
                    AccessAuditElements.Add((AccessAuditElement)elementType.AccessAuditDataElementValueType_ID, elementType);
                else
                    Errors.Add($"Undefined access audit element type: {elementType.AccessAuditDataElementValueType_ID} [{elementType.ElementName}]");
            }
        }

        public int AddAuditHeader(AccessAuditPage page)
        {
            string subject_submitter = $"{Repositories.CurrentUser} ({Repositories.CurrentSubmitter})";

            return Repositories.AccessAuditRepository.SaveDataPageInfo(page, subject_submitter);
        }

        public void AddAuditElement(int headerId, AccessAuditElement elementType, string elementValue)
        {
            string elementName = AccessAuditElements[elementType].ElementName;
            Repositories.AccessAuditRepository.SaveDataValue(headerId, elementName, elementValue);
        }

        public void AddAuditElements(int headerId, Dictionary<AccessAuditElement, string> elements)
        {
            foreach (var element in elements)
                AddAuditElement(headerId, element.Key, element.Value);
        }

    }
}
