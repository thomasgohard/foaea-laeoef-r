using DBHelper;
using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class ESDEventProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;

        public ESDEventProcess(IRepositories repositories, IRepositories_Finance repositories_finance,
                               IFoaeaConfigurationHelper config)
        {
            DB = repositories;
            DBfinance = repositories_finance;
            Config = config;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("ESD Events Process", "ESD Events Process Started", "O");

            await UpdateESDreceivedDate();

            await ProcessRejectedInterceptions();

            await prodAudit.InsertAsync("ESD Events Process", "ESD Events Process Completed", "O");
        }

        private async Task ProcessRejectedInterceptions()
        {
            var applRejects = await DB.InterceptionTable.GetApplicationsForReject();

            foreach (var appl in applRejects)
            {
                var manager = new InterceptionManager(DB, DBfinance, Config);
                await manager.LoadApplicationAsync(appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd);

                if (appl.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
                {
                    var dateDiff = DateTime.Now - appl.Appl_Rcptfrm_Dte;
                    if (dateDiff.Days > 10)
                    {
                        manager.AcceptedWithin30Days = true;
                        await manager.RejectInterceptionAsync();
                    }
                    else
                    {
                        dateDiff = DateTime.Now - appl.Appl_Create_Dte;
                        if ((dateDiff.Days == 3) || (dateDiff.Days == 5))
                        {
                            manager.EventManager.AddEvent(EventCode.C50902_AWAITING_AN_ACTION_ON_THIS_APPLICATION, updateSubm: "F02SSS");
                            await manager.EventManager.SaveEventsAsync();
                        }
                    }
                }
                else
                {
                    bool isESDsite = Config.ESDsites.Contains(appl.Appl_EnfSrv_Cd);
                    manager.GarnisheeSummonsReceiptDate = await DB.InterceptionTable.GetGarnisheeSummonsReceiptDateAsync(
                                                                            appl.Appl_EnfSrv_Cd, appl.Appl_CtrlCd, isESDsite);

                    if (manager.GarnisheeSummonsReceiptDate is null || manager.GarnisheeSummonsReceiptDate.Value == DateTime.MinValue)
                    {
                        var dateDiff = DateTime.Now - appl.Appl_Rcptfrm_Dte;
                        if (dateDiff.Days > 18)
                        {
                            manager.AcceptedWithin30Days = true;
                            await manager.RejectInterceptionAsync();
                        }
                    }
                    else
                    {
                        var dateDiff = DateTime.Now - manager.GarnisheeSummonsReceiptDate.Value;
                        if (dateDiff.Days > 18)
                        {
                            manager.AcceptedWithin30Days = true;
                            await manager.RejectInterceptionAsync();
                        }
                    }
                }
            }
        }

        private async Task UpdateESDreceivedDate()
        {
            var esdRequiredList = await DB.InterceptionTable.GetESDrequiredAsync();
            foreach (var esdRequired in esdRequiredList)
            {
                var enfSrv = esdRequired.Appl_EnfSrv_Cd;
                var ctrlCd = esdRequired.Appl_CtrlCd;

                (bool isNew, DateTime newDate) = await DB.InterceptionTable.IsNewESDreceivedAsync(enfSrv, ctrlCd,
                                                                                                  esdRequired.ESDRequired);
                if (isNew)
                {
                    await DB.InterceptionTable.UpdateESDrequiredAsync(enfSrv, ctrlCd, newDate);
                }
                else
                {
                    var manager = new InterceptionManager(DB, DBfinance, Config);
                    await manager.LoadApplicationAsync(enfSrv, ctrlCd);

                    var dateDiff = DateTime.Now - newDate.Date;
                    if ((dateDiff.Days == 4) || (dateDiff.Days == 7))
                    {
                        manager.EventManager.AddEvent(EventCode.C50764_NO_ELECTRONIC_SUMMONS_DOCUMENT_RECEIVED_FOR_THE_APPLICATION, updateSubm: "F02SSS");
                    }
                    else if (dateDiff.Days > 10)
                    {
                        manager.AcceptedWithin30Days = true;
                        manager.ESDReceived = false;
                        await manager.RejectInterceptionAsync();
                        manager.EventManager.AddEvent(EventCode.C50765_APPLICATION_REJECTED_AS_NO_ELECTRONIC_SUMMONS_DOCUMENT_RECEIVED, updateSubm: "F02SSS");

                        await DB.InterceptionTable.UpdateESDrequiredAsync(enfSrv, ctrlCd, newDate);
                    }

                    await manager.EventManager.SaveEventsAsync();
                }
            }
        }
    }
}
