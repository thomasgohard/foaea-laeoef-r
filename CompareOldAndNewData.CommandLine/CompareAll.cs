using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareAll
    {
        public static void Run(DbRepositories repositories2, DbRepositories_Finance repositories2Finance,
                               DbRepositories repositories3, DbRepositories_Finance repositories3Finance,
                               string enfSrv, string ctrlCd, DateTime foaea2RunDate, DateTime foaea3RunDate)
        {
            var diffs = CompareAppl.Run("Appl", repositories2, repositories3, enfSrv, ctrlCd);
            diffs.AddRange(CompareSummSmry.Run("SummSmry", repositories2Finance, repositories3Finance, enfSrv, ctrlCd));
            diffs.AddRange(CompareIntFinH.Run("IntFinH", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(CompareHldbCnd.Run("HldbCnd", repositories2, repositories3, enfSrv, ctrlCd));
            diffs.AddRange(CompareEvents.Run("EvntSubm", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSubm));
            diffs.AddRange(CompareEvents.Run("EvntBF", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventBF));
            diffs.AddRange(CompareEvents.Run("EvntSIN", repositories2, repositories3, enfSrv, ctrlCd, EventQueue.EventSIN));

            Console.WriteLine($"Table\tKey\tColumn\tGood\tBad");
            string lastTable = string.Empty;
            string lastKey = string.Empty;
            foreach (var diff in diffs)
            {
                bool skipBecauseOfDateDiff = false;
                if ((diff.GoodValue is DateTime goodDate) && (diff.BadValue is DateTime badDate))
                {
                    if ((goodDate.Date == foaea2RunDate) && 
                        (badDate.Date == foaea3RunDate))
                        skipBecauseOfDateDiff = true;
                }

                if (diff.ColName.ToLower() == "event_id")
                    skipBecauseOfDateDiff = true;

                if (!skipBecauseOfDateDiff)
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

                    Console.WriteLine($"{thisTable}\t{thisKey}\t{diff.ColName}\t{diff.GoodValue}\t{diff.BadValue}");
                }
            }
            Console.WriteLine("\n");
        }
    }
}
