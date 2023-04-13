using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class AutoSwearProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;
        private readonly ClaimsPrincipal User;

        public AutoSwearProcess(IRepositories repositories, IRepositories_Finance repositories_finance,
                                        IFoaeaConfigurationHelper config, ClaimsPrincipal user)
        {
            Config = config;
            DB = repositories;
            DBfinance = repositories_finance;
            User = user;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Auto Swear Process", "Auto Swear Process Started", "O");

            var interceptionManager = new InterceptionManager(DB, DBfinance, Config, User);
            foreach (string autoSwearEnfSrv in Config.AutoSwear)
            {
                bool isESDsite = Config.ESDsites.Contains(autoSwearEnfSrv);

                var applications = await interceptionManager.GetApplicationsForVariationAutoAcceptAsync(autoSwearEnfSrv);
                foreach (var appl in applications)
                {
                    bool exGratia = false;

                    var manager = new InterceptionManager(appl, DB, DBfinance, Config, User);
                    if (await manager.IsSinBlockedAsync() || await manager.IsRefNumberBlockedAsync())
                    {
                        exGratia = true;
                    }

                    manager.AcceptedWithin30Days = true;
                    manager.GarnisheeSummonsReceiptDate = await DB.InterceptionTable.GetGarnisheeSummonsReceiptDateAsync(
                                                                                appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, isESDsite);

                    var dateDiff = DateTime.Now - manager.GarnisheeSummonsReceiptDate.Value;
                    if (dateDiff.Days > 30)
                    {
                        manager.AcceptedWithin30Days = false;
                        await manager.RejectInterceptionAsync();
                    }
                    else
                    {
                        string enfSrv = manager.InterceptionApplication.Appl_EnfSrv_Cd;
                        string ctrlCd = manager.InterceptionApplication.Appl_CtrlCd;
                        var overrideAppl = await DB.InterceptionTable.GetAutoAcceptGarnisheeOverrideDataAsync(enfSrv, ctrlCd);
                        DateTime supportingDocDate;

                        if ((overrideAppl is not null) && (overrideAppl.Appl_CtrlCd == ctrlCd))
                            supportingDocDate = overrideAppl.Appl_RecvAffdvt_Dte ?? DateTime.Now;
                        else
                            supportingDocDate = DateTime.Now;

                        if (!exGratia)
                            await manager.AcceptInterceptionAsync(supportingDocDate);
                    }
                }
            }

            await prodAudit.InsertAsync("Auto Swear Process", "Auto Swear Process Completed", "O");
        }
    }
}
