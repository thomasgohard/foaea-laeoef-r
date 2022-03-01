using FOAEA3.Model.Enums;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialManager
    {
        protected override void Process_04_SinConfirmed()
        {
            base.Process_04_SinConfirmed();

            // get Licence Suspension data from LicSusp table
            // if none are found, then go to state 7 (VALID_AFFIDAVIT_NOT_RECEIVED)

            if (AffidavitExists())
                SetNewStateTo(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);
            else
                SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

        }

    }
}
