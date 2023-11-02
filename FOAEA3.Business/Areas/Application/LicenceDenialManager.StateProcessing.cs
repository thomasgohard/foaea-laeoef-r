using FOAEA3.Model.Enums;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialManager
    {
        protected override async Task Process_04_SinConfirmed()
        {
            await base.Process_04_SinConfirmed();

            // get Licence Suspension data from LicSusp table
            // if none are found, then go to state 7 (VALID_AFFIDAVIT_NOT_RECEIVED)

            if (AffidavitExists())
                await SetNewStateTo(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6);
            else
                await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

        }

        protected override async Task Process_12_PartiallyServiced()
        {
            await base.Process_12_PartiallyServiced();

            var licenceResponseData = await DB.LicenceDenialResponseTable.GetLastResponseData(Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (licenceResponseData != null)
            {

                short rqstStatCd = licenceResponseData.RqstStat_Cd;
                string source = licenceResponseData.EnfSrv_Cd;

                switch (rqstStatCd)
                {
                    case 3:
                        LicenceDenialApplication.LicSusp_AnyLicReinst_Ind = 1;
                        EventManager.AddEvent(EventCode.C50824_A_DEBTOR_LICENCE_HAS_BEEN_SUSPENDED);
                        break;
                    case 5:
                        EventManager.AddEvent(EventCode.C50827_ASSISTANCE_REQUESTED_TO_CORRECTLY_IDENTIFY_DEBTOR, 
                                              queue: EventQueue.EventAM);
                        break;
                    case 8:
                        LicenceDenialApplication.LicSusp_AnyLicRvkd_Ind = 1;
                        break;
                    default: 
                        break;
                }

            }
        }

    }
}
