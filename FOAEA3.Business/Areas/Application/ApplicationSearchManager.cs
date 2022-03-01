using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using FOAEA3.Business.Security;
using FOAEA3.Resources.Helpers;

namespace FOAEA3.Business.Areas.Application
{
    internal class ApplicationSearchManager
    {
        private readonly IRepositories Repositories;
        private readonly AccessAuditManager AuditManager;

        public ApplicationSearchManager(IRepositories repositories)
        {
            Repositories = repositories; 
            AuditManager = new AccessAuditManager(Repositories);
        }

        public List<ApplicationSearchResultData> Search(QuickSearchData searchCriteria, out int totalCount)
        {
            int accessAuditId = AuditManager.AddAuditHeader(AccessAuditPage.ApplicationSearch);
            AddSearchCriteriaToAccessAudit(accessAuditId, searchCriteria);

            var searchResults = Repositories.ApplicationSearchRepository.QuickSearch(searchCriteria, out totalCount);

            AddSearchResultsToAccessAudit(accessAuditId, searchResults);

            return searchResults;
        }

        private void AddSearchCriteriaToAccessAudit(int accessAuditId, QuickSearchData searchCriteria)
        {
            var items = new Dictionary<AccessAuditElement, string>();

            if (!string.IsNullOrEmpty(searchCriteria.Category)) items.Add(AccessAuditElement.Category, searchCriteria.Category);
            if (!string.IsNullOrEmpty(searchCriteria.ControlCode)) items.Add(AccessAuditElement.CtrlCode, searchCriteria.ControlCode);
            if (!string.IsNullOrEmpty(searchCriteria.EnfService)) items.Add(AccessAuditElement.EnfSrv_Cd, searchCriteria.EnfService);
            if (!string.IsNullOrEmpty(searchCriteria.EnfSourceRefNumber)) items.Add(AccessAuditElement.EnfRefSourceNr, searchCriteria.EnfSourceRefNumber);
            if (!string.IsNullOrEmpty(searchCriteria.FirstName)) items.Add(AccessAuditElement.FirstName, searchCriteria.FirstName);
            if (!string.IsNullOrEmpty(searchCriteria.MiddleName)) items.Add(AccessAuditElement.MiddleName, searchCriteria.MiddleName);
            if (!string.IsNullOrEmpty(searchCriteria.LastName)) items.Add(AccessAuditElement.LastName, searchCriteria.LastName);
            if (!string.IsNullOrEmpty(searchCriteria.JusticeNumber)) items.Add(AccessAuditElement.JusticeNr, searchCriteria.JusticeNumber);
            if (!string.IsNullOrEmpty(searchCriteria.SIN)) items.Add(AccessAuditElement.SIN, searchCriteria.SIN);
            if (!string.IsNullOrEmpty(searchCriteria.Status)) items.Add(AccessAuditElement.Status, searchCriteria.Status);
            if (!string.IsNullOrEmpty(searchCriteria.Subm_SubmCd)) items.Add(AccessAuditElement.Subm_SubmCd, searchCriteria.Subm_SubmCd);
            if (searchCriteria.State != ApplicationState.UNDEFINED) items.Add(AccessAuditElement.State, ((int)searchCriteria.State).ToString());
            if (searchCriteria.Appl_Dbtr_Brth_Dte_Start.HasValue) items.Add(AccessAuditElement.DOBFrom, searchCriteria.Appl_Dbtr_Brth_Dte_Start.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.Appl_Dbtr_Brth_Dte_End.HasValue) items.Add(AccessAuditElement.DOBTo, searchCriteria.Appl_Dbtr_Brth_Dte_End.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.Appl_Create_Dte_Start.HasValue) items.Add(AccessAuditElement.ApplicationReceiptDateFrom, searchCriteria.Appl_Create_Dte_Start.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.Appl_Create_Dte_End.HasValue) items.Add(AccessAuditElement.ApplicationReceiptDateTo, searchCriteria.Appl_Create_Dte_End.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.ActualEnd_Dte_Start.HasValue) items.Add(AccessAuditElement.ApplicationExpiryDateFrom, searchCriteria.ActualEnd_Dte_Start.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.ActualEnd_Dte_End.HasValue) items.Add(AccessAuditElement.ApplicationExpiryDateTo, searchCriteria.ActualEnd_Dte_End.Value.ToString(DateTimeHelper.YYYY_MM_DD));
            if (searchCriteria.ViewAllJurisdiction) items.Add(AccessAuditElement.ViewAllJurisdictions, searchCriteria.ViewAllJurisdiction.ToString());
            if (searchCriteria.SearchOnlySinConfirmed) items.Add(AccessAuditElement.SINConfirmed, searchCriteria.SearchOnlySinConfirmed.ToString());

            AuditManager.AddAuditElements(accessAuditId, items);
        }

        private void AddSearchResultsToAccessAudit(int accessAuditId, List<ApplicationSearchResultData> searchResults)
        {
            foreach (var searchResult in searchResults)
            {
                string key = $"{searchResult.Appl_EnfSrv_Cd.Trim()}-{searchResult.Appl_CtrlCd.Trim()}";
                AuditManager.AddAuditElement(accessAuditId, AccessAuditElement.ApplicationID, key);
            }
        }

    }
}
