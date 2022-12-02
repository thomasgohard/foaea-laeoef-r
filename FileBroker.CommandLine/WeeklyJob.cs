using FileBroker.Model.Interfaces;

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
                    case "IFMSFDOUT":
                        // CreateIFMSOutboundFile(r)
                        break;

                    case "CPPBFOUT":
                    case "OASBFOUT":
                    case "TRBFOUT":
                        // CreateFinancialBlockedFundsOutBoundFile(r)
                        break;

                    case "CHEQRECFD":
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
