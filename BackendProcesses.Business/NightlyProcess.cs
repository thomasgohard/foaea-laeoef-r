using DBHelper;
using FOAEA3.Model.Interfaces;

namespace BackendProcesses.Business
{
    public class NightlyProcess
    {
        public void Run(IRepositories repositories, IRepositories_Finance repositoriesFinance)
        {
            CompletedInterceptionsProcess.Run(); // formerly known as AppDaily

            var amountOwedProcess = new AmountOwedProcess(repositories, repositoriesFinance);
            amountOwedProcess.Run();

            var divertFundProcess = new DivertFundsProcess(repositories, repositoriesFinance);
            divertFundProcess.Run();
            
            ChequeReqProcess.Run();
        }
    }
}
