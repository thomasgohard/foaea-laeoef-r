using DBHelper;
using FOAEA3.Common.Models;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources;
using System;

namespace FOAEA3.Business.Areas.Application
{
    internal class TracingValidation : ApplicationValidation
    {
        private TracingApplicationData TracingApplication { get; }
        public TracingValidation(TracingApplicationData tracingApplication, ApplicationEventManager eventManager, IRepositories repositories,
                                 IFoaeaConfigurationHelper config, FoaeaUser user) :
                                    base(tracingApplication, eventManager, repositories, config, user)
        {
            TracingApplication = tracingApplication;
        }

        public TracingValidation(TracingApplicationData tracingApplication, IRepositories repositories,
                                 IFoaeaConfigurationHelper config, FoaeaUser user) : base(tracingApplication, repositories, config, user)
        {
            TracingApplication = tracingApplication;
        }

        public override bool IsValidMandatoryData()
        {
            bool isSuccess = base.IsValidMandatoryData();

            if (IsC78())
            {
                isSuccess = isSuccess && IsValidDeclaration();
                isSuccess = isSuccess && IsValidPurpose();
                isSuccess = isSuccess && IsValidRequestedTracingData();
                isSuccess = isSuccess && IsValidRequestedSinData();
                isSuccess = isSuccess && IsValidRequestedFinancialsData();
            }
            else
            {
                if (!string.IsNullOrEmpty(TracingApplication.Medium_Cd) && (TracingApplication.Medium_Cd.ToUpper() == "FTP"))
                {
                    isSuccess = IsValidMandatory(isSuccess, TracingApplication.Appl_Crdtr_FrstNme, Resources.ErrorResource.MISSING_CREDITOR_FIRST_NAME);
                    isSuccess = IsValidMandatory(isSuccess, TracingApplication.Appl_Crdtr_SurNme, Resources.ErrorResource.MISSING_CREDITOR_SURNAME);
                    isSuccess = IsValidMandatory(isSuccess, TracingApplication.FamPro_Cd, Resources.ErrorResource.MISSING_FAMPRO);
                    isSuccess = IsValidMandatory(isSuccess, TracingApplication.Trace_Breach_Text, Resources.ErrorResource.MISSING_BREACH);
                }
            }

            return isSuccess;
        }

        private bool IsValidDeclaration()
        {
            string declaration = TracingApplication.Declaration?.Trim();
            if (declaration is not null &&
                (declaration.Equals(Config.TracingDeclaration.English, StringComparison.InvariantCultureIgnoreCase) ||
                 declaration.Equals(Config.TracingDeclaration.French, StringComparison.InvariantCultureIgnoreCase)))
                return true;
            else
            {
                TracingApplication.Messages.AddError("Invalid or missing declaration.");
                return false;
            }
        }

        private bool IsValidPurpose()
        {
            if (!TracingApplication.AppReas_Cd.In("1", "2", "3"))
            {
                TracingApplication.Messages.AddError(ErrorResource.INVALID_PURPOSE);
                return false;
            }
            else
                return true;
        }

        private bool IsValidRequestedTracingData()
        {
            if (IsC78() && (TracingApplication.AppReas_Cd == "3") &&
                TracingApplication.TraceInformation.In<short>((short)TracingInformationType.ResidentialAddress_EmployerAddress_Name,
                                                              (short)TracingInformationType.EmployerAddress_Name))
            {
                TracingApplication.Messages.AddError(ErrorResource.INVALID_TRACING_INFORMATION_TYPE);
                return false;
            }
            else
                return true;
        }

        private bool IsValidRequestedSinData()
        {
            if (IsC78() && (TracingApplication.AppReas_Cd != "1") && TracingApplication.IncludeSinInformation)
            {
                TracingApplication.Messages.AddError(ErrorResource.INVALID_REQUEST_SIN_DATA);
                return false;
            }
            else
                return true;
        }

        private bool IsValidRequestedFinancialsData()
        {
            if (IsC78() && (TracingApplication.AppReas_Cd != "1") && TracingApplication.IncludeFinancialInformation)
            {
                TracingApplication.Messages.AddError(ErrorResource.INVALID_REQUEST_FINANCIAL_DATA);
                return false;
            }
            else
                return true;
        }
    }
}
