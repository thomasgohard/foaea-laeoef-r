using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareLicSusp
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories foaea2db, IRepositories foaea3db,
                                 string enfSrv, string ctrlCd, string category)
        {
            var diffs = new List<DiffData>();

            string key = ApplKey.MakeKey(enfSrv, ctrlCd);

            LicenceDenialApplicationData appl2;
            LicenceDenialApplicationData appl3;
            if (category == "L01")
            {
                appl2 = await foaea2db.LicenceDenialTable.GetLicenceDenialDataAsync(enfSrv, appl_L01_CtrlCd: ctrlCd);
                appl3 = await foaea3db.LicenceDenialTable.GetLicenceDenialDataAsync(enfSrv, appl_L01_CtrlCd: ctrlCd);
            }
            else
            {
                appl2 = await foaea2db.LicenceDenialTable.GetLicenceDenialDataAsync(enfSrv, appl_L03_CtrlCd: ctrlCd);
                appl3 = await foaea3db.LicenceDenialTable.GetLicenceDenialDataAsync(enfSrv, appl_L03_CtrlCd: ctrlCd);
            }

            if ((appl2 is null) && (appl3 is null))
                return diffs;

            if (appl2 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "", badValue: $"{category} not found in FOAEA 2!"));
                return diffs;
            }

            if (appl3 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: $"{category} not found in FOAEA 3!", badValue: ""));
                return diffs;
            }

            if (appl2.LicSusp_SupportOrder_Dte.Date != appl3.LicSusp_SupportOrder_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_SupportOrder_Dte", goodValue: appl2.LicSusp_SupportOrder_Dte, badValue: appl3.LicSusp_SupportOrder_Dte));
            if (appl2.LicSusp_NoticeSentToDbtr_Dte.Date != appl3.LicSusp_NoticeSentToDbtr_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_NoticeSentToDbtr_Dte", goodValue: appl2.LicSusp_NoticeSentToDbtr_Dte, badValue: appl3.LicSusp_NoticeSentToDbtr_Dte));
            if (appl2.LicSusp_CourtNme != appl3.LicSusp_CourtNme) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_CourtNme", goodValue: appl2.LicSusp_CourtNme, badValue: appl3.LicSusp_CourtNme));
            if (appl2.PymPr_Cd != appl3.PymPr_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "PymPr_Cd", goodValue: appl2.PymPr_Cd, badValue: appl3.PymPr_Cd));
            if (appl2.LicSusp_NrOfPymntsInDefault != appl3.LicSusp_NrOfPymntsInDefault) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_NrOfPymntsInDefault   ", goodValue: appl2.LicSusp_NrOfPymntsInDefault, badValue: appl3.LicSusp_NrOfPymntsInDefault));
            if (appl2.LicSusp_AmntOfArrears != appl3.LicSusp_AmntOfArrears) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_AmntOfArrears         ", goodValue: appl2.LicSusp_AmntOfArrears, badValue: appl3.LicSusp_AmntOfArrears));
            if (appl2.LicSusp_Dbtr_EmplNme != appl3.LicSusp_Dbtr_EmplNme) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplNme          ", goodValue: appl2.LicSusp_Dbtr_EmplNme, badValue: appl3.LicSusp_Dbtr_EmplNme));
            if (appl2.LicSusp_Dbtr_EmplAddr_Ln != appl3.LicSusp_Dbtr_EmplAddr_Ln) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_Ln      ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_Ln, badValue: appl3.LicSusp_Dbtr_EmplAddr_Ln));
            if (appl2.LicSusp_Dbtr_EmplAddr_Ln1 != appl3.LicSusp_Dbtr_EmplAddr_Ln1) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_Ln1     ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_Ln1, badValue: appl3.LicSusp_Dbtr_EmplAddr_Ln1));
            if (appl2.LicSusp_Dbtr_EmplAddr_CityNme != appl3.LicSusp_Dbtr_EmplAddr_CityNme) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_CityNme ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_CityNme, badValue: appl3.LicSusp_Dbtr_EmplAddr_CityNme));
            if (appl2.LicSusp_Dbtr_EmplAddr_PrvCd != appl3.LicSusp_Dbtr_EmplAddr_PrvCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_PrvCd   ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_PrvCd, badValue: appl3.LicSusp_Dbtr_EmplAddr_PrvCd));
            if (appl2.LicSusp_Dbtr_EmplAddr_CtryCd != appl3.LicSusp_Dbtr_EmplAddr_CtryCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_CtryCd  ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_CtryCd, badValue: appl3.LicSusp_Dbtr_EmplAddr_CtryCd));
            if (appl2.LicSusp_Dbtr_EmplAddr_PCd != appl3.LicSusp_Dbtr_EmplAddr_PCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EmplAddr_PCd     ", goodValue: appl2.LicSusp_Dbtr_EmplAddr_PCd, badValue: appl3.LicSusp_Dbtr_EmplAddr_PCd));
            if (appl2.LicSusp_Dbtr_EyesColorCd != appl3.LicSusp_Dbtr_EyesColorCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_EyesColorCd      ", goodValue: appl2.LicSusp_Dbtr_EyesColorCd, badValue: appl3.LicSusp_Dbtr_EyesColorCd));
            if (appl2.LicSusp_Dbtr_HeightUOMCd != appl3.LicSusp_Dbtr_HeightUOMCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_HeightUOMCd      ", goodValue: appl2.LicSusp_Dbtr_HeightUOMCd, badValue: appl3.LicSusp_Dbtr_HeightUOMCd));
            if (appl2.LicSusp_Dbtr_HeightQty != appl3.LicSusp_Dbtr_HeightQty) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_HeightQty        ", goodValue: appl2.LicSusp_Dbtr_HeightQty, badValue: appl3.LicSusp_Dbtr_HeightQty));
            if (appl2.LicSusp_Dbtr_Brth_CityNme != appl3.LicSusp_Dbtr_Brth_CityNme) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_Brth_CityNme     ", goodValue: appl2.LicSusp_Dbtr_Brth_CityNme, badValue: appl3.LicSusp_Dbtr_Brth_CityNme));
            if (appl2.LicSusp_Dbtr_Brth_CtryCd != appl3.LicSusp_Dbtr_Brth_CtryCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_Brth_CtryCd      ", goodValue: appl2.LicSusp_Dbtr_Brth_CtryCd, badValue: appl3.LicSusp_Dbtr_Brth_CtryCd));
            if (appl2.LicSusp_TermRequestDte?.Date != appl3.LicSusp_TermRequestDte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_TermRequestDte        ", goodValue: appl2.LicSusp_TermRequestDte, badValue: appl3.LicSusp_TermRequestDte));
            if (appl2.LicSusp_Still_InEffect_Ind != appl3.LicSusp_Still_InEffect_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Still_InEffect_Ind    ", goodValue: appl2.LicSusp_Still_InEffect_Ind, badValue: appl3.LicSusp_Still_InEffect_Ind));
            if (appl2.LicSusp_AnyLicRvkd_Ind != appl3.LicSusp_AnyLicRvkd_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_AnyLicRvkd_Ind        ", goodValue: appl2.LicSusp_AnyLicRvkd_Ind, badValue: appl3.LicSusp_AnyLicRvkd_Ind));
            if (appl2.LicSusp_AnyLicReinst_Ind != appl3.LicSusp_AnyLicReinst_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_AnyLicReinst_Ind      ", goodValue: appl2.LicSusp_AnyLicReinst_Ind, badValue: appl3.LicSusp_AnyLicReinst_Ind));
            if (appl2.LicSusp_LiStCd != appl3.LicSusp_LiStCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_LiStCd                ", goodValue: appl2.LicSusp_LiStCd, badValue: appl3.LicSusp_LiStCd));
            if (appl2.LicSusp_Appl_CtrlCd != appl3.LicSusp_Appl_CtrlCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Appl_CtrlCd           ", goodValue: appl2.LicSusp_Appl_CtrlCd, badValue: appl3.LicSusp_Appl_CtrlCd));
            if (appl2.LicSusp_Dbtr_LastAddr_Ln != appl3.LicSusp_Dbtr_LastAddr_Ln) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_Ln      ", goodValue: appl2.LicSusp_Dbtr_LastAddr_Ln, badValue: appl3.LicSusp_Dbtr_LastAddr_Ln));
            if (appl2.LicSusp_Dbtr_LastAddr_Ln1 != appl3.LicSusp_Dbtr_LastAddr_Ln1) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_Ln1     ", goodValue: appl2.LicSusp_Dbtr_LastAddr_Ln1, badValue: appl3.LicSusp_Dbtr_LastAddr_Ln1));
            if (appl2.LicSusp_Dbtr_LastAddr_CityNme != appl3.LicSusp_Dbtr_LastAddr_CityNme) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_CityNme ", goodValue: appl2.LicSusp_Dbtr_LastAddr_CityNme, badValue: appl3.LicSusp_Dbtr_LastAddr_CityNme));
            if (appl2.LicSusp_Dbtr_LastAddr_PrvCd != appl3.LicSusp_Dbtr_LastAddr_PrvCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_PrvCd   ", goodValue: appl2.LicSusp_Dbtr_LastAddr_PrvCd, badValue: appl3.LicSusp_Dbtr_LastAddr_PrvCd));
            if (appl2.LicSusp_Dbtr_LastAddr_CtryCd != appl3.LicSusp_Dbtr_LastAddr_CtryCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_CtryCd  ", goodValue: appl2.LicSusp_Dbtr_LastAddr_CtryCd, badValue: appl3.LicSusp_Dbtr_LastAddr_CtryCd));
            if (appl2.LicSusp_Dbtr_LastAddr_PCd != appl3.LicSusp_Dbtr_LastAddr_PCd) diffs.Add(new DiffData(tableName, key: key, colName: "LicSusp_Dbtr_LastAddr_PCd     ", goodValue: appl2.LicSusp_Dbtr_LastAddr_PCd, badValue: appl3.LicSusp_Dbtr_LastAddr_PCd));

            return diffs;
        }
    }
}
