using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Resources.Helpers;
using System.Text;

namespace FOAEA3.Data.DB
{

    internal class DBApplicationSearch : DBbase, IApplicationSearchRepository
    {
        public DBApplicationSearch(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount, 
                                                             int page = 1, int perPage = 1000)
        {
            string firstName = searchData.FirstName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string middleName = searchData.MiddleName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string lastName = searchData.LastName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string justiceNumber = searchData.JusticeNumber?.Trim()?.FixApostropheForSQL();

            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(searchData.EnfService)) parameters.Add("Appl_EnfSrv_Cd", searchData.EnfService);
            if (!string.IsNullOrEmpty(searchData.ControlCode)) parameters.Add("Appl_CtrlCd", searchData.ControlCode);
            if (!string.IsNullOrEmpty(searchData.Category)) parameters.Add("AppCtgy_Cd", searchData.Category);
            if (!string.IsNullOrEmpty(firstName)) parameters.Add("Appl_Dbtr_FrstNme", firstName);
            if (!string.IsNullOrEmpty(middleName)) parameters.Add("Appl_Dbtr_MddleNme", middleName);
            if (!string.IsNullOrEmpty(lastName)) parameters.Add("Appl_Dbtr_SurNme", lastName);
            if (!string.IsNullOrEmpty(searchData.SIN)) parameters.Add("Appl_Dbtr_Entrd_SIN", searchData.SIN);
            if (searchData.State != ApplicationState.UNDEFINED) parameters.Add("AppLiSt_Cd", searchData.State);
            if (!string.IsNullOrEmpty(searchData.Status)) parameters.Add("ActvSt_Cd", searchData.Status);
            if (!string.IsNullOrEmpty(searchData.EnfSourceRefNumber)) parameters.Add("SourceReference", searchData.EnfSourceRefNumber);
            if (!string.IsNullOrEmpty(justiceNumber)) parameters.Add("JusticeNr", justiceNumber);

            if (!string.IsNullOrEmpty(searchData.Subm_SubmCd)) 
                parameters.Add("Subm_SubmCd", searchData.Subm_SubmCd);
            else
            {
                var searchRange = BuildSearchRangeForSubmitter(MainDB.Submitter);
                if (!string.IsNullOrEmpty(searchRange))
                    parameters.Add("Subm_SubmCd", searchRange);
            }

            if (searchData.Appl_Dbtr_Brth_Dte_Start.HasValue && searchData.Appl_Dbtr_Brth_Dte_End.HasValue)
            {
                parameters.Add("Appl_Dbtr_Brth_Dte_Start", searchData.Appl_Dbtr_Brth_Dte_Start);
                parameters.Add("Appl_Dbtr_Brth_Dte_End", searchData.Appl_Dbtr_Brth_Dte_End);
            }

            if (searchData.Appl_Create_Dte_Start.HasValue && searchData.Appl_Create_Dte_End.HasValue)
            {
                parameters.Add("Appl_Create_Dte_Start", searchData.Appl_Create_Dte_Start);
                parameters.Add("Appl_Create_Dte_End", searchData.Appl_Create_Dte_End);
            }

            if (searchData.ActualEnd_Dte_Start.HasValue && searchData.ActualEnd_Dte_End.HasValue)
            {
                parameters.Add("ActualEnd_Dte_Start", searchData.ActualEnd_Dte_Start);
                parameters.Add("ActualEnd_Dte_End", searchData.ActualEnd_Dte_End);
            }

            if (MainDB.Submitter.IsInternalUser())
            {
                parameters.Add("IsInternalUser", true);
                if (searchData.SearchOnlySinConfirmed) parameters.Add("OnlySINConfirmed", searchData.SearchOnlySinConfirmed);
            }

            parameters.Add("Page", page);
            parameters.Add("PerPage", perPage);

            var data = MainDB.GetDataFromStoredProc<ApplicationSearchResultData>("Appl_Search", parameters, FillDataFromReader);

            totalCount = MainDB.LastReturnValue;

            return data;
        }

        private string BuildSearchRangeForSubmitter(string submitter)
        {
            var searchRange = "";

            var submitterDB = new DBSubmitterProfile(MainDB);

            var submitterData = submitterDB.GetSubmitterProfile(submitter);
            string submClass = submitterData.Subm_Class.ToLower();

            if (submClass == "sm")
                return "";

            if (submClass.NotIn("am", "fm", "ac", "fc"))
            {
                searchRange = submitter.Substring(0, 2);
                if (submClass.In("eo", "ro", "fo")) {
                    searchRange += "%" + submitter.Substring(3, 1);
                }
                searchRange += "%";
            }

            return searchRange;
        }

        private void FillDataFromReader(IDBHelperReader rdr, ApplicationSearchResultData data)
        {
            data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string;
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Subm_Recpt_SubmCd = rdr["Subm_Recpt_SubmCd"] as string;
            data.Appl_Rcptfrm_Dte = (DateTime)rdr["Appl_Rcptfrm_Dte"];
            data.Appl_Expiry_Dte = rdr["Appl_Expiry_Dte"] as DateTime?;
            data.Appl_Dbtr_SurNme = rdr["Appl_Dbtr_SurNme"] as string;
            data.Appl_Source_RfrNr = rdr["Appl_Source_RfrNr"] as string; // can be null 
            data.Appl_JusticeNr = rdr["Appl_JusticeNr"] as string; // can be null 
            data.ActvSt_Cd = ReferenceData.Instance().ActiveStatuses[rdr["ActvSt_Cd"] as string].ActvSt_Txt_E;
            data.AppLiSt_Cd = (ApplicationState)rdr["AppLiSt_Cd"];
        }

    }
}
