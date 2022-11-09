using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareAppl
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories repositories2, IRepositories repositories3,
                                                          string enfSrv, string ctrlCd, string category)
        {
            var diffs = new List<DiffData>();

            string key = ApplKey.MakeKey(enfSrv, ctrlCd) + " " + category + " ";

            var appl2 = await repositories2.ApplicationTable.GetApplicationAsync(enfSrv, ctrlCd);
            var appl3 = await repositories3.ApplicationTable.GetApplicationAsync(enfSrv, ctrlCd);

            if ((appl2 is null) && (appl3 is null))
                return diffs;

            if (appl2 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "", badValue: "Not found in FOAEA 2!"));
                return diffs;
            }

            if (appl3 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "Not found in FOAEA 3!", badValue: ""));
                return diffs;
            }

            if (appl2.Appl_EnfSrv_Cd != appl3.Appl_EnfSrv_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_EnfSrv_Cd", goodValue: appl2.Appl_EnfSrv_Cd, badValue: appl3.Appl_EnfSrv_Cd));
            if (appl2.Appl_CtrlCd != appl3.Appl_CtrlCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_CtrlCd", goodValue: appl2.Appl_CtrlCd, badValue: appl3.Appl_CtrlCd));
            if (appl2.Subm_SubmCd != appl3.Subm_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_SubmCd", goodValue: appl2.Subm_SubmCd, badValue: appl3.Subm_SubmCd));
            if (appl2.Subm_Recpt_SubmCd != appl3.Subm_Recpt_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_Recpt_SubmCd", goodValue: appl2.Subm_Recpt_SubmCd, badValue: appl3.Subm_Recpt_SubmCd));
            if (appl2.Appl_Lgl_Dte != appl3.Appl_Lgl_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Lgl_Dte", goodValue: appl2.Appl_Lgl_Dte, badValue: appl3.Appl_Lgl_Dte));

            if (appl2.Appl_CommSubm_Text != appl3.Appl_CommSubm_Text)
            {
                // known "bug" in old system where comments were not saved when application is cancelled, but new system does save it as it should -- only BC uses this
                if (appl2.Appl_EnfSrv_Cd.Trim().ToUpper() != "BC01")
                    diffs.Add(new DiffData(tableName, key: key, colName: "Appl_CommSubm_Text", goodValue: appl2.Appl_CommSubm_Text, badValue: appl3.Appl_CommSubm_Text));
            }

            if (appl2.Appl_Rcptfrm_Dte.Date != appl3.Appl_Rcptfrm_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Rcptfrm_Dte", goodValue: appl2.Appl_Rcptfrm_Dte, badValue: appl3.Appl_Rcptfrm_Dte));
            if (appl2.Appl_Group_Batch_Cd != appl3.Appl_Group_Batch_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Group_Batch_Cd", goodValue: appl2.Appl_Group_Batch_Cd, badValue: appl3.Appl_Group_Batch_Cd));
            if (appl2.Appl_Source_RfrNr != appl3.Appl_Source_RfrNr) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Source_RfrNr", goodValue: appl2.Appl_Source_RfrNr, badValue: appl3.Appl_Source_RfrNr));
            if (appl2.Subm_Affdvt_SubmCd != appl3.Subm_Affdvt_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "Subm_Affdvt_SubmCd", goodValue: appl2.Subm_Affdvt_SubmCd, badValue: appl3.Subm_Affdvt_SubmCd));
            if (appl2.Appl_RecvAffdvt_Dte?.Date != appl3.Appl_RecvAffdvt_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_RecvAffdvt_Dte", goodValue: appl2.Appl_RecvAffdvt_Dte, badValue: appl3.Appl_RecvAffdvt_Dte));
            if (appl2.Appl_Affdvt_DocTypCd != appl3.Appl_Affdvt_DocTypCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Affdvt_DocTypCd", goodValue: appl2.Appl_Affdvt_DocTypCd, badValue: appl3.Appl_Affdvt_DocTypCd));
            if (appl2.Appl_JusticeNr != appl3.Appl_JusticeNr) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_JusticeNr", goodValue: appl2.Appl_JusticeNr, badValue: appl3.Appl_JusticeNr));
            if (appl2.Appl_Crdtr_FrstNme != appl3.Appl_Crdtr_FrstNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Crdtr_FrstNme", goodValue: appl2.Appl_Crdtr_FrstNme, badValue: appl3.Appl_Crdtr_FrstNme));
            if (appl2.Appl_Crdtr_MddleNme != appl3.Appl_Crdtr_MddleNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Crdtr_MddleNme", goodValue: appl2.Appl_Crdtr_MddleNme, badValue: appl3.Appl_Crdtr_MddleNme));
            if (appl2.Appl_Crdtr_SurNme != appl3.Appl_Crdtr_SurNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Crdtr_SurNme", goodValue: appl2.Appl_Crdtr_SurNme, badValue: appl3.Appl_Crdtr_SurNme));
            if (appl2.Appl_Dbtr_FrstNme != appl3.Appl_Dbtr_FrstNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_FrstNme", goodValue: appl2.Appl_Dbtr_FrstNme, badValue: appl3.Appl_Dbtr_FrstNme));
            if (appl2.Appl_Dbtr_MddleNme != appl3.Appl_Dbtr_MddleNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_MddleNme", goodValue: appl2.Appl_Dbtr_MddleNme, badValue: appl3.Appl_Dbtr_MddleNme));
            if (appl2.Appl_Dbtr_SurNme != appl3.Appl_Dbtr_SurNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_SurNme", goodValue: appl2.Appl_Dbtr_SurNme, badValue: appl3.Appl_Dbtr_SurNme));
            if (appl2.Appl_Dbtr_Parent_SurNme_Birth != appl3.Appl_Dbtr_Parent_SurNme_Birth) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Parent_SurNme_Birth", goodValue: appl2.Appl_Dbtr_Parent_SurNme_Birth, badValue: appl3.Appl_Dbtr_Parent_SurNme_Birth));
            if (appl2.Appl_Dbtr_Brth_Dte != appl3.Appl_Dbtr_Brth_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Brth_Dte", goodValue: appl2.Appl_Dbtr_Brth_Dte, badValue: appl3.Appl_Dbtr_Brth_Dte));
            if (appl2.Appl_Dbtr_LngCd != appl3.Appl_Dbtr_LngCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_LngCd", goodValue: appl2.Appl_Dbtr_LngCd, badValue: appl3.Appl_Dbtr_LngCd));
            if (appl2.Appl_Dbtr_Gendr_Cd != appl3.Appl_Dbtr_Gendr_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Gendr_Cd", goodValue: appl2.Appl_Dbtr_Gendr_Cd, badValue: appl3.Appl_Dbtr_Gendr_Cd));
            if (appl2.Appl_Dbtr_Entrd_SIN != appl3.Appl_Dbtr_Entrd_SIN) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Entrd_SIN", goodValue: appl2.Appl_Dbtr_Entrd_SIN, badValue: appl3.Appl_Dbtr_Entrd_SIN));
            if (appl2.Appl_Dbtr_Cnfrmd_SIN != appl3.Appl_Dbtr_Cnfrmd_SIN) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Cnfrmd_SIN", goodValue: appl2.Appl_Dbtr_Cnfrmd_SIN, badValue: appl3.Appl_Dbtr_Cnfrmd_SIN));
            if (appl2.Appl_Dbtr_RtrndBySrc_SIN != appl3.Appl_Dbtr_RtrndBySrc_SIN) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_RtrndBySrc_SIN", goodValue: appl2.Appl_Dbtr_RtrndBySrc_SIN, badValue: appl3.Appl_Dbtr_RtrndBySrc_SIN));
            if (appl2.Appl_Dbtr_Addr_Ln != appl3.Appl_Dbtr_Addr_Ln) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_Ln", goodValue: appl2.Appl_Dbtr_Addr_Ln, badValue: appl3.Appl_Dbtr_Addr_Ln));
            if (appl2.Appl_Dbtr_Addr_Ln1 != appl3.Appl_Dbtr_Addr_Ln1) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_Ln1", goodValue: appl2.Appl_Dbtr_Addr_Ln1, badValue: appl3.Appl_Dbtr_Addr_Ln1));
            if (appl2.Appl_Dbtr_Addr_CityNme != appl3.Appl_Dbtr_Addr_CityNme) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_CityNme", goodValue: appl2.Appl_Dbtr_Addr_CityNme, badValue: appl3.Appl_Dbtr_Addr_CityNme));
            if (appl2.Appl_Dbtr_Addr_PrvCd != appl3.Appl_Dbtr_Addr_PrvCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_PrvCd", goodValue: appl2.Appl_Dbtr_Addr_PrvCd, badValue: appl3.Appl_Dbtr_Addr_PrvCd));
            if (appl2.Appl_Dbtr_Addr_CtryCd != appl3.Appl_Dbtr_Addr_CtryCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_CtryCd", goodValue: appl2.Appl_Dbtr_Addr_CtryCd, badValue: appl3.Appl_Dbtr_Addr_CtryCd));
            if (appl2.Appl_Dbtr_Addr_PCd != appl3.Appl_Dbtr_Addr_PCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Dbtr_Addr_PCd", goodValue: appl2.Appl_Dbtr_Addr_PCd, badValue: appl3.Appl_Dbtr_Addr_PCd));
            if (appl2.Medium_Cd != appl3.Medium_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Medium_Cd", goodValue: appl2.Medium_Cd, badValue: appl3.Medium_Cd));
            if (appl2.Appl_Reactv_Dte != appl3.Appl_Reactv_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Reactv_Dte", goodValue: appl2.Appl_Reactv_Dte, badValue: appl3.Appl_Reactv_Dte));
            if (appl2.Appl_SIN_Cnfrmd_Ind != appl3.Appl_SIN_Cnfrmd_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_SIN_Cnfrmd_Ind", goodValue: appl2.Appl_SIN_Cnfrmd_Ind, badValue: appl3.Appl_SIN_Cnfrmd_Ind));
            if (appl2.AppCtgy_Cd != appl3.AppCtgy_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "AppCtgy_Cd", goodValue: appl2.AppCtgy_Cd, badValue: appl3.AppCtgy_Cd));
            if (appl2.AppReas_Cd != appl3.AppReas_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "AppReas_Cd", goodValue: appl2.AppReas_Cd, badValue: appl3.AppReas_Cd));
            if (appl2.Appl_Create_Dte.Date != appl3.Appl_Create_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Create_Dte", goodValue: appl2.Appl_Create_Dte, badValue: appl3.Appl_Create_Dte));
            if (appl2.Appl_Create_Usr != appl3.Appl_Create_Usr) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Create_Usr", goodValue: appl2.Appl_Create_Usr, badValue: appl3.Appl_Create_Usr));
            if (appl2.Appl_LastUpdate_Dte?.Date != appl3.Appl_LastUpdate_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_LastUpdate_Dte", goodValue: appl2.Appl_LastUpdate_Dte, badValue: appl3.Appl_LastUpdate_Dte));
            if (appl2.Appl_LastUpdate_Usr != appl3.Appl_LastUpdate_Usr) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_LastUpdate_Usr", goodValue: appl2.Appl_LastUpdate_Usr, badValue: appl3.Appl_LastUpdate_Usr));
            if (appl2.ActvSt_Cd != appl3.ActvSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "ActvSt_Cd", goodValue: appl2.ActvSt_Cd, badValue: appl3.ActvSt_Cd));
            if (appl2.AppLiSt_Cd != appl3.AppLiSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "AppLiSt_Cd", goodValue: appl2.AppLiSt_Cd, badValue: appl3.AppLiSt_Cd));
            if (appl2.Appl_WFID != appl3.Appl_WFID) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_WFID", goodValue: appl2.Appl_WFID, badValue: appl3.Appl_WFID));
            if (appl2.Appl_Crdtr_Brth_Dte != appl3.Appl_Crdtr_Brth_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_Crdtr_Brth_Dte     ", goodValue: appl2.Appl_Crdtr_Brth_Dte, badValue: appl3.Appl_Crdtr_Brth_Dte));

            return diffs;

        }
    }
}
