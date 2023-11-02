using DBHelper;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using System.Text;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareAll
    {
        public static async Task RunAsync(DbRepositories foaea2db, DbRepositories_Finance foaea2financeDb,
                               DbRepositories foaea3db, DbRepositories_Finance foaea3financeDb,
                               string action, string enfSrv, string ctrlCd, DateTime foaea2RunDate, DateTime foaea3RunDate,
                               StringBuilder output)
        {
            var appl = await foaea2db.ApplicationTable.GetApplication(enfSrv, ctrlCd);
            if (appl is null)
                return;

            string category = appl.AppCtgy_Cd.ToUpper();

            var diffs = await CompareAppl.Run("Appl", foaea2db, foaea3db, enfSrv, ctrlCd, category);

            if (category == "I01")
            {
                diffs.AddRange(await CompareSummSmry.Run("SummSmry", foaea2financeDb, foaea3financeDb, enfSrv, ctrlCd, category));
                diffs.AddRange(await CompareIntFinH.Run("IntFinH", foaea2db, foaea3db, enfSrv, ctrlCd, category, foaea2RunDate, foaea3RunDate));
                diffs.AddRange(await CompareHldbCnd.Run("HldbCnd", foaea2db, foaea3db, enfSrv, ctrlCd, category, foaea2RunDate, foaea3RunDate));
                diffs.AddRange(await CompareEvents.Run("EvntDbtr", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventDbtr));
            }

            diffs.AddRange(await CompareEvents.Run("EvntSubm", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventSubm));

            if (category != "L03")
            {
                diffs.AddRange(await CompareEvents.Run("EvntBF", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventBF));
                diffs.AddRange(await CompareEvents.Run("EvntBFN", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventBFN));
                diffs.AddRange(await CompareEvents.Run("EvntSIN", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventSIN));
            }

            if (category.In("L01", "L03"))
            {
                diffs.AddRange(await CompareEvents.Run("EvntLicence", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventLicence));
                diffs.AddRange(await CompareLicSusp.Run("LicSusp", foaea2db, foaea3db, enfSrv, ctrlCd, category));
            }

            if (category == "T01")
            {
                diffs.AddRange(await CompareEvents.Run("EvntTrace", foaea2db, foaea3db, enfSrv, ctrlCd, category, EventQueue.EventTrace));
            }

            CleanupData(foaea2RunDate, foaea3RunDate, diffs);

            OutputResults(action, enfSrv, ctrlCd, output, diffs, category);
        }

        private static void CleanupData(DateTime foaea2RunDate, DateTime foaea3RunDate, List<DiffData> diffs)
        {
            diffs.RemoveAll(m => (m.GoodValue is DateTime goodValue) && (m.BadValue is DateTime badValue) &&
                                 (goodValue.Date == foaea2RunDate) && (badValue.Date == foaea3RunDate));
            diffs.RemoveAll(m => m.ColName.ToLower() == "event_id");
        }

        private static void OutputResults(string action, string enfSrv, string ctrlCd, StringBuilder output, List<DiffData> diffs, string category)
        {
            if (diffs.Any())
            {
                OutputDifferences(action, output, diffs);
            }
            else
            {
                OutputNoDifferences(action, enfSrv, ctrlCd, output, category);
            }
        }

        private static void OutputNoDifferences(string action, string enfSrv, string ctrlCd, StringBuilder output, string category)
        {
            string key = ApplKey.MakeKey(enfSrv, ctrlCd) + " " + category + " ";
            output.AppendLine($"{action}\t{key}: No differences found.");
        }

        private static void OutputDifferences(string action, StringBuilder output, List<DiffData> diffs)
        {
            output.AppendLine($"\nAction\tTable\tKey\tColumn\tFoaea2\tFoaea3\tDescription");
            string lastTable = string.Empty;
            string lastKey = string.Empty;
            bool firstEntry = true;
            foreach (var diff in diffs)
            {
                string currentTable = diff.TableName;
                string currentKey = diff.Key;
                string thisTable;
                string thisKey;
                if ((currentTable != lastTable) || (currentKey != lastKey))
                {
                    lastTable = currentTable;
                    thisTable = currentTable;
                    lastKey = currentKey;
                    thisKey = currentKey;
                }
                else
                {
                    thisKey = string.Empty;
                    thisTable = string.Empty;
                }

                output.AppendLine($"{action}\t{thisTable}\t{thisKey}\t{diff.ColName}\t{diff.GoodValue}\t{diff.BadValue}\t{diff.Description}");

                if (firstEntry)
                {
                    firstEntry = false;
                    action = string.Empty;
                }
            }
            output.AppendLine("");
        }
    }
}
