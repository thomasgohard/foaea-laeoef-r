using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Threading.Tasks;

namespace BackendProcesses.Business
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
            var dbApplication = DB.ApplicationTable; // use ApplicationManager() instead?
            var dbNotification = DB.NotificationService;

            await prodAudit.InsertAsync("BF Events Process", "BF Events Process Started", "O");

            var bfEventList = await dbApplicationEvent.GetActiveEventBFsAsync();
            foreach (var bfEvent in bfEventList)
            {
                try
                {
                    var application = await dbApplication.GetApplicationAsync(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);

                    switch (application.AppCtgy_Cd)
                    {
                        case "I01":
                            break;

                        case "T01":
                            var tracingData = new TracingApplicationData();
                            tracingData.Merge(application);
                            // TODO: call API instead?
                            //var tracingManager = new TracingManager(tracingData, DB, config);
                            //tracingManager.ProcessBringForwards(bfEvent);
                            break;

                        case "L01":
                            var licencingData = new LicenceDenialApplicationData();
                            licencingData.Merge(application);
                            // TODO: call API instead?
                            //var licencingManager = new LicenceDenialManager(licencingData, DB, config);
                            //licencingManager.ProcessBringForwards(bfEvent);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception e)
                {
                    await dbNotification.SendEmailAsync("BF Error",
                        "", // System.Configuration.ConfigurationManager.AppSettings("EmailRecipients")
                        e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                }
            }

            await prodAudit.InsertAsync("BF Events Process", "BF Events Process Completed", "O");
        }
    }
}
