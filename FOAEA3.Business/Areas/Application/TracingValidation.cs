using FOAEA3.Model;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Business.Areas.Application
{
    internal class TracingValidation : ApplicationValidation
    {
        private TracingApplicationData TracingApplication { get; }
        public TracingValidation(TracingApplicationData tracingApplication, ApplicationEventManager eventManager, IRepositories repositories,
                                 CustomConfig config) : base(tracingApplication, eventManager, repositories, config)
        {
            TracingApplication = tracingApplication;
        }

        public TracingValidation(TracingApplicationData tracingApplication, IRepositories repositories,
                                 CustomConfig config) : base(tracingApplication, repositories, config)
        {
            TracingApplication = tracingApplication;
        }

        public override bool IsValidMandatoryData()
        {
            bool isSuccess = base.IsValidMandatoryData();

            if (!string.IsNullOrEmpty(TracingApplication.Medium_Cd) && (TracingApplication.Medium_Cd.ToUpper() == "FTP"))
            {
                isSuccess = IsValidMandatory(isSuccess, TracingApplication.Appl_Crdtr_FrstNme, Resources.ErrorResource.MISSING_CREDITOR_FIRST_NAME);
                isSuccess = IsValidMandatory(isSuccess, TracingApplication.Appl_Crdtr_SurNme, Resources.ErrorResource.MISSING_CREDITOR_SURNAME);
                isSuccess = IsValidMandatory(isSuccess, TracingApplication.FamPro_Cd, Resources.ErrorResource.MISSING_FAMPRO);
                isSuccess = IsValidMandatory(isSuccess, TracingApplication.Trace_Breach_Text, Resources.ErrorResource.MISSING_BREACH);
            }

            return isSuccess;
        }
    }
}
