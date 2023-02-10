using FOAEA3.Common.Helpers;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using FOAEA3.Web.Models;
using System;
using System.Collections.Generic;

namespace FOAEA3.Web.Helpers
{
    public class ActionHelper
    {
        public static MenuAction ExtractInfo(string value)
        {
            string[] values = value.Split(' ');
            var applKey = new ApplKey(values[0]);

            return new MenuAction
            {
                Appl_EnfSrv_Cd = applKey.EnfSrv,
                Appl_CtrlCd = applKey.CtrlCd,
                Action = (MenuActionChoice)Enum.Parse(typeof(MenuActionChoice), values[1])
            };
        }

        public static List<MenuActionChoice> GetValidActions(string submitter, string category, ApplicationState state)
        {
            var validActions = new List<MenuActionChoice>
            {
                MenuActionChoice.Menu
            };

            var isAgent = submitter.IsInternalUser();
            category = category.ToUpper().Trim();

            switch (state)
            {
                case ApplicationState.UNDEFINED:
                    break;

                case ApplicationState.INITIAL_STATE_0:
                    break;

                case ApplicationState.INVALID_APPLICATION_1:
                    break;

                case ApplicationState.AWAITING_VALIDATION_2:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.SIN_CONFIRMATION_PENDING_3:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.SIN_CONFIRMED_4:
                    break;

                case ApplicationState.SIN_NOT_CONFIRMED_5:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                    break;

                case ApplicationState.APPLICATION_REJECTED_9:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    if (isAgent)
                    {
                        validActions.Add(MenuActionChoice.Reverse);
                    }
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.APPLICATION_ACCEPTED_10:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (isAgent)
                    {
                        validActions.Add(MenuActionChoice.Reverse);
                    }
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.APPLICATION_REINSTATED_11:
                    break;

                case ApplicationState.PARTIALLY_SERVICED_12:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (isAgent)
                    {
                        validActions.Add(MenuActionChoice.Reverse);
                    }
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.FULLY_SERVICED_13:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.MANUALLY_TERMINATED_14:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    if (isAgent)
                    {
                        validActions.Add(MenuActionChoice.Reverse);
                    }
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.EXPIRED_15:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.FINANCIAL_TERMS_VARIED_17:
                    break;

                case ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    if (isAgent)
                    {
                        validActions.Add(MenuActionChoice.Approve);
                        validActions.Add(MenuActionChoice.Reject);
                    }
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Suspend);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.APPLICATION_SUSPENDED_35:
                    validActions.Add(MenuActionChoice.Notes);
                    validActions.Add(MenuActionChoice.ViewEvents);
                    validActions.Add(MenuActionChoice.View);
                    validActions.Add(MenuActionChoice.Edit);
                    validActions.Add(MenuActionChoice.Transfer);
                    validActions.Add(MenuActionChoice.Cancel);
                    validActions.Add(MenuActionChoice.ViewQCissues);
                    validActions.Add(MenuActionChoice.ViewQChistory);
                    if (category != "T01") validActions.Add(MenuActionChoice.LinkT01);
                    if (category != "I01") validActions.Add(MenuActionChoice.LinkI01);
                    if (category != "L01") validActions.Add(MenuActionChoice.LinkL01);
                    break;

                case ApplicationState.INVALID_VARIATION_SOURCE_91:
                    break;

                case ApplicationState.INVALID_VARIATION_FINTERMS_92:
                    break;

                case ApplicationState.VALID_FINANCIAL_VARIATION_93:
                    break;
            }

            return validActions;
        }
    }
}
