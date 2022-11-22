using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Threading.Tasks;
using FOAEA3.Business.Areas.Application;

namespace FOAEA3.Business.BackendProcesses
{
    public class BringForwardEventProcess
    {
        private readonly RecipientsConfig config;
        private readonly IRepositories DB;

        public BringForwardEventProcess(IRepositories repositories, RecipientsConfig config)
        {
            this.config = config;
            DB = repositories;
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
                    var applicationManager = new ApplicationManager(null, DB, config);
                    await applicationManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);

                    switch (applicationManager.GetCategory())
                    {
                        case "I01":
                            break;

                        case "T01":
                            var tracingManager = new TracingManager(null, DB, config);
                            await tracingManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);
                            await tracingManager.ProcessBringForwardsAsync(bfEvent);
                            break;

                        case "L01":
                            var licencingManager = new LicenceDenialManager(null, DB, config);
                            await licencingManager.LoadApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);
                            await licencingManager.ProcessBringForwardsAsync(bfEvent);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception e)
                {
                    await dbNotification.SendEmailAsync("BF Error", config.EmailRecipients, e.Message + "\n\n" + e.StackTrace);
                }
            }

            await prodAudit.InsertAsync("BF Events Process", "BF Events Process Completed", "O");
        }
    }
}
