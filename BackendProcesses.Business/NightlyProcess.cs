using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace BackendProcesses.Business
{
    public class NightlyProcess
    {
        public async Task Run(IRepositories repositories, IRepositories_Finance repositoriesFinance)
        {
            CompletedInterceptionsProcess.Run(); // formerly known as AppDaily

            var amountOwedProcess = new AmountOwedProcess(repositories, repositoriesFinance);
            await amountOwedProcess.RunAsync();

            var divertFundProcess = new DivertFundsProcess(repositories, repositoriesFinance);
            await divertFundProcess.RunAsync();

            ChequeReqProcess.Run();
        }
    }
}
