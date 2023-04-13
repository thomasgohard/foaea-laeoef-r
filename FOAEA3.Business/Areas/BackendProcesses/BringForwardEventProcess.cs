using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.BackendProcesses
{
    public class BringForwardEventProcess
    {
        private readonly IFoaeaConfigurationHelper config;
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;
        private readonly ClaimsPrincipal User;

        public BringForwardEventProcess(IRepositories repositories, IRepositories_Finance repositories_finance,
                                        IFoaeaConfigurationHelper config, ClaimsPrincipal user)
        {
            this.config = config;
            DB = repositories;
            DBfinance = repositories_finance;
            User = user;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;
            var dbApplicationEvent = DB.ApplicationEventTable;
            var dbNotification = DB.NotificationService;

            await prodAudit.InsertAsync("BF Events Process", "BF Events Process Started", "O");

            var bfEventList = await dbApplicationEvent.GetActiveEventBFsAsync();
            foreach (var bfEvent in bfEventList)
            {
                try
                {
                    var applicationManager = new ApplicationManager(null, DB, config, User);
                    await applicationManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);

                    switch (applicationManager.GetCategory())
                    {
                        case "I01":
                            var interceptionManager = new InterceptionManager(null, DB, DBfinance, config, User);
                            await interceptionManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);
                            await interceptionManager.ProcessBringForwardsAsync(bfEvent);
                            break;

                        case "T01":
                            var tracingManager = new TracingManager(null, DB, config, User);
                            await tracingManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);
                            await tracingManager.ProcessBringForwardsAsync(bfEvent);
                            break;

                        case "L01":
                            var licencingManager = new LicenceDenialManager(null, DB, config, User);
                            await licencingManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);
                            await licencingManager.ProcessBringForwardsAsync(bfEvent);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception e)
                {
                    await dbNotification.SendEmailAsync("BF Error", config.Recipients.EmailRecipients, e.Message + "\n\n" + e.StackTrace);
                }
            }

            await prodAudit.InsertAsync("BF Events Process", "BF Events Process Completed", "O");
        }
    }
}
