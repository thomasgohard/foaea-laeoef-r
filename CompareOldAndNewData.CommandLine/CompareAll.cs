using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System.Text;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareAll
    {
        public static void Run(DbRepositories repositories2, DbRepositories_Finance repositories2Finance,
                               DbRepositories repositories3, DbRepositories_Finance repositories3Finance,
                               string action, string enfSrv, string ctrlCd, DateTime foaea2RunDate, DateTime foaea3RunDate,
                               StringBuilder output)
        {
            var diffs = CompareAppl.Run("Appl", repositories2, repositories3, enfSrv, ctrlCd);
            diffs.AddRange(CompareSummSmry.Run("SummSmry", repositories2Finance, repositories3Finance, enfSrv, ctrlCd));
            diffs.AddRange(CompareIntFinH.Run("IntFinH", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(CompareHldbCnd.Run("HldbCnd", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(CompareEvents.Run("EvntSubm", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSubm));
            diffs.AddRange(CompareEvents.Run("EvntBF", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventBF));
            diffs.AddRange(CompareEvents.Run("EvntBFN", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventBFN));
            diffs.AddRange(CompareEvents.Run("EvntSIN", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSIN));
            // diffs.AddRange(CompareEISOOUT.Run("Prcs_EISOOUT_History", repositories2, repositories3, enfSrv, ctrlCd));

            diffs.RemoveAll(m => (m.GoodValue is DateTime goodValue) && (m.BadValue is DateTime badValue) &&
                                 (goodValue.Date == foaea2RunDate) && (badValue.Date == foaea3RunDate));
            diffs.RemoveAll(m => m.ColName.ToLower() == "event_id");

            if (diffs.Any())
            {
                output.AppendLine($"\nAction\tTable\tKey\tColumn\tGood\tBad\tDescription");
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
            else
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd);
                output.AppendLine($"{action}\t{key}: No differences found.");
            }
        }
    }
}
