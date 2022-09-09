using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using System.Text;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareAll
    {
        public static async Task RunAsync(DbRepositories repositories2, DbRepositories_Finance repositories2Finance,
                               DbRepositories repositories3, DbRepositories_Finance repositories3Finance,
                               string action, string enfSrv, string ctrlCd, DateTime foaea2RunDate, DateTime foaea3RunDate,
                               StringBuilder output)
        {
            var diffs = await CompareAppl.RunAsync("Appl", repositories2, repositories3, enfSrv, ctrlCd);
            diffs.AddRange(await CompareSummSmry.RunAsync("SummSmry", repositories2Finance, repositories3Finance, enfSrv, ctrlCd));
            diffs.AddRange(await CompareIntFinH.RunAsync("IntFinH", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(await CompareHldbCnd.RunAsync("HldbCnd", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(await CompareEvents.RunAsync("EvntSubm", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSubm));
            diffs.AddRange(await CompareEvents.RunAsync("EvntBF", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventBF));
            diffs.AddRange(await CompareEvents.RunAsync("EvntBFN", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventBFN));
            diffs.AddRange(await CompareEvents.RunAsync("EvntSIN", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSIN));

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
