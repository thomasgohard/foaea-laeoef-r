using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{

    internal class DBApplicationSearch : DBbase, IApplicationSearchRepository
    {
        public string LastError { get; set; }

        public DBApplicationSearch(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<ApplicationSearchResultData> QuickSearch(QuickSearchData searchData, out int totalCount,
                                                             int page = 1, int perPage = 1000, 
                                                             string orderBy = "EnforcementService, ControlCode")
        {
            var emptyResult = new List<ApplicationSearchResultData>();

            LastError = string.Empty;

            if (!orderBy.IsValidOrderByClause())
            {
                LastError = "Invalid Order By Clause: " + orderBy;
                totalCount = 0;
                return emptyResult;
            }

            var sqlBase = @"SELECT  A.AppCtgy_Cd [Category], 
                                    A.Appl_EnfSrv_Cd [EnforcementService], 
                                    A.Subm_SubmCd [Submitter], 
                                    A.Appl_CtrlCd [ControlCode], 
                                    A.Subm_Recpt_SubmCd [RecipientSubmitter], 
                                    A.Appl_Rcptfrm_Dte [FormReceiptDate], 
                                    COALESCE(NULLIF(S.ActualEnd_Dte, ''), S.End_Dte) [ExpiryDate],
                                    A.Appl_Dbtr_SurNme [DebtorSurname],
                                    A.Appl_Source_RfrNr [ReferenceNumber],
                                    A.Appl_JusticeNr [JusticeNumber],
                                    A.ActvSt_Cd [ActiveState],
                                    A.AppLiSt_Cd [ApplicationLifeState]";

            var sqlTables = @" FROM    Appl AS A
                               LEFT JOIN SummSmry AS S ON S.Appl_CtrlCd = A.Appl_CtrlCd AND S.Appl_EnfSrv_Cd = A.Appl_EnfSrv_Cd ";

            string firstName = searchData.FirstName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string middleName = searchData.MiddleName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string lastName = searchData.LastName?.Trim()?.FixApostropheForSQL()?.FixWildcardForSQL();
            string justiceNumber = searchData.JusticeNumber?.Trim()?.FixApostropheForSQL();

            var parameters = new Dictionary<string, object>();

            string sqlWhere = string.Empty;

            if (!string.IsNullOrEmpty(searchData.EnfService))
            {
                parameters.Add("Appl_EnfSrv_Cd", searchData.EnfService);
                sqlWhere = AppendWhereClause(sqlWhere, "@Appl_EnfSrv_Cd = A.Appl_EnfSrv_Cd");
            }
            if (!string.IsNullOrEmpty(searchData.ControlCode))
            {
                parameters.Add("Appl_CtrlCd", searchData.ControlCode);
                sqlWhere = AppendWhereClause(sqlWhere, "@Appl_CtrlCd = A.Appl_CtrlCd");
            }
            if (!string.IsNullOrEmpty(searchData.Category))
            {
                parameters.Add("AppCtgy_Cd", searchData.Category);
                sqlWhere = AppendWhereClause(sqlWhere, "@AppCtgy_Cd = A.AppCtgy_Cd");
            }
            if (!string.IsNullOrEmpty(firstName))
            {
                parameters.Add("Appl_Dbtr_FrstNme", firstName);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Appl_Dbtr_FrstNme LIKE @Appl_Dbtr_FrstNme");
            }
            if (!string.IsNullOrEmpty(middleName))
            {
                parameters.Add("Appl_Dbtr_MddleNme", middleName);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Appl_Dbtr_MddleNme LIKE @Appl_Dbtr_MddleNme");
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                parameters.Add("Appl_Dbtr_SurNme", lastName);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Appl_Dbtr_SurNme LIKE @Appl_Dbtr_SurNme");
            }
            if (!string.IsNullOrEmpty(searchData.SIN))
            {
                parameters.Add("Appl_Dbtr_Entrd_SIN", searchData.SIN);
                if (MainDB.Submitter.IsInternalUser())
                {
                    parameters.Add("IsInternalUser", true);
                    if (searchData.SearchOnlySinConfirmed) parameters.Add("OnlySINConfirmed", searchData.SearchOnlySinConfirmed);
                }
                sqlWhere = AppendWhereClause(sqlWhere, @"(
                                                            ((@IsInternalUser = 0) AND (@Appl_Dbtr_Entrd_SIN = A.Appl_Dbtr_Entrd_SIN))
                                                            OR
                                                            ((@IsInternalUser = 1) AND (
                                                                                        ((@OnlySinConfirmed = 0) AND (@Appl_Dbtr_Entrd_SIN = A.Appl_Dbtr_Entrd_SIN)) 
                                                                                        OR 
                                                                                        (@Appl_Dbtr_Entrd_SIN = A.Appl_Dbtr_Cnfrmd_SIN)
                                                                                        )
                                                            )
                                                        )");
            }
            if (searchData.State != ApplicationState.UNDEFINED)
            {
                parameters.Add("AppLiSt_Cd", searchData.State);
                sqlWhere = AppendWhereClause(sqlWhere, "@AppLiSt_Cd = A.AppLiSt_Cd");
            }
            if (!string.IsNullOrEmpty(searchData.Status))
            {
                parameters.Add("ActvSt_Cd", searchData.Status);
                sqlWhere = AppendWhereClause(sqlWhere, "@ActvSt_Cd = A.ActvSt_Cd");
            }
            if (!string.IsNullOrEmpty(searchData.EnfSourceRefNumber))
            {
                parameters.Add("SourceReference", searchData.EnfSourceRefNumber);
                sqlWhere = AppendWhereClause(sqlWhere, "@SourceReference = A.Appl_Source_RfrNr");
            }
            if (!string.IsNullOrEmpty(justiceNumber))
            {
                parameters.Add("JusticeNr", justiceNumber);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Appl_JusticeNr LIKE @JusticeNr");
            }

            if (!string.IsNullOrEmpty(searchData.Subm_SubmCd))
            {
                parameters.Add("Subm_SubmCd", searchData.Subm_SubmCd);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Subm_SubmCd LIKE @Subm_SubmCd");
            }
            else
            {
                var searchRange = BuildSearchRangeForSubmitter(MainDB.Submitter);
                if (!string.IsNullOrEmpty(searchRange))
                {
                    parameters.Add("Subm_SubmCd", searchRange);
                    sqlWhere = AppendWhereClause(sqlWhere, "A.Subm_SubmCd LIKE @Subm_SubmCd");
                }
            }

            if (searchData.Appl_Dbtr_Brth_Dte_Start.HasValue && searchData.Appl_Dbtr_Brth_Dte_End.HasValue)
            {
                parameters.Add("Appl_Dbtr_Brth_Dte_Start", searchData.Appl_Dbtr_Brth_Dte_Start);
                parameters.Add("Appl_Dbtr_Brth_Dte_End", searchData.Appl_Dbtr_Brth_Dte_End);
                sqlWhere = AppendWhereClause(sqlWhere, "A.Appl_Dbtr_Brth_Dte >= @Appl_Dbtr_Brth_Dte_Start AND A.Appl_Dbtr_Brth_Dte <= @Appl_Dbtr_Brth_Dte_End");
            }

            if (searchData.Appl_Create_Dte_Start.HasValue && searchData.Appl_Create_Dte_End.HasValue)
            {
                parameters.Add("Appl_Create_Dte_Start", searchData.Appl_Create_Dte_Start);
                parameters.Add("Appl_Create_Dte_End", searchData.Appl_Create_Dte_End);
                sqlWhere = AppendWhereClause(sqlWhere, "CONVERT(DATE, A.Appl_Create_Dte) >= @Appl_Create_Dte_Start AND CONVERT(DATE, A.Appl_Create_Dte) <= @Appl_Create_Dte_End");
            }

            if (searchData.ActualEnd_Dte_Start.HasValue && searchData.ActualEnd_Dte_End.HasValue)
            {
                parameters.Add("ActualEnd_Dte_Start", searchData.ActualEnd_Dte_Start);
                parameters.Add("ActualEnd_Dte_End", searchData.ActualEnd_Dte_End);
                sqlWhere = AppendWhereClause(sqlWhere, "CONVERT(DATE, IsNull(S.ActualEnd_Dte, S.End_Dte)) >= @ActualEnd_Dte_Start AND CONVERT(DATE, IsNull(S.ActualEnd_Dte, S.End_Dte)) <= @ActualEnd_Dte_End");
            }

            string sqlSort = @" ORDER BY " + orderBy +
                             @" OFFSET ((@Page-1) * @PerPage) ROWS 
                                FETCH NEXT @PerPage ROWS ONLY"; 

            parameters.Add("Page", page);
            parameters.Add("PerPage", perPage);

            string sql = sqlBase + sqlTables + sqlWhere + sqlSort;

            var data = MainDB.GetDataFromSql<ApplicationSearchResultData>(sql, parameters, FillDataFromReader);
            if (!string.IsNullOrEmpty(MainDB.LastError))
            {
                LastError = MainDB.LastError;
                totalCount = 0;
                return emptyResult;
            }

            string sqlCount = "SELECT Count(*) " + sqlTables + sqlWhere;

            totalCount = MainDB.GetDataFromSqlSingleValue<int>(sqlCount, parameters);
            if (!string.IsNullOrEmpty(MainDB.LastError))
            {
                LastError = MainDB.LastError;
                totalCount = 0;
                return emptyResult;
            }

            return data;
        }

        private static string AppendWhereClause(string sqlWhere, string clause)
        {
            if (string.IsNullOrEmpty(sqlWhere))
                return sqlWhere = " WHERE " + clause;
            else
                return sqlWhere += " AND " + clause;
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
                if (submClass.In("eo", "ro", "fo"))
                {
                    searchRange += "%" + submitter.Substring(3, 1);
                }
                searchRange += "%";
            }

            return searchRange;
        }

        private void FillDataFromReader(IDBHelperReader rdr, ApplicationSearchResultData data)
        {
            data.AppCtgy_Cd = rdr["Category"] as string;
            data.Appl_EnfSrv_Cd = rdr["EnforcementService"] as string;
            data.Subm_SubmCd = rdr["Submitter"] as string;
            data.Appl_CtrlCd = rdr["ControlCode"] as string;
            data.Subm_Recpt_SubmCd = rdr["RecipientSubmitter"] as string;
            data.Appl_Rcptfrm_Dte = (DateTime)rdr["FormReceiptDate"];
            data.Appl_Expiry_Dte = rdr["ExpiryDate"] as DateTime?;
            data.Appl_Dbtr_SurNme = rdr["DebtorSurname"] as string;
            data.Appl_Source_RfrNr = rdr["ReferenceNumber"] as string; // can be null 
            data.Appl_JusticeNr = rdr["JusticeNumber"] as string; // can be null 
            data.ActvSt_Cd = ReferenceData.Instance().ActiveStatuses[rdr["ActiveState"] as string].ActvSt_Txt_E;
            data.AppLiSt_Cd = (ApplicationState)rdr["ApplicationLifeState"];
        }

    }
}
