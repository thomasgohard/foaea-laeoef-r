using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareEISOOUT
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories repositories2, IRepositories repositories3,
                                         string enfSrv, string ctrlCd)
        {
            // compare table Prcs_EISOOUT_History for SIN

            var diffs = new List<DiffData>();

            string key = ApplKey.MakeKey(enfSrv, ctrlCd);

            var appl2 = await repositories2.ApplicationRepository.GetApplicationAsync(enfSrv, ctrlCd);
            var appl3 = await repositories3.ApplicationRepository.GetApplicationAsync(enfSrv, ctrlCd);

            if ((appl2 is null) && (appl3 is null))
                return diffs;

            if (appl2 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "",
                                       goodValue: "", badValue: "Not found in FOAEA 3!"));
                return diffs;
            }

            if (appl3 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "",
                                       goodValue: "Not found in FOAEA 3!", badValue: ""));
                return diffs;
            }

            var eisoout2 = (await repositories2.InterceptionRepository.GetEISOHistoryBySINAsync(appl2.Appl_Dbtr_Cnfrmd_SIN)).FirstOrDefault();
            var eisoout3 = (await repositories3.InterceptionRepository.GetEISOHistoryBySINAsync(appl3.Appl_Dbtr_Cnfrmd_SIN)).FirstOrDefault();

            if ((eisoout2 is null) && (eisoout3 is null))
                return diffs;

            if (eisoout2 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "ACCT_NBR",
                                       goodValue: "", badValue: "Not found in FOAEA 3!"));
                return diffs;
            }

            if (eisoout3 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "ACCT_NBR",
                                       goodValue: "Not found in FOAEA 3!", badValue: ""));
                return diffs;
            }

            return diffs;

        }

    }
}
