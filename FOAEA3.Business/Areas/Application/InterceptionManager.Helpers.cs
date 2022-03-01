using FOAEA3.Model.Enums;
using System;
using System.Linq;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class InterceptionManager : ApplicationManager
    {
        private void SendDebtorLetter()
        {
            // TODO: this checks will currently fail in the old code, but will work here... should it?
            var events = EventManager.GetEventBF(InterceptionApplication.Subm_SubmCd, Appl_CtrlCd,
                                                 EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR, "A");

            if (events.Count == 0)
                EventManager.AddBFEvent(EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR, effectiveTimestamp: DateTime.Now);

        }

        private static string GetDebtorID(string justiceID)
        {
            return justiceID.Trim().Substring(0, 7);
        }

        private static string DebtorPrefix(string name)
        {
            name = name.Trim();
            name = name.Replace("'", "");
            if (name.Length >= 3)
                return name.Substring(0, 3);
            else
                return name.PadRight(3, '-');
        }

        private string GenerateDebtorID(string debtorLastName)
        {
            return Repositories.InterceptionRepository.GetDebtorID(DebtorPrefix(debtorLastName)); ;
        }

        private string NextJusticeID(string justiceID)
        {
            string justiceSuffix = justiceID.Substring(justiceID.Length - 1, 1);
            string debtorID = justiceID.Substring(0, 7);
            if (string.IsNullOrEmpty(justiceID))
                justiceSuffix = "A";
            else
                justiceSuffix = GetNextSuffixLetter(justiceSuffix);

            if (justiceSuffix == "@")
                justiceSuffix = "A";

            string newJusticeNumber = debtorID.Trim() + justiceSuffix.Trim();

            bool isAlreadyUser = Repositories.InterceptionRepository.IsAlreadyUsedJusticeNumber(newJusticeNumber);

            if (isAlreadyUser)
                justiceSuffix = NextJusticeID(justiceSuffix);

            nextJusticeID_callCount++;
            if (nextJusticeID_callCount > 26)
                throw new Exception("Infinite loop!");

            return justiceSuffix;
        }

        private static string GetNextSuffixLetter(string previousJusticeSuffix)
        {
            const string JUSTICE_ID_SUFFIX = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int position = JUSTICE_ID_SUFFIX.IndexOf(previousJusticeSuffix);
            if ((position >= 25) || (position == -1) || (position == JUSTICE_ID_SUFFIX.Length - 1))
                return "@";
            else
                return JUSTICE_ID_SUFFIX.Substring(position + 1, 1);
        }

        private bool IsESD_MEP(string enforcementServiceCode)
        {
            return config.ESDsites.Contains(enforcementServiceCode);
        }

        private void ProcessSummSmryBFN(string debtorId, ref EventCode eventBFNreasonCode)
        {
            var summSmryData = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(debtorId: debtorId);

            foreach (var summSmryItem in summSmryData)
                if ((summSmryItem.Appl_EnfSrv_Cd != Appl_EnfSrv_Cd) && (summSmryItem.Appl_CtrlCd != Appl_CtrlCd))
                    ProcessBringForwardNotification(summSmryItem.Appl_EnfSrv_Cd, summSmryItem.Appl_CtrlCd, ref eventBFNreasonCode);
        }

        private void ProcessBringForwardNotification(string applEnfSrvCode, string applControlCode, ref EventCode eventBFNreasonCode)
        {
            var thisApplicationManager = new InterceptionManager(Repositories, RepositoriesFinance, config);
            var thisApplication = thisApplicationManager.InterceptionApplication;
            var applEventManager = new ApplicationEventManager(thisApplication, Repositories);
            var applEventDetailManager = new ApplicationEventDetailManager(thisApplication, Repositories);

            var eventBFNs = applEventManager.GetApplicationEventsForQueue(EventQueue.EventBFN).Where(m => m.ActvSt_Cd == "A");
            var eventBFNDetails = applEventDetailManager.GetApplicationEventDetailsForQueue(EventQueue.EventBFN_dtl).Where(m => m.ActvSt_Cd == "A");

            foreach (var thisEventBFN in eventBFNs)
            {
                EventCode thisEventCode = EventCode.UNDEFINED;
                if (thisEventBFN.Event_Reas_Cd.HasValue)
                    thisEventCode = thisEventBFN.Event_Reas_Cd.Value;

                switch (thisEventCode)
                {
                    case EventCode.C56003_CANCELLED_OR_COMPLETED_BFN:
                        eventBFNreasonCode = EventCode.C56002_NEW_BFN_FOR_EXISTING_DEBTOR;
                        thisEventBFN.ActvSt_Cd = "C";
                        thisEventBFN.Event_Compl_Dte = DateTime.Now;
                        break;
                    case EventCode.C56001_NEW_BFN_FOR_NEW_DEBTOR:
                        eventBFNreasonCode = EventCode.UNDEFINED;
                        break;
                    case EventCode.C56002_NEW_BFN_FOR_EXISTING_DEBTOR:
                        eventBFNreasonCode = EventCode.UNDEFINED;
                        break;
                    case EventCode.UNDEFINED:
                        eventBFNreasonCode = EventCode.C56001_NEW_BFN_FOR_NEW_DEBTOR;
                        break;
                }

            }

            foreach (var thisEventBFNdetail in eventBFNDetails)
            {
                EventCode thisEventDetailCode = EventCode.UNDEFINED;
                if (thisEventBFNdetail.Event_Reas_Cd.HasValue)
                    thisEventDetailCode = thisEventBFNdetail.Event_Reas_Cd.Value;

                if (thisEventDetailCode == EventCode.C56003_CANCELLED_OR_COMPLETED_BFN)
                {
                    thisEventBFNdetail.ActvSt_Cd = "C";
                    thisEventBFNdetail.Event_Compl_Dte = DateTime.Now;
                    thisEventBFNdetail.Event_Reas_Text = $"Application {thisApplication.Appl_EnfSrv_Cd}-{thisApplication.Appl_CtrlCd} became active, STOP BLOCK not sent";
                }
            }

            applEventManager.SaveEvents();
            applEventDetailManager.SaveEventDetails();
        }

    }
}
