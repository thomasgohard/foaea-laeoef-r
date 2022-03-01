using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;

namespace BackendProcesses.Business
{
    public class BringForwardEventProcess
    {
        private readonly CustomConfig config;
        private readonly IRepositories Repositories;

        public BringForwardEventProcess(IRepositories repositories, CustomConfig config)
        {
            this.config = config;
            Repositories = repositories;
        }

        public void Run()
        {
            var prodAudit = Repositories.ProductionAuditRepository;
            var dbApplicationEvent = Repositories.ApplicationEventRepository;
            var dbApplication = Repositories.ApplicationRepository; // use ApplicationManager() instead?
            var dbNotification = Repositories.NotificationRepository;

            prodAudit.Insert("BF Events Process", "BF Events Process Started", "O");

            var bfEventList = dbApplicationEvent.GetActiveEventBFs();
            foreach (var bfEvent in bfEventList)
            {
                try
                {
                    var application = dbApplication.GetApplication(bfEvent.Appl_EnfSrv_Cd, bfEvent.Appl_CtrlCd);

                    switch (application.AppCtgy_Cd)
                    {
                        case "I01":
                            break;

                        case "T01":
                            var tracingData = new TracingApplicationData();
                            tracingData.Merge(application);
                            // TODO: call API instead?
                            //var tracingManager = new TracingManager(tracingData, Repositories, config);
                            //tracingManager.ProcessBringForwards(bfEvent);
                            break;

                        case "L01":
                            var licencingData = new LicenceDenialData();
                            licencingData.Merge(application);
                            // TODO: call API instead?
                            //var licencingManager = new LicenceDenialManager(licencingData, Repositories, config);
                            //licencingManager.ProcessBringForwards(bfEvent);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception e)
                {
                    dbNotification.SendEmail("BF Error",
                        "", // System.Configuration.ConfigurationManager.AppSettings("EmailRecipients")
                        e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                }
            }

            prodAudit.Insert("BF Events Process", "BF Events Process Completed", "O");
        }
    }
}
