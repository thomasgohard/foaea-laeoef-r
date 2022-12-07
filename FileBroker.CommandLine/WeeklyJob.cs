using FileBroker.Model.Interfaces;
using Outgoing.FileCreator.Fed.Interception;
using Outgoing.FileCreator.IFMS;

namespace FileBroker.CommandLine
{
    internal class WeeklyJob
    {
        // Date.Now.DayOfWeek
        public static async Task Run(IFileTableRepository fileTable)
        {
            var jobs = (await fileTable.GetAllActiveAsync()).Where(j => j.Frequency == (int)DateTime.Now.DayOfWeek);
            foreach (var job in jobs)
            {
                switch (job.Category.ToUpper())
                {
                    case "IFMSFDOUT": // PrcId = 300
                        await OutgoingFileCreatorIFMS.Run();
                        break;

                    case "OASBFOUT":  // PrcId = 43
                    case "TRBFOUT":   // PrcId = 46
                        var process = new string[] { job.Category.ToUpper() };
                        await OutgoingFileCreatorFedInterception.RunBlockFunds(process);
                        break;

                    case "CHEQRECFD": // PrcId = (301, 302, 303, 304, 310, 322, 323, 324, 325, 326, 328)
                        // CreateCheqRecFinancialDetailOutboundFile(r)
                        break;

                    default:
                        // unknown??
                        break;
                }
            }
        }
    }
}
