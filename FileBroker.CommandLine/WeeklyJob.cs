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
            var jobs = (await fileTable.GetAllActive()).Where(j => j.Frequency == (int)DateTime.Now.DayOfWeek);
            foreach (var job in jobs)
            {
                string category = job.Category.ToUpper();

                switch (category)
                {
                    case "IFMSFDOUT": // PrcId = 300
                        await OutgoingFileCreatorIFMS.Run();
                        break;

                    case "OASBFOUT":  // PrcId = 43
                    case "TRBFOUT":   // PrcId = 46
                        await OutgoingFileCreatorFedInterception.RunBlockFunds(new string[] { category });
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
